using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.PlayerBoardEvents;
using Azul.PlayerEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace PlayerEvents
    {
        public class OnPlayerTurnScoreFinishedPayload
        {
            public int PlayerNumber { get; init; }
        }
        public class OnAllPlayersTurnScoreFinishedPayload
        {

        }

        public class OnPlayerBoardExceedsOverflowPayload
        {
            public int PlayerNumber { get; init; }
        }
    }
    namespace Controller
    {
        public class PlayerController : MonoBehaviour
        {
            [SerializeField] private List<Player> players;
            [SerializeField][Range(0, 3)] private int currentPlayer = 0;
            [SerializeField] private int turnStartsWith = 0;
            private int playerWithOneTile = -1;

            private UnityEvent<OnPlayerTurnStartPayload> onPlayerTurnStart = new UnityEvent<OnPlayerTurnStartPayload>();
            private UnityEvent<OnPlayerTurnStartPayload> onPlayerTurnEnd = new UnityEvent<OnPlayerTurnStartPayload>();
            private UnityEvent<OnPlayerTurnScoreFinishedPayload> onPlayerTurnScoreFinished = new();
            private UnityEvent<OnAllPlayersTurnScoreFinishedPayload> onAllPlayersTurnScoreFinished = new();
            private UnityEvent<OnPlayerBoardExceedsOverflowPayload> onPlayerBoardExceedsOverflow = new();


            public void InitializeListeners()
            {
                RoundController roundController = System.Instance.GetRoundController();
                roundController.AddOnRoundPhaseAcquireListener(this.OnRoundPhaseAcquireStart);
                roundController.AddOnRoundPhaseScoreListener(this.OnRoundPhaseScoreStart);
                FactoryController factoryController = System.Instance.GetFactoryController();
                factoryController.AddOnFactoryTilesDrawnListener(this.OnFactoryTilesDrawn);
                TableController tableController = System.Instance.GetTableController();
                tableController.AddOnTilesDrawnListener(this.OnTableTilesDrawn);
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                playerBoardController.AddOnPlayerAcquiresOneTileListener(this.onPlayerAcquireOneTile);
            }

            public void SetPlayers(List<string> players)
            {
                for (int idx = 0; idx < players.Count; idx++)
                {
                    GameObject playerGO = new GameObject(players[idx]);
                    Player player = playerGO.AddComponent<Player>();
                    player.Initialize(idx, players[idx]);
                    playerGO.transform.SetParent(this.transform);
                    this.players.Add(player);
                }
            }

            public void SetStartingPlayer(int player)
            {
                this.turnStartsWith = player;
                this.currentPlayer = player;
            }

            public int GetNumberOfPlayers()
            {
                return this.players.Count;
            }

            public Player GetCurrentPlayer()
            {
                return this.players[this.currentPlayer];
            }

            public void SetCurrentPlayer(int player)
            {
                if (player < this.players.Count)
                {
                    this.currentPlayer = player;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(player), "Player number is out of bounds");
                }
            }

            public void StartTurn()
            {
                this.onPlayerTurnStart.Invoke(new OnPlayerTurnStartPayload
                {
                    PlayerNumber = this.currentPlayer,
                    Phase = System.Instance.GetRoundController().GetCurrentPhase()
                });
            }

            public void EndPlayerScoringTurn()
            {
                if (this.CanPlayerEndScoreTurn())
                {
                    this.ResolveScoreTurn();
                }
                else
                {
                    this.onPlayerBoardExceedsOverflow.Invoke(new OnPlayerBoardExceedsOverflowPayload
                    {
                        PlayerNumber = this.currentPlayer
                    });
                }
            }

            private bool CanPlayerEndScoreTurn()
            {
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                return !playerBoardController.HasExcessiveOverflow(this.currentPlayer);
            }

            private void ResolveScoreTurn()
            {
                this.onPlayerTurnScoreFinished.Invoke(new OnPlayerTurnScoreFinishedPayload
                {
                    PlayerNumber = this.currentPlayer,
                });
                if (this.GetNextPlayerNumber() == this.turnStartsWith)
                {
                    // end the round
                    this.onAllPlayersTurnScoreFinished.Invoke(new OnAllPlayersTurnScoreFinishedPayload());
                }
                else
                {
                    this.NextTurn();
                }
            }

            private int GetNextPlayerNumber()
            {
                return (this.currentPlayer + 1) % this.players.Count;
            }

            private void NextTurn()
            {
                this.currentPlayer = this.GetNextPlayerNumber();
                this.StartTurn();
            }

            public void AddOnPlayerTurnStartListener(UnityAction<OnPlayerTurnStartPayload> listener)
            {
                this.onPlayerTurnStart.AddListener(listener);
            }

            public void AddOnPlayerTurnScoreFinished(UnityAction<OnPlayerTurnScoreFinishedPayload> listener)
            {
                this.onPlayerTurnScoreFinished.AddListener(listener);
            }

            public void AddOnAllPlayersTurnScoreFinished(UnityAction<OnAllPlayersTurnScoreFinishedPayload> listener)
            {
                this.onAllPlayersTurnScoreFinished.AddListener(listener);
            }

            public void AddOnPlayerBoardExceedsOverflowListener(UnityAction<OnPlayerBoardExceedsOverflowPayload> listener)
            {
                this.onPlayerBoardExceedsOverflow.AddListener(listener);
            }


            private void OnRoundPhaseAcquireStart(OnRoundPhaseAcquirePayload payload)
            {
                if (this.playerWithOneTile >= 0)
                {
                    this.turnStartsWith = this.playerWithOneTile;
                }
                this.currentPlayer = this.turnStartsWith;
                this.StartTurn();
            }

            private void OnRoundPhaseScoreStart(OnRoundPhaseScorePayload payload)
            {
                this.currentPlayer = this.turnStartsWith;
                this.StartTurn();
            }

            private void OnFactoryTilesDrawn(OnFactoryTilesDrawn payload)
            {
                System.Instance.GetPlayerBoardController().AddDrawnTiles(this.currentPlayer, payload.TilesDrawn);
                this.NextTurn();
            }

            private void OnTableTilesDrawn(TableController.OnTableTilesDrawnPayload payload)
            {
                System.Instance.GetPlayerBoardController().AddDrawnTiles(this.currentPlayer, payload.Tiles);
                this.NextTurn();
            }

            private void onPlayerAcquireOneTile(OnPlayerAcquireOneTilePayload payload)
            {
                this.turnStartsWith = payload.PlayerNumber;
                this.playerWithOneTile = payload.PlayerNumber;
            }

        }
    }
}

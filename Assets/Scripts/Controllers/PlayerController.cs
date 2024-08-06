using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.PlayerBoardEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
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

            private void NextTurn()
            {
                this.currentPlayer = (this.currentPlayer + 1) % this.players.Count;
                this.StartTurn();
            }

            public void AddOnPlayerTurnStartListener(UnityAction<OnPlayerTurnStartPayload> listener)
            {
                this.onPlayerTurnStart.AddListener(listener);
            }

            private void OnRoundPhaseAcquireStart(OnRoundPhaseAcquirePayload payload)
            {
                if (this.playerWithOneTile >= 0)
                {
                    this.turnStartsWith = this.playerWithOneTile;
                }
                else
                {
                    this.turnStartsWith = 0;
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
            }

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Event;
using Azul.GameEvents;
using Azul.Model;
using Azul.PlayerBoardEvents;
using Azul.PlayerEvents;
using Azul.ScoreBoardEvents;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class PlayerUIController : MonoBehaviour
        {
            [SerializeField] private List<GameObject> playerUIContainers;
            [SerializeField] private AudioSource scoreIncreaseSFX;
            [SerializeField] private AudioSource scoreDecreaseSFX;
            private List<PlayerUI> playerUIs = new();
            // Start is called before the first frame update
            void Start()
            {
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnGameSetupComplete);
            }

            private void OnGameSetupComplete(OnGameSetupCompletePayload payload)
            {
                RoundController roundController = System.Instance.GetRoundController();
                PlayerController playerController = System.Instance.GetPlayerController();
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                ScoreBoardController scoreBoardController = System.Instance.GetScoreBoardController();
                foreach (Player player in playerController.GetPlayers())
                {
                    this.CreatePlayerUI(player);
                }
                playerController.AddOnBeforePlayerTurnStartListener(this.OnPlayerTurnStart);
                playerBoardController.AddOnPlayerBoardTilesCollectedListener(this.OnPlayerTilesCollected);
                playerBoardController.AddOnPlayerBoardTilesDiscardedListener(this.OnPlayerTilesDiscarded);
                playerBoardController.AddOnPlayerBoardDiscardOneTileListener(this.OnPlayerDiscardOneTile);
                playerBoardController.AddOnPlaceStarTileListener(this.OnPlayerPlaceTile);
                scoreBoardController.AddOnScoreBoardUpdatedListener(this.OnPlayerScoreUpdated);
                roundController.AddOnAllRoundsCompleteListener(this.OnAllRoundsCompleted);
            }

            private void CreatePlayerUI(Player player)
            {
                ScoreBoardController scoreBoardController = System.Instance.GetScoreBoardController();
                PlayerUI playerUI = System.Instance.GetPrefabFactory().CreatePlayerUI();
                playerUI.transform.SetParent(this.playerUIContainers[player.GetPlayerNumber()].transform);
                playerUI.transform.localScale = Vector3.one;
                playerUI.SetPlayer(player);
                playerUI.SetActive(false);
                playerUI.SetScore(scoreBoardController.GetPlayerScore(player.GetPlayerNumber()));
                this.playerUIs.Add(playerUI);
            }

            private void OnPlayerTurnStart(OnBeforePlayerTurnStartPayload payload)
            {
                foreach (PlayerUI playerUI in this.playerUIs)
                {
                    playerUI.SetActive(playerUI.GetPlayerNumber() == payload.Player.GetPlayerNumber());
                }
            }

            private void OnPlayerScoreUpdated(OnScoreBoardUpdatePayload payload)
            {
                PlayerUI playerUI = this.playerUIs[payload.PlayerNumber];
                int newScore = payload.Scores[payload.PlayerNumber];
                if (playerUI.GetScore() != newScore)
                {
                    if (newScore < playerUI.GetScore())
                    {
                        this.scoreDecreaseSFX.Play();
                    }
                    this.playerUIs[payload.PlayerNumber].UpdateScore(payload.Scores[payload.PlayerNumber], .5f);
                }
            }

            private void OnPlayerTilesCollected(OnPlayerBoardTilesCollectedPayload payload)
            {
                PlayerUI playerUI = this.playerUIs[payload.PlayerNumber];
                playerUI.UpdateTileCount(payload.TileCounts);
            }

            private void OnPlayerTilesDiscarded(OnPlayerBoardTilesDiscardedPayload payload)
            {
                this.RecalculatePlayerTileCounts(payload.PlayerNumber);
            }

            private void OnPlayerDiscardOneTile(OnPlayerBoardDiscardOneTilePayload payload)
            {
                this.RecalculatePlayerTileCounts(payload.PlayerNumber);
            }

            private void OnPlayerPlaceTile(EventTracker<OnPlayerBoardPlaceStarTilePayload> payload)
            {
                this.RecalculatePlayerTileCounts(payload.Payload.PlayerNumber);
                payload.Done();
            }

            private void RecalculatePlayerTileCounts(int playerNumber)
            {
                UnityEngine.Debug.Log($"Recalcuating tile counts: {playerNumber}");
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                List<ColoredValue<int>> tileCounts = playerBoardController.GetPlayerBoard(playerNumber).GetTileCounts(true).Select(tileCount => tileCount.ToColoredValue()).ToList();
                PlayerUI playerUI = this.playerUIs[playerNumber];
                playerUI.UpdateTileCount(tileCounts);
            }

            private void OnAllRoundsCompleted(OnAllRoundsCompletePayload payload)
            {
                this.playerUIContainers.ForEach(playerUiContainer => playerUiContainer.SetActive(false));
            }

            public Vector3 GetTileCountPosition(int playerNumber, TileColor tileColor)
            {
                PlayerUI playerUI = this.playerUIs[playerNumber];
                return playerUI.GetTileCountPosition(tileColor);
            }

            public Vector3 GetScoreWorldPosition(int playerNumber)
            {
                return this.playerUIs[playerNumber].GetScoreWorldPosition();
            }
            public Vector3 GetScoreScreenPosition(int playerNumber)
            {
                return this.playerUIs[playerNumber].GetScoreScreenPosition();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Azul.GameEvents;
using Azul.ScoreBoardEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace GameEvents
    {
        public struct OnGameSetupCompletePayload
        {
            public int NumberOfPlayers { get; init; }
        }
    }
    namespace Controller
    {
        public class GameController : MonoBehaviour
        {
            [SerializeField] private UnityEvent<OnGameSetupCompletePayload> onGameSetupComplete = new();
            public void StartGame()
            {
                AIController aiController = System.Instance.GetAIController();
                BagController bagController = System.Instance.GetBagController();
                CameraController cameraController = System.Instance.GetCameraController();
                FactoryController factoryController = System.Instance.GetFactoryController();
                PlayerController playerController = System.Instance.GetPlayerController();
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                RoundController roundController = System.Instance.GetRoundController();
                ScoreBoardController scoreBoardController = System.Instance.GetScoreBoardController();
                StarController starController = System.Instance.GetStarController();
                TableController tableController = System.Instance.GetTableController();
                TileController tileController = System.Instance.GetTileController();
                UIController uIController = System.Instance.GetUIController();
                // TODO - this should be triggered by the UI/Player Ready
                aiController.SetupGame(playerController.GetPlayers());
                cameraController.SetupGame();
                tableController.SetupGame();
                playerBoardController.SetupGame(playerController.GetNumberOfPlayers(), starController);
                scoreBoardController.SetupGame(playerController.GetNumberOfPlayers());
                factoryController.SetupGame(playerController.GetNumberOfPlayers());
                tileController.SetupGame();
                bagController.SetupGame(tileController.GetTiles());
                // DEVNOTE - we will not fill the supply for now, in favor of allowing the player to select a tile of their choosing from the bag.
                // scoreBoardController.FillSupply(bagController);
                roundController.SetupGame();
                // dispatch game setup complete
                this.onGameSetupComplete.Invoke(new OnGameSetupCompletePayload
                {
                    NumberOfPlayers = playerController.GetNumberOfPlayers()
                });
                // initialize event listeners
                aiController.InitializeListeners();
                cameraController.InitializeListeners();
                factoryController.InitializeListeners();
                playerController.InitializeListeners();
                playerBoardController.InitializeListeners();
                roundController.InitializeListeners();
                scoreBoardController.InitializeListeners();
                tableController.InitializeListeners();
                tileController.InitializeListeners();
                uIController.InitializeListeners();
                scoreBoardController.AddOnScoreBoardUpdatedListener(this.OnScoreUpdate);
                // populate the table
                tableController.AddPlayerBoards(playerBoardController.GetPlayerBoards());
                tableController.AddFactories(factoryController.GetFactories());
                tableController.AddScoreBoard(scoreBoardController.GetScoreBoard());
                // Start the first round!   
                roundController.StartRound();
            }

            private void OnScoreUpdate(OnScoreBoardUpdatePayload payload)
            {
                UnityEngine.Debug.Log("Scores updated!");
                for (int idx = 0; idx < payload.Scores.Count; idx++)
                {
                    UnityEngine.Debug.Log($"Player {idx + 1}: {payload.Scores[idx]}");
                }
            }

            public void AddOnGameSetupCompleteListener(UnityAction<OnGameSetupCompletePayload> listener)
            {
                this.onGameSetupComplete.AddListener(listener);
            }

        }
    }
}

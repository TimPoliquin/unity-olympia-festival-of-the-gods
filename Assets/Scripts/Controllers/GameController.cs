using System.Collections;
using System.Collections.Generic;
using Azul.ScoreBoardEvents;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class GameController : MonoBehaviour
        {
            // Start is called before the first frame update
            void Start()
            {
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
                cameraController.SetupGame();
                tableController.SetupGame();
                playerBoardController.SetupGame(playerController.GetNumberOfPlayers(), starController);
                scoreBoardController.SetupGame(playerController.GetNumberOfPlayers());
                factoryController.SetupGame(playerController.GetNumberOfPlayers());
                tileController.SetupGame();
                bagController.SetupGame(tileController.GetTiles());
                scoreBoardController.FillSupply(bagController);
                roundController.SetupGame();
                // initialize event listeners
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
        }
    }
}

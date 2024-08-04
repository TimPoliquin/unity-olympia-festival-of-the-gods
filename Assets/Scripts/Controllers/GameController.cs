using System.Collections;
using System.Collections.Generic;
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
                FactoryController factoryController = System.Instance.GetFactoryController();
                PlayerController playerController = System.Instance.GetPlayerController();
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                RoundController roundController = System.Instance.GetRoundController();
                ScoreBoardController scoreBoardController = System.Instance.GetScoreBoardController();
                StarController starController = System.Instance.GetStarController();
                TableController tableController = System.Instance.GetTableController();
                TileController tileController = System.Instance.GetTileController();
                // TODO - this should be triggered by the UI/Player Ready
                tableController.SetupGame();
                playerBoardController.SetupGame(playerController.GetNumberOfPlayers(), starController);
                scoreBoardController.SetupGame();
                factoryController.SetupGame(playerController.GetNumberOfPlayers());
                tileController.SetupGame();
                bagController.SetupGame(tileController.GetTiles());
                scoreBoardController.FillSupply(bagController);
                roundController.SetupGame();
                // initialize event listeners
                factoryController.InitializeListeners();
                playerController.InitializeListeners();
                playerBoardController.InitializeListeners();
                scoreBoardController.InitializeListeners();
                tileController.InitializeListeners();
                // populate the table
                tableController.AddPlayerBoards(playerBoardController.GetPlayerBoards());
                tableController.AddFactories(factoryController.GetFactories());
                tableController.AddScoreBoard(scoreBoardController.GetScoreBoard());
                // Start the first round!   
                roundController.StartRound();
            }
        }
    }
}

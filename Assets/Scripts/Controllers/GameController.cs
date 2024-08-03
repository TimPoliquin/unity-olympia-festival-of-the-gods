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
                ScoreBoardController scoreBoardController = System.Instance.GetScoreBoardController();
                StarController starController = System.Instance.GetStarController();
                TileController tileController = System.Instance.GetTileController();
                // TODO - this should be triggered by the UI/Player Ready
                playerBoardController.SetupGame(playerController.GetNumberOfPlayers(), starController);
                scoreBoardController.SetupGame();
                factoryController.SetupGame(playerController.GetNumberOfPlayers());
                tileController.SetupGame();
                bagController.SetupGame(tileController.GetTiles());
                factoryController.FillFactories(bagController);
                scoreBoardController.FillSupply(bagController);

            }
        }
    }
}

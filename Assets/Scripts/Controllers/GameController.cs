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
                StarController starController = System.Instance.GetStarController();
                TileController tileController = System.Instance.GetTileController();
                // TODO - this should be triggered by the UI/Player Ready
                playerBoardController.SetupGame(playerController.GetNumberOfPlayers(), starController);
                tileController.SetupGame();
                bagController.SetupGame(tileController.GetTiles());
                factoryController.SetupGame(playerController.GetNumberOfPlayers());
                factoryController.FillFactories(bagController);
            }
        }
    }
}

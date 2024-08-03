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
                TileController tileController = System.Instance.GetTileController();
                // TODO - this should be triggered by the UI/Player Ready
                tileController.SetupGame();
                bagController.SetupGame(tileController.GetTiles());
                factoryController.SetupGame(System.Instance.GetPlayerController().GetNumberOfPlayers());
                factoryController.FillFactories(bagController);

            }
        }
    }
}

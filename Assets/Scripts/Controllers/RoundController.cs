using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class RoundController : MonoBehaviour
        {
            private int currentRound = 0;
            [SerializeField][Range(1, 6)] private int totalRounds = 6;

            public void StartRound(BagController bagController, FactoryController factoryController, TableController tableController, TileController tileController, ScoreBoardController scoreBoardController)
            {
                this.currentRound++;
                // Set round counter on scoreboard
                scoreBoardController.StartRound(this.currentRound);
                // Fill factories/refill the bag if needed
                factoryController.FillFactories(bagController);
                // Place the 1 Tile
                tableController.MoveOneTileToCenter(tileController.GetOneTile());
                // Tell TurnController which player starts first

            }

        }
    }
}

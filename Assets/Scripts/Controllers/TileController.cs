using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Azul.Model;
using Azul.GameEvents;

namespace Azul
{
    namespace Controller
    {
        public class TileController : MonoBehaviour
        {
            private static int NUM_TILES = 132;

            /// <summary>
            /// A special tile. This tile indicates which player goest first in the next steps of the current round,
            /// and which player goes first in the following round.
            /// </summary>
            private Tile oneTile;
            private List<Tile> tiles;

            public void SetupGame()
            {
                this.CreateStandardTiles();
                this.CreateOneTile();
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnGameSetupComplete);
            }

            private void CreateStandardTiles()
            {
                PrefabFactory prefabFactory = System.Instance.GetPrefabFactory();
                this.tiles = new();
                TileColor[] tileColors = TileColorUtils.GetTileColors();
                for (int idx = 0; idx < NUM_TILES; idx++)
                {
                    TileColor tileColor = tileColors[idx % tileColors.Length];
                    Tile tile = prefabFactory.CreateTile(tileColor);
                    tiles.Add(tile);
                }
                tiles.Shuffle();
            }

            private void CreateOneTile()
            {
                PrefabFactory prefabFactory = System.Instance.GetPrefabFactory();
                this.oneTile = prefabFactory.CreateTile(TileColor.ONE);
            }


            public List<Tile> GetTiles()
            {
                return this.tiles;
            }

            public Tile GetOneTile()
            {
                return this.oneTile;
            }

            private void OnGameSetupComplete(OnGameSetupCompletePayload payload)
            {
                TableController tableController = System.Instance.GetTableController();
                tableController.MoveOneTileToCenter(this.oneTile);
            }
        }
    }
}

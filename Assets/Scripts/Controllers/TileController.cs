using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Azul.Model;

namespace Azul
{
    namespace Controller
    {
        public class TileController : MonoBehaviour
        {
            private static int NUM_TILES = 132;
            [SerializeField] private GameObject tilePrefab;

            private List<Tile> tiles;

            public void SetupGame()
            {
                this.tiles = new();
                TileColor[] tileColors = TileColorUtils.GetTileColors();
                for (int idx = 0; idx < NUM_TILES; idx++)
                {
                    TileColor tileColor = tileColors[idx % tileColors.Length];
                    tiles.Add(Tile.Create(this.tilePrefab, tileColor, false));
                }
                tiles.Shuffle();
            }

            public List<Tile> GetTiles()
            {
                return this.tiles;
            }
        }
    }
}

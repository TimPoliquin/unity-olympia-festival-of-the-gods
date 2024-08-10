using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class Tower : MonoBehaviour
        {
            private List<Tile> tiles = new();

            public void Add(List<Tile> tiles)
            {
                this.tiles.AddRange(tiles);
                foreach (Tile tile in tiles)
                {
                    tile.transform.SetParent(this.transform);
                    tile.gameObject.SetActive(false);
                }
            }
            public List<Tile> Dump()
            {
                List<Tile> tiles = this.tiles;
                this.tiles = new();
                return tiles;
            }
        }
    }
}

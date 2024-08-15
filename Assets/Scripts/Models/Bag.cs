using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class Bag : MonoBehaviour
        {
            private Queue<Tile> tiles = new();

            public void Fill(List<Tile> tiles)
            {
                foreach (Tile tile in tiles)
                {
                    tile.transform.SetParent(this.transform);
                    tile.gameObject.SetActive(false);
                    this.tiles.Enqueue(tile);
                }
            }

            public List<Tile> Draw(int count)
            {
                List<Tile> drawnTiles = new List<Tile>();
                for (int idx = 0; idx < count && this.tiles.Count > 0; idx++)
                {
                    Tile tile = this.tiles.Dequeue();
                    tile.gameObject.SetActive(true);
                    drawnTiles.Add(tile);
                }
                return drawnTiles;
            }

            public Tile Draw(TileColor tileColor)
            {
                List<Tile> hackList = this.tiles.ToList();
                Tile tile = hackList.Find(tile => tile.Color == tileColor);
                if (null != tile)
                {
                    tile.gameObject.SetActive(true);
                    hackList.Remove(tile);
                    this.tiles = new Queue<Tile>(hackList);
                }
                return tile;
            }
        }

    }
}

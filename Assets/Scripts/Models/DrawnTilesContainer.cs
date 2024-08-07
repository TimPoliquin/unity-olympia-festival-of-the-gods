using System.Collections;
using System.Collections.Generic;
using Azul.Layout;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    public class DrawnTilesContainer : MonoBehaviour
    {
        [SerializeField] private LinearLayout one;
        [SerializeField] private LinearLayout red;
        [SerializeField] private LinearLayout blue;
        [SerializeField] private LinearLayout orange;
        [SerializeField] private LinearLayout purple;
        [SerializeField] private LinearLayout green;
        [SerializeField] private LinearLayout yellow;

        private Dictionary<TileColor, LinearLayout> layoutsByColor;

        void Awake()
        {
            this.layoutsByColor = new();
            layoutsByColor[TileColor.ONE] = this.one;
            layoutsByColor[TileColor.RED] = this.red;
            layoutsByColor[TileColor.BLUE] = this.blue;
            layoutsByColor[TileColor.YELLOW] = this.yellow;
            layoutsByColor[TileColor.ORANGE] = this.orange;
            layoutsByColor[TileColor.GREEN] = this.green;
            layoutsByColor[TileColor.PURPLE] = this.purple;
        }

        public void AddTiles(List<Tile> tiles)
        {
            Dictionary<TileColor, List<GameObject>> tilesByColor = new();
            foreach (Tile tile in tiles)
            {
                if (!tilesByColor.ContainsKey(tile.Color))
                {
                    tilesByColor.Add(tile.Color, new());
                }
                tilesByColor[tile.Color].Add(tile.gameObject);
            }
            foreach (var pair in tilesByColor)
            {
                this.layoutsByColor[pair.Key].AddChildren(pair.Value);
            }
        }
    }
}

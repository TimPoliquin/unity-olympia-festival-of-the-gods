using System.Collections;
using System.Collections.Generic;
using Azul.Controller.TableUtilities;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Utils
    {
        public class TileUtils
        {
            public static List<ColoredValue<int>> GetTileCounts(List<Tile> tiles)
            {
                Dictionary<TileColor, int> countsByColor = new();
                foreach (Tile tile in tiles)
                {
                    if (!countsByColor.ContainsKey(tile.Color))
                    {
                        countsByColor[tile.Color] = 1;
                    }
                    else
                    {
                        countsByColor[tile.Color] += 1;
                    }
                }
                List<ColoredValue<int>> tileCounts = new();
                foreach (KeyValuePair<TileColor, int> entry in countsByColor)
                {
                    tileCounts.Add(ColoredValue<int>.Create(entry.Key, entry.Value));
                }
                return tileCounts;
            }
        }
    }
}

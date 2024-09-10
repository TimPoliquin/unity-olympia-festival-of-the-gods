using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Controller.TableUtilities;
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

        private List<Tile> drawnTiles = new();
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
            this.drawnTiles.AddRange(tiles);
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

        public int GetTileCount(TileColor tileColor)
        {
            return this.drawnTiles.FindAll(tile => tile.Color == tileColor).Count;
        }

        public int GetTileCount()
        {
            return this.drawnTiles.FindAll(tile => !tile.IsOneTile()).Count;
        }

        public List<TileCount> GetTileCounts(bool includeOneTile = false)
        {
            List<TileCount> tileCounts = new();
            Dictionary<TileColor, int> tileCountsByColor = new();
            foreach (TileColor tileColor in TileColorUtils.GetTileColors())
            {
                tileCountsByColor[tileColor] = 0;
            }
            if (includeOneTile)
            {
                tileCountsByColor[TileColor.ONE] = 0;
            }
            foreach (Tile tile in this.drawnTiles)
            {
                if (tile.IsOneTile())
                {
                    if (includeOneTile)
                    {
                        tileCountsByColor[tile.Color] = 1;
                    }
                }
                else
                {
                    tileCountsByColor[tile.Color] += 1;
                }
            }
            foreach (KeyValuePair<TileColor, int> entry in tileCountsByColor)
            {
                tileCounts.Add(new TileCount
                {
                    TileColor = entry.Key,
                    Count = entry.Value
                });
            }
            return tileCounts;
        }

        public List<Tile> UseTiles(TileColor mainColor, int mainCount, TileColor wildColor, int wildCount)
        {
            List<Tile> usedTiles = new();
            foreach (Tile tile in this.drawnTiles)
            {
                if (tile.Color == mainColor && mainCount > 0)
                {
                    usedTiles.Add(tile);
                    mainCount--;
                }
                else if (tile.Color == wildColor && wildCount > 0)
                {
                    usedTiles.Add(tile);
                    wildCount--;
                }
                if (mainCount == 0 && wildCount == 0)
                {
                    break;
                }
            }
            if (mainCount > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(mainCount), "The player does not have enough tiles of the requested color");
            }
            else if (wildCount > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(wildCount), "The player does not have enough tiles of the requested color");
            }
            else
            {
                foreach (Tile usedTile in usedTiles)
                {
                    this.drawnTiles.Remove(usedTile);
                    usedTile.transform.SetParent(null);
                }
                this.layoutsByColor[mainColor].Refresh();
                if (this.layoutsByColor.ContainsKey(wildColor))
                {
                    this.layoutsByColor[wildColor].Refresh();
                }
                return usedTiles;
            }
        }

        public Tile DiscardOneTile()
        {
            Tile oneTile = this.drawnTiles.Find(tile => tile.IsOneTile());
            if (oneTile != null)
            {
                this.drawnTiles.Remove(oneTile);
            }
            return oneTile;
        }

        public List<Tile> DiscardRemainingTiles()
        {
            List<Tile> discard = this.drawnTiles;
            foreach (Tile tile in this.drawnTiles)
            {
                tile.transform.SetParent(null);
            }
            this.drawnTiles = new();
            return discard;
        }

        public bool HasOneTile()
        {
            return this.drawnTiles.Find(tile => tile.IsOneTile());
        }

        public GameObject GetTileContainer(TileColor tileColor)
        {
            return this.layoutsByColor[tileColor].gameObject;
        }
    }
}

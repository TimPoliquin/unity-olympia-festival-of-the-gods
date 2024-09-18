using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Layout;
using Azul.Model;
using Azul.TableEvents;
using Azul.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace TableEvents
    {
        public class OnTableAddTilesPayload
        {
            public List<Tile> Tiles { get; init; }
        }
        public class OnTableDrawTilesPayload
        {
            public List<Tile> TilesDrawn { get; init; }
        }
    }
    namespace Model
    {
        public class Table : MonoBehaviour
        {
            [SerializeField] private GameObject playerBoards;
            [SerializeField] private LinearLayout factoriesLayout;
            [SerializeField] private GameObject scoreBoard;
            [SerializeField] private GameObject center;
            [SerializeField] private float centerRadius = 10.0f;
            [SerializeField] private float dropHeight = 5.0f;
            private List<Factory> factories = new();
            private List<Tile> tiles = new();
            private UnityEvent<OnTableAddTilesPayload> onAddTiles = new();
            private UnityEvent<OnTableDrawTilesPayload> onDrawTiles = new();

            public void AddPlayerBoards(List<PlayerBoard> playerBoards)
            {
                Layout.Layout layout = this.playerBoards.GetComponent<CircularLayout>();
                layout.AddChildren(playerBoards.Select(playerBoard => playerBoard.gameObject).ToList());
            }

            public void AddFactories(List<Factory> factories)
            {
                this.factories.AddRange(factories);
                this.factoriesLayout.AddChildren(factories.Select(factory => factory.gameObject).ToList());
            }

            public void AddScoreBoard(ScoreBoard scoreBoard)
            {
                scoreBoard.transform.SetParent(this.scoreBoard.transform);
                scoreBoard.transform.localPosition = Vector3.zero;
            }

            public void AddToCenter(Tile tile, bool deadCenter)
            {
                if (this.tiles.Contains(tile))
                {
                    UnityEngine.Debug.Log($"Adding a tile that has already been added...");
                    return;
                }
                tile.transform.SetParent(this.center.transform);
                // TODO - some kind of animation is probably warranted here.
                // for now, we'll just drop it?
                if (deadCenter)
                {
                    tile.transform.localPosition = Vector3.zero + Vector3.up * this.dropHeight;
                }
                else
                {
                    tile.transform.localPosition = VectorUtils.CreateRandomVector3(this.centerRadius, this.dropHeight);
                }
                this.tiles.Add(tile);
                this.onAddTiles.Invoke(new OnTableAddTilesPayload { Tiles = new() { tile } });
            }

            public void AddToCenter(List<Tile> tiles)
            {
                foreach (Tile tile in tiles)
                {
                    tile.transform.SetParent(this.center.transform);
                    // TODO - some kind of animation is probably warranted here.
                    // for now, we'll just drop it?
                    tile.transform.localPosition = VectorUtils.CreateRandomVector3(10, 5);
                }
                this.tiles.AddRange(tiles);
                this.onAddTiles.Invoke(new OnTableAddTilesPayload { Tiles = tiles });
            }

            public void DrawTiles(List<Tile> drawnTiles)
            {
                foreach (Tile tile in drawnTiles)
                {
                    this.tiles.Remove(tile);
                    tile.transform.SetParent(null);
                }
                this.onDrawTiles.Invoke(new OnTableDrawTilesPayload { TilesDrawn = drawnTiles }); ;
            }

            public List<Factory> GetFactories()
            {
                return this.factories;
            }

            public int GetTileCount(TileColor tileColor)
            {
                return this.tiles.FindAll(tile => tile.Color == tileColor).Count;
            }

            public bool HasTileOfColor(TileColor tileColor)
            {
                return this.tiles.Any(tile => tile.Color == tileColor);
            }

            public bool HasOneTile()
            {
                return this.tiles.Any(tile => tile.IsOneTile());
            }

            public Dictionary<TileColor, int> GetTileCounts()
            {
                Dictionary<TileColor, int> tileCounts = new();
                foreach (Tile tile in this.tiles)
                {
                    if (tile.IsOneTile())
                    {
                        continue;
                    }
                    else if (!tileCounts.ContainsKey(tile.Color))
                    {
                        tileCounts[tile.Color] = 1;
                    }
                    else
                    {
                        tileCounts[tile.Color] += 1;
                    }
                }
                return tileCounts;
            }

            public void AddOnTilesAddedListener(UnityAction<OnTableAddTilesPayload> listener)
            {
                this.onAddTiles.AddListener(listener);
            }

            public void AddOnTilesDrawnListener(UnityAction<OnTableDrawTilesPayload> listener)
            {
                this.onDrawTiles.AddListener(listener);
            }

            public bool IsEmpty()
            {
                return this.tiles.Count == 0;
            }
        }
    }
}

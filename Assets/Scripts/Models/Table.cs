using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Controller;
using Azul.Layout;
using Azul.Model;
using Azul.TableEvents;
using Azul.TileAnimation;
using Azul.TileHolderEvents;
using Azul.Util;
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
            public int PlayerNumber { get; init; }
            public List<Tile> TilesDrawn { get; init; }
        }
    }
    namespace Model
    {
        public class Table : MonoBehaviour, TileProvider
        {
            [SerializeField] private GameObject playerBoards;
            [SerializeField] private LinearLayout factoriesLayout;
            [SerializeField] private GameObject scoreBoard;
            [SerializeField] private GameObject center;
            [SerializeField] private float centerWidth = 30.0f;
            [SerializeField] private float centerDepth = 10.0f;
            [SerializeField] private float dropHeight = 5.0f;
            private List<Factory> factories;
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
                this.factories = factories;
                this.factoriesLayout.AddChildren(factories.Select(factory => factory.gameObject).ToList());
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
                    tile.transform.localPosition = VectorUtils.CreateRandomVector3(this.centerWidth, this.dropHeight, this.centerDepth);
                }
                this.tiles.Add(tile);
                this.onAddTiles.Invoke(new OnTableAddTilesPayload { Tiles = new() { tile } });
            }

            public CoroutineResult AddToCenter(List<Tile> tiles)
            {
                CoroutineResult result = CoroutineResult.Single();
                this.StartCoroutine(this.MoveToCenterCoroutine(tiles, result));
                return result;
            }

            private IEnumerator MoveToCenterCoroutine(List<Tile> tiles, CoroutineResult result)
            {
                result.Start();
                FactoryController factoryController = System.Instance.GetFactoryController();
                float minX = this.center.transform.position.x;
                float maxX = this.center.transform.position.x;
                foreach (Factory factory in factoryController.GetFactories())
                {
                    if (factory.transform.position.x < minX)
                    {
                        minX = factory.transform.position.x;
                    }
                    if (factory.transform.position.x > maxX)
                    {
                        maxX = factory.transform.position.x;
                    }
                }
                float width = (Mathf.Abs(minX) + Mathf.Abs(maxX)) / 2.0f;
                float scale = width > this.centerWidth ? this.centerWidth / width : width / this.centerWidth;
                TileAnimationController tileAnimationController = System.Instance.GetTileAnimationController();
                yield return CoroutineResult.Multi(tiles.Select((tile, idx) =>
                {
                    float x = tile.transform.position.x * scale + Random.Range(-1f, 1f);
                    float z = this.center.transform.position.z - this.centerDepth / 2f + idx * this.centerDepth * 2f / tiles.Count - Random.Range(-1f, 0);
                    return tileAnimationController.MoveTiles(new() { tile }, new TilesMoveConfig()
                    {
                        Position = new Vector3(x, this.dropHeight, z),
                        Time = .25f,
                        Delay = 0,
                        AfterEach = (tile, idx) =>
                        {
                            tile.transform.SetParent(this.center.transform);
                        }
                    });
                }).ToArray()).WaitUntilCompleted();
                this.tiles.AddRange(tiles);
                this.onAddTiles.Invoke(new OnTableAddTilesPayload { Tiles = tiles });
                result.Finish();
            }

            public void DrawTiles(List<Tile> drawnTiles)
            {
                foreach (Tile tile in drawnTiles)
                {
                    this.tiles.Remove(tile);
                    tile.transform.SetParent(null);
                }
                this.onDrawTiles.Invoke(new OnTableDrawTilesPayload
                {
                    PlayerNumber = System.Instance.GetPlayerController().GetCurrentPlayer().GetPlayerNumber(),
                    TilesDrawn = drawnTiles
                }); ;
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

            public bool HasHadesToken()
            {
                return this.tiles.Any(tile => tile.IsHadesToken());
            }

            public Dictionary<TileColor, int> GetTileCounts()
            {
                Dictionary<TileColor, int> tileCounts = new();
                foreach (Tile tile in this.tiles)
                {
                    if (tile.IsHadesToken())
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

            public bool IsFactory()
            {
                return false;
            }
            public bool IsTable()
            {
                return true;
            }
        }
    }
}

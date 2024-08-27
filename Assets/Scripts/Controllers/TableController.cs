using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Controller.TableEvents;
using Azul.Controller.TableUtilities;
using Azul.Model;
using Azul.TableEvents;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Azul
{
    namespace Controller
    {
        namespace TableEvents
        {
            public class OnTableTilesDrawnPayload
            {
                public List<Tile> Tiles { get; init; }
                public bool IncludesOneTile { get; init; }
            }
            public class OnTableTilesAddedPayload
            {
                public List<Tile> Tiles { get; init; }
            }

        }
        namespace TableUtilities
        {
            public struct TileCount
            {
                public TileColor TileColor { get; init; }
                public int Count;
            }
        }
        public class TableController : MonoBehaviour
        {
            [SerializeField] private GameObject tablePrefab;

            private UnityEvent<OnTableTilesAddedPayload> onTilesAdded = new();
            private UnityEvent<OnTableTilesDrawnPayload> onTilesDrawn = new();
            private UnityEvent onTableEmpty = new();

            private Table table;

            public void SetupGame()
            {
                this.table = Instantiate(this.tablePrefab, Vector3.zero, Quaternion.identity).GetComponent<Table>();
            }

            public void InitializeListeners()
            {
                FactoryController factoryController = System.Instance.GetFactoryController();
                factoryController.AddOnFactoryTilesDiscardedListener(this.OnTilesDiscarded);
                this.table.AddOnTilesDrawnListener(this.OnTilesDrawn);
            }

            public TableSelectableTileHolderController GetTableSelectableTileHolderController()
            {
                return this.table.GetComponent<TableSelectableTileHolderController>();
            }


            public void AddPlayerBoards(List<PlayerBoard> playerBoards)
            {
                this.table.AddPlayerBoards(playerBoards);
            }

            public void AddFactories(List<Factory> factories)
            {
                this.table.AddFactories(factories);
            }

            public void AddScoreBoard(ScoreBoard scoreBoard)
            {
                this.table.AddScoreBoard(scoreBoard);
            }

            public void MoveOneTileToCenter(Tile oneTile)
            {
                this.table.AddToCenter(oneTile);
                this.onTilesAdded.Invoke(new OnTableTilesAddedPayload { Tiles = new() { oneTile } });
            }

            public void AddOnTilesAddedListener(UnityAction<OnTableTilesAddedPayload> listener)
            {
                this.onTilesAdded.AddListener(listener);
            }

            public void AddOnTilesDrawnListener(UnityAction<OnTableTilesDrawnPayload> listener)
            {
                this.onTilesDrawn.AddListener(listener);
            }

            public void AddOnTableEmptyListener(UnityAction listener)
            {
                this.onTableEmpty.AddListener(listener);
            }

            public Factory GetFactoryWithMostTilesOfColor(TileColor desiredColor)
            {
                RoundController roundController = System.Instance.GetRoundController();
                TileColor wildColor = roundController.GetCurrentRound().GetWildColor();
                List<Factory> matchingFactories = this.table.GetFactories().FindAll(factory => !factory.IsEmpty()).OrderByDescending(factory =>
                {
                    int count = factory.GetTileCount(desiredColor);
                    if (wildColor != desiredColor && factory.HasTileOfColor(wildColor))
                    {
                        count += 1;
                    }
                    return count;
                }).ToList();
                if (matchingFactories.Count > 0)
                {
                    return matchingFactories[0];
                }
                else
                {
                    return null;
                }
            }

            public int GetTableSupplyTileCount(TileColor tileColor)
            {
                return this.table.GetTileCount(tileColor);
            }

            public bool HasTableSupplyTileOfColor(TileColor tileColor)
            {
                return this.table.HasTileOfColor(tileColor);
            }

            public List<TileCount> GetTileCounts()
            {
                Dictionary<TileColor, TileCount> tileCounts = new();
                foreach (Factory factory in this.table.GetFactories())
                {
                    Dictionary<TileColor, int> factoryTileCounts = factory.GetTileCounts();
                    foreach (KeyValuePair<TileColor, int> entry in factoryTileCounts)
                    {
                        if (!tileCounts.ContainsKey(entry.Key))
                        {
                            tileCounts[entry.Key] = new TileCount
                            {
                                TileColor = entry.Key,
                                Count = entry.Value
                            };
                        }
                        else
                        {
                            TileCount tileCount = tileCounts[entry.Key];
                            tileCount.Count += entry.Value;
                        }
                    }
                }
                Dictionary<TileColor, int> tableTileCounts = this.table.GetTileCounts();
                foreach (KeyValuePair<TileColor, int> entry in tableTileCounts)
                {
                    if (!tileCounts.ContainsKey(entry.Key))
                    {
                        tileCounts[entry.Key] = new TileCount
                        {
                            TileColor = entry.Key,
                            Count = entry.Value
                        };
                    }
                    else
                    {
                        TileCount tileCount = tileCounts[entry.Key];
                        tileCount.Count += entry.Value;
                    }
                }
                return tileCounts.Values.OrderBy(tileCount => tileCount.Count).ToList();
            }

            public bool HasOneTile()
            {
                return this.table.HasOneTile();
            }

            private void OnTilesDiscarded(OnFactoryTilesDiscarded payload)
            {
                this.table.AddToCenter(payload.TilesDiscarded);
                this.onTilesAdded.Invoke(new OnTableTilesAddedPayload { Tiles = payload.TilesDiscarded });
            }

            private void OnTilesDrawn(OnTableDrawTilesPayload payload)
            {
                this.onTilesDrawn.Invoke(new OnTableTilesDrawnPayload
                {
                    Tiles = payload.TilesDrawn,
                    IncludesOneTile = payload.TilesDrawn.Find(tile => tile.Color == TileColor.ONE) != null
                });
                if (this.table.IsEmpty())
                {
                    this.onTableEmpty.Invoke();
                }
            }

        }
    }
}

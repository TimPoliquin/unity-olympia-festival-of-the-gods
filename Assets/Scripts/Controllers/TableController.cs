using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Controller.TableEvents;
using Azul.Controller.TableUtilities;
using Azul.Event;
using Azul.Model;
using Azul.TableEvents;
using Azul.TileHolderEvents;
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
                public ColoredValue<int> ToColoredValue()
                {
                    return ColoredValue<int>.Create(TileColor, Count);
                }
                public override string ToString()
                {
                    return $"{this.TileColor}:{this.Count}";
                }
                public static List<TileCount> FromDictionary(Dictionary<TileColor, int> dictionary)
                {
                    List<TileCount> list = new();
                    foreach (KeyValuePair<TileColor, int> entry in dictionary)
                    {
                        list.Add(new TileCount() { TileColor = entry.Key, Count = entry.Value });
                    }
                    return list;
                }
            }
            public class TileProviderCounts
            {
                public List<TileCount> TileCounts { get; init; }
                public TileProvider Provider { get; init; }

                public ColoredValue<int> GetMaxColor(TileColor wildColor)
                {
                    ColoredValue<int> max = this.TileCounts[0].ToColoredValue();
                    if (max.GetTileColor() == wildColor)
                    {
                        // if the color is wild, then it only counts for 1.
                        max = ColoredValue<int>.Create(wildColor, Math.Min(max.GetValue(), 1));
                    }
                    foreach (TileCount tileCount in this.TileCounts)
                    {
                        if (tileCount.TileColor != wildColor)
                        {
                            if (max.GetTileColor() == wildColor)
                            {
                                max = tileCount.ToColoredValue();
                            }
                            else if (max.GetValue() < tileCount.Count)
                            {
                                max = tileCount.ToColoredValue();
                            }
                        }
                    }
                    return max;
                }

                public bool HasColor(TileColor color)
                {
                    return this.TileCounts.Any(count => count.TileColor == color && count.Count > 0);
                }
            }
        }
        public class TableController : MonoBehaviour
        {
            [SerializeField] private GameObject tablePrefab;

            private UnityEvent<OnTableTilesAddedPayload> onTilesAdded = new();
            private AzulEvent<OnTableTilesDrawnPayload> onTilesDrawn = new();
            private UnityEvent<OnTileHoverEnterPayload> onTilesHoverEnter = new();
            private UnityEvent<OnTileHoverExitPayload> onTilesHoverExit = new();
            private UnityEvent<OnTileSelectPayload> onTilesSelect = new();
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
                TableSelectableTileHolderController tableSelectableTileHolderController = this.GetTableSelectableTileHolderController();
                tableSelectableTileHolderController.AddOnTileHoverEnterListener(this.OnTilesHoverEnter);
                tableSelectableTileHolderController.AddOnTileHoverExitListener(this.OnTilesHoverExit);
                tableSelectableTileHolderController.AddOnTileSelectListener(this.OnTilesSelect);
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
                foreach (Factory factory in factories)
                {
                    factory.GetSelectableTileHolderController().AddOnTileHoverEnterListener(this.OnTilesHoverEnter);
                    factory.GetSelectableTileHolderController().AddOnTileHoverExitListener(this.OnTilesHoverExit);
                    factory.GetSelectableTileHolderController().AddOnTileSelectListener(this.OnTilesSelect);
                }
            }

            public void MoveOneTileToCenter(Tile oneTile)
            {
                this.table.AddToCenter(oneTile, true);
                this.onTilesAdded.Invoke(new OnTableTilesAddedPayload { Tiles = new() { oneTile } });
            }

            public void AddOnTilesAddedListener(UnityAction<OnTableTilesAddedPayload> listener)
            {
                this.onTilesAdded.AddListener(listener);
            }

            public void AddOnTilesDrawnListener(UnityAction<EventTracker<OnTableTilesDrawnPayload>> listener)
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
                    if (count > 0 && wildColor != desiredColor && factory.HasTileOfColor(wildColor))
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

            public Dictionary<TileColor, int> GetTableSupplyTileCounts()
            {
                return this.table.GetTileCounts();
            }

            public bool HasTableSupplyTileOfColor(TileColor tileColor)
            {
                return this.table.HasTileOfColor(tileColor);
            }

            public TileColor GetTableSupplyWithLowestCount(TileColor wildColor)
            {
                Dictionary<TileColor, int> tileCountsByColor = this.GetTableSupplyTileCounts();
                List<ColoredValue<int>> tileCounts = new();
                foreach (KeyValuePair<TileColor, int> kvp in tileCountsByColor)
                {
                    tileCounts.Add(ColoredValue<int>.Create(kvp.Key, kvp.Value));
                }
                ColoredValue<int>[] counts = tileCounts.FindAll(count => count.GetTileColor() != wildColor && count.GetValue() > 0).OrderBy(count => count.GetValue()).ToArray();
                if (counts.Length > 0)
                {
                    return counts[0].GetTileColor();
                }
                else
                {
                    return wildColor;
                }
            }

            public List<TileProviderCounts> GetTileCountsByProvider()
            {

                List<TileProviderCounts> tileProviderCounts = new();
                foreach (Factory factory in this.table.GetFactories())
                {
                    List<TileCount> factoryTileCounts = TileCount.FromDictionary(factory.GetTileCounts());
                    if (factoryTileCounts.Count > 0)
                    {
                        tileProviderCounts.Add(new TileProviderCounts()
                        {
                            Provider = factory,
                            TileCounts = TileCount.FromDictionary(factory.GetTileCounts())
                        });
                    }
                }
                List<TileCount> tableTileCounts = TileCount.FromDictionary(this.table.GetTileCounts());
                if (tableTileCounts.Count > 0)
                {
                    tileProviderCounts.Add(new TileProviderCounts()
                    {
                        Provider = this.table,
                        TileCounts = tableTileCounts
                    });
                }
                return tileProviderCounts;
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

            public bool HasHadesToken()
            {
                return this.table.HasHadesToken();
            }

            public bool IsCompletelyEmpty()
            {
                return this.table.IsEmpty() && this.table.GetFactories().All(factory => factory.IsEmpty());
            }

            private void OnTilesDiscarded(EventTracker<OnFactoryTilesDiscarded> payload)
            {
                this.StartCoroutine(this.OnTilesDiscardedCoroutine(payload.Payload.TilesDiscarded, payload.Done));
            }

            private IEnumerator OnTilesDiscardedCoroutine(List<Tile> tiles, Action Done)
            {
                yield return this.table.AddToCenter(tiles).WaitUntilCompleted();
                this.onTilesAdded.Invoke(new OnTableTilesAddedPayload { Tiles = tiles });
                Done.Invoke();
            }

            private void OnTilesDrawn(OnTableDrawTilesPayload payload)
            {
                this.StartCoroutine(this.OnTilesDrawnCoroutine(payload.PlayerNumber, payload.TilesDrawn));
            }

            private IEnumerator OnTilesDrawnCoroutine(int playerNumber, List<Tile> tilesDrawn)
            {
                bool includesHades = tilesDrawn.Find(tile => tile.Color == TileColor.ONE) != null;
                if (includesHades)
                {
                    yield return System.Instance.GetUIController().GetHadesTokenPanelUIController().Show(playerNumber, tilesDrawn.Count).WaitUntilCompleted();
                }
                yield return this.onTilesDrawn.Invoke(new OnTableTilesDrawnPayload
                {
                    Tiles = tilesDrawn,
                    IncludesOneTile = includesHades
                }).WaitUntilCompleted();
                yield return new WaitForSeconds(.5f);
                if (this.table.IsEmpty())
                {
                    this.onTableEmpty.Invoke();
                }
            }

            public void AddOnTilesHoverEnterListener(UnityAction<OnTileHoverEnterPayload> listener)
            {
                this.onTilesHoverEnter.AddListener(listener);
            }

            public void AddOnTilesHoverExitListener(UnityAction<OnTileHoverExitPayload> listener)
            {
                this.onTilesHoverExit.AddListener(listener);
            }

            public void AddOnTileSelectListener(UnityAction<OnTileSelectPayload> listener)
            {
                this.onTilesSelect.AddListener(listener);
            }

            private void OnTilesHoverEnter(OnTileHoverEnterPayload payload)
            {
                this.onTilesHoverEnter.Invoke(payload);
            }

            private void OnTilesHoverExit(OnTileHoverExitPayload payload)
            {
                this.onTilesHoverExit.Invoke(payload);
            }

            private void OnTilesSelect(OnTileSelectPayload payload)
            {
                this.onTilesSelect.Invoke(payload);
            }

        }
    }
}

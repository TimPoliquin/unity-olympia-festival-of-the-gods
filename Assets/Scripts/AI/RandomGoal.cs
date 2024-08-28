using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.AIEvents;
using Azul.Controller;
using Azul.Controller.TableUtilities;
using Azul.Model;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Azul
{
    namespace AI
    {
        public class RandomGoal : Goal
        {
            private int playerNumber { get; init; }
            private List<StarSpace> actionableSpaces;
            private UnityEvent<OnDrawFromFactoryPayload> onDrawFromFactory = new();
            private UnityEvent<OnDrawFromTablePayload> onDrawFromTable = new();
            private UnityEvent<OnScoreSpaceSelectedPayload> onScoreSpaceSelected = new();

            public void Acquire()
            {
                // get first factory tile provider with tiles
                TableController tableController = System.Instance.GetTableController();
                // find out what's available on the table
                List<TileCount> tileCounts = tableController.GetTileCounts();
                TileColor wildColor = System.Instance.GetRoundController().GetCurrentRound().GetWildColor();
                // pick a random color that's available
                TileColor tileColor = ListUtils.GetRandomElement(tileCounts.Select(tileCount => tileCount.TileColor).Except(new TileColor[] { wildColor, TileColor.ONE }).ToArray());
                UnityEngine.Debug.Log($"Random Goal: ${tileColor} / ${wildColor}");
                // find the factory that has the most tiles of that color
                Factory factory = tableController.GetFactoryWithMostTilesOfColor(tileColor);
                int tableTileCount = tableController.GetTableSupplyTileCount(tileColor);
                bool tableHasWild = tableController.HasTableSupplyTileOfColor(wildColor);
                bool tableHasOne = tableController.HasOneTile();
                int factoryTilesCount = 0;
                bool factoryHasWild = false;
                if (null != factory)
                {
                    factoryTilesCount = factory.GetTileCount(tileColor);
                    factoryHasWild = factory.HasTileOfColor(wildColor);
                }
                factoryTilesCount += factoryHasWild ? 1 : 0;
                tableTileCount += tableHasWild ? 1 : 0;
                UnityEngine.Debug.Log($"Random Goal: Factory Tiles: {factoryTilesCount} / {factoryHasWild}");
                UnityEngine.Debug.Log($"Random Goal: Table Tiles: {tableTileCount} / {tableHasWild}");
                if (null != factory && factoryTilesCount >= tableTileCount)
                {
                    // prefer drawing from a factory over the center of the table.
                    UnityEngine.Debug.Log("Drawing from factory");
                    this.DrawFromFactory(factory, tileColor, wildColor);
                }
                else
                {
                    UnityEngine.Debug.Log("Drawing from table");
                    this.DrawFromTable(tileColor, wildColor);
                }
            }

            public GoalAcquireFeasability CalculateAcquireFeasibility()
            {
                // do something random instead of something impossible, but prioritize everything else.
                return GoalAcquireFeasability.NOT_LIKELY;
            }

            public GoalStatus EvaluateCompletion()
            {
                return GoalStatus.IN_PROGRESS;
            }

            public bool IsComplete()
            {
                return false;
            }

            private void DrawFromFactory(Factory factory, TileColor desiredColor, TileColor wildColor)
            {
                this.onDrawFromFactory.Invoke(new OnDrawFromFactoryPayload
                {
                    Factory = factory,
                    DesiredColor = desiredColor,
                    WildColor = wildColor
                });
                this.onDrawFromFactory.RemoveAllListeners();
                this.onDrawFromTable.RemoveAllListeners();
            }

            private void DrawFromTable(TileColor desiredColor, TileColor wildColor)
            {
                this.onDrawFromTable.Invoke(new OnDrawFromTablePayload
                {
                    DesiredColor = desiredColor,
                    WildColor = wildColor
                });
                this.onDrawFromFactory.RemoveAllListeners();
                this.onDrawFromTable.RemoveAllListeners();
            }

            internal static RandomGoal Create(int playerNumber)
            {
                return new RandomGoal()
                {
                    playerNumber = playerNumber
                };
            }

            public void AddOnDrawFromTableListener(UnityAction<OnDrawFromTablePayload> listener)
            {
                this.onDrawFromTable.AddListener(listener);
            }

            public void AddOnDrawFromFactoryListener(UnityAction<OnDrawFromFactoryPayload> listener)
            {
                this.onDrawFromFactory.AddListener(listener);
            }

            public void Score()
            {
                StarSpace starSpace = ListUtils.GetRandomElement(this.actionableSpaces);
                UnityEngine.Debug.Log($"Scoring: {starSpace.GetOriginColor()}* {starSpace.GetValue()}");
                this.onScoreSpaceSelected.Invoke(new OnScoreSpaceSelectedPayload
                {
                    Selection = starSpace
                });
                this.onScoreSpaceSelected.RemoveAllListeners();
            }

            public int GetScoreProgress()
            {
                return 0;
            }

            public bool CanScore()
            {
                TileColor wildColor = System.Instance.GetRoundController().GetCurrentRound().GetWildColor();
                PlayerBoard playerBoard = System.Instance.GetPlayerBoardController().GetPlayerBoard(this.playerNumber);
                List<TileCount> tileCounts = playerBoard.GetTileCounts();
                this.actionableSpaces = new();
                int wildCount = this.GetWildTileCount(tileCounts, wildColor);
                foreach (TileCount tileCount in tileCounts)
                {
                    int count = tileCount.TileColor != wildColor ? tileCount.Count + wildCount : tileCount.Count;
                    UnityEngine.Debug.Log($"Finding open spaces: {tileCount.TileColor}/{tileCount.Count}/{count}");
                    if (tileCount.TileColor == TileColor.ONE)
                    {
                        continue;
                    }
                    List<StarSpace> openSpaces = playerBoard.GetOpenSpaces(tileCount.TileColor);
                    foreach (StarSpace starSpace in openSpaces)
                    {
                        if (starSpace.GetValue() == 1 && tileCount.Count >= 1)
                        {
                            this.actionableSpaces.Add(starSpace);
                            UnityEngine.Debug.Log($"Adding actionable space: {starSpace.GetOriginColor()}* {starSpace.GetValue()}");
                        }
                        else if (starSpace.GetValue() <= count)
                        {
                            UnityEngine.Debug.Log($"Adding actionable space: {starSpace.GetOriginColor()}* {starSpace.GetValue()}");
                            this.actionableSpaces.Add(starSpace);
                        }
                    }
                }
                this.actionableSpaces.Sort((a, b) => a.GetValue().CompareTo(b.GetValue()));
                return actionableSpaces.Count > 0;
            }

            private int GetWildTileCount(List<TileCount> tileCounts, TileColor wildColor)
            {
                TileCount wildTileCount = tileCounts.Where(tileCount => tileCount.TileColor == wildColor).DefaultIfEmpty(new TileCount
                {
                    TileColor = wildColor,
                    Count = 0
                }).FirstOrDefault();
                return wildTileCount.Count;

            }

            public void AddOnScoreSpaceSelectedListener(UnityAction<OnScoreSpaceSelectedPayload> listener)
            {
                this.onScoreSpaceSelected.AddListener(listener);
            }
        }
    }
}

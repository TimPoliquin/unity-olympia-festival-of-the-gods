using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.AIEvents;
using Azul.Controller;
using Azul.Controller.TableUtilities;
using Azul.Event;
using Azul.Model;
using Azul.PlayerBoardEvents;
using Azul.Util;
using UnityEngine;
using UnityEngine.Events;
using Utils;
using Random = UnityEngine.Random;

namespace Azul
{
    namespace AI
    {
        public class AcquireTileChoice
        {
            public TileProvider Provider { get; init; }
            public TileColor TileColor { get; init; }
            public int Count { get; init; }
        }

        public abstract class BaseGoal : Goal
        {
            protected int playerNumber { get; init; }
            private UnityEvent<OnDrawFromFactoryPayload> onDrawFromFactory = new();
            private UnityEvent<OnDrawFromTablePayload> onDrawFromTable = new();

            private RandomScoringSelection scoringSelection;

            public abstract void Acquire();

            protected bool ShouldRandomlyDrawFromTable()
            {
                TableController tableController = System.Instance.GetTableController();
                List<TileCount> tileCounts = TileCount.FromDictionary(tableController.GetTableSupplyTileCounts());
                if (tileCounts.Count > 0)
                {
                    bool drawFromTable = Random.Range(0f, 1f) > .8f;
                    if (drawFromTable)
                    {
                        UnityEngine.Debug.Log($"Player {playerNumber}: Randomly drawing from the table!");
                    }
                    return drawFromTable;
                }
                else
                {
                    return false;
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

            protected void DrawFromFactory(Factory factory, TileColor desiredColor, TileColor wildColor)
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

            protected void DrawFromTable(TileColor desiredColor, TileColor wildColor)
            {
                this.onDrawFromTable.Invoke(new OnDrawFromTablePayload
                {
                    DesiredColor = desiredColor,
                    WildColor = wildColor
                });
                this.onDrawFromFactory.RemoveAllListeners();
                this.onDrawFromTable.RemoveAllListeners();
            }

            public void AddOnDrawFromTableListener(UnityAction<OnDrawFromTablePayload> listener)
            {
                this.onDrawFromTable.AddListener(listener);
            }

            public void AddOnDrawFromFactoryListener(UnityAction<OnDrawFromFactoryPayload> listener)
            {
                this.onDrawFromFactory.AddListener(listener);
            }

            public CoroutineResult PrepareForScoring()
            {
                CoroutineResult result = CoroutineResult.Single();
                result.Start();
                if (null != this.scoringSelection)
                {
                    this.scoringSelection.EndScoring();
                }
                this.scoringSelection = new RandomScoringSelection();
                this.scoringSelection.Evaluate(this.playerNumber);
                result.Finish();
                return result;
            }

            public AltarSpace ChooseSpace()
            {
                return this.scoringSelection.ChooseSpace();
            }

            public TileColor ChooseWildColor(AltarSpace space)
            {
                return this.scoringSelection.ChooseWildColor(space);
            }

            public int GetScoreProgress()
            {
                return 0;
            }

            public bool CanScore()
            {
                return this.scoringSelection.CanScore() && !this.scoringSelection.WantsToWait();
            }

            public void AddOnScoreSpaceSelectedListener(UnityAction<EventTracker<OnScoreSpaceSelectedPayload>> listener)
            {
                this.scoringSelection.AddOnScoreSpaceSelectedListener(listener);
            }


            public void AddOnTileSelectedListener(UnityAction<OnGoalScoreTilesSelectedPayload> listener)
            {
                this.scoringSelection.AddOnTileSelectedListener(listener);
            }

            public void RemoveOnTileSelectedListener(UnityAction<OnGoalScoreTilesSelectedPayload> listener)
            {
                this.scoringSelection.RemoveOnTileSelectedListener(listener);
            }

            public void EndScoring()
            {

                this.scoringSelection.EndScoring();
                this.scoringSelection = null;
            }
        }

        public class RandomGoal : BaseGoal
        {
            internal static RandomGoal Create(int playerNumber)
            {
                return new RandomGoal()
                {
                    playerNumber = playerNumber
                };
            }
            public override void Acquire()
            {
                // TODO -- there is a bug that is causing this logic to choose tiles from a factory that has already been drawn.
                // get first factory tile provider with tiles
                TableController tableController = System.Instance.GetTableController();
                // find out what's available on the table
                List<TileCount> tileCounts = tableController.GetTileCounts();
                TileColor wildColor = System.Instance.GetRoundController().GetCurrentRound().GetWildColor();
                // pick a random color that's available
                TileColor tileColor = tileCounts.Count > 1 ? ListUtils.GetRandomElement(tileCounts.FindAll(tileCount => tileCount.Count > 0).Select(tileCount => tileCount.TileColor).Except(new TileColor[] { wildColor }).ToArray()) : tileCounts[0].TileColor;
                UnityEngine.Debug.Log($"Random Goal: ${tileColor} / ${wildColor}");
                // find the factory that has the most tiles of that color
                Factory factory = tableController.GetFactoryWithMostTilesOfColor(tileColor);
                bool tableHasOne = tableController.HasHadesToken();
                bool tableHasWild = tableController.HasTableSupplyTileOfColor(wildColor);
                int tableTileCount = tableController.GetTableSupplyTileCount(tileColor);
                int factoryTilesCount = 0;
                bool factoryHasWild = false;
                if (null != factory)
                {
                    factoryTilesCount = factory.GetTileCount(tileColor);
                    factoryHasWild = factory.HasTileOfColor(wildColor);
                }
                UnityEngine.Debug.Log($"Random Goal: Factory Tiles: {factoryTilesCount} / {factoryHasWild}");
                UnityEngine.Debug.Log($"Random Goal: Table Tiles: {tableTileCount} / {tableHasWild}");
                bool tableOneFudge = this.ShouldRandomlyDrawFromTable();
                if (null != factory && (factoryTilesCount >= tableTileCount || tableOneFudge))
                {
                    // prefer drawing from a factory over the center of the table.
                    UnityEngine.Debug.Log($"Drawing {tileColor} from factory");
                    this.DrawFromFactory(factory, tileColor, wildColor);
                }
                else
                {
                    UnityEngine.Debug.Log("Drawing from table");
                    if (tableHasOne)
                    {
                        this.DrawFromTable(tableController.GetTableSupplyWithLowestCount(wildColor), wildColor);
                    }
                    else
                    {
                        this.DrawFromTable(tileColor, wildColor);
                    }
                }
            }
        }

        public class AgressiveGoal : BaseGoal
        {
            internal static AgressiveGoal Create(int playerNumber)
            {
                return new AgressiveGoal()
                {
                    playerNumber = playerNumber
                };
            }
            public override void Acquire()
            {
                TileColor wildColor = System.Instance.GetRoundController().GetCurrentRound().GetWildColor();
                TableController tableController = System.Instance.GetTableController();
                if (tableController.HasHadesToken() && this.ShouldRandomlyDrawFromTable())
                {
                    this.DrawFromTable(tableController.GetTableSupplyWithLowestCount(wildColor), wildColor);
                    return;
                }
                List<TileProviderCounts> tileCounts = tableController.GetTileCountsByProvider();
                AcquireTileChoice choice = null;
                foreach (TileProviderCounts tileProviderCount in tileCounts)
                {
                    ColoredValue<int> localMax = tileProviderCount.GetMaxColor(wildColor);
                    bool updateChoice;
                    if (null == choice)
                    {
                        updateChoice = true;
                    }
                    else if (tileProviderCount.Provider.HasHadesToken())
                    {
                        updateChoice = false;
                    }
                    else if (choice.Count < localMax.GetValue())
                    {
                        updateChoice = true;
                    }
                    else if (choice.Count == localMax.GetValue() && tileProviderCount.HasColor(wildColor))
                    {
                        updateChoice = true;
                    }
                    else
                    {
                        updateChoice = false;
                    }
                    if (updateChoice)
                    {
                        choice = new AcquireTileChoice()
                        {
                            Provider = tileProviderCount.Provider,
                            TileColor = localMax.GetTileColor(),
                            Count = localMax.GetValue()
                        };
                    }
                }
                if (choice.Provider.IsTable())
                {
                    this.DrawFromTable(choice.TileColor, wildColor);
                }
                else
                {
                    this.DrawFromFactory((Factory)choice.Provider, choice.TileColor, wildColor);
                }
            }
        }
    }
}

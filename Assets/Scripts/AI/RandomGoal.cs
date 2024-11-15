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

            protected RandomScoringSelection scoringSelection;

            public abstract void Acquire();

            public abstract AltarSpace ChooseSpace();

            protected abstract float GetRandomChanceOfStupidity();

            protected bool ShouldRandomlyDrawFromTable()
            {
                TableController tableController = System.Instance.GetTableController();
                List<TileCount> tileCounts = TileCount.FromDictionary(tableController.GetTableSupplyTileCounts());
                if (tileCounts.Count > 0)
                {
                    bool drawFromTable = Random.Range(0f, 1f) < this.GetRandomChanceOfStupidity();
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
                if (null != this.scoringSelection)
                {
                    this.scoringSelection.EndScoring();
                    this.scoringSelection = null;
                }
            }

            protected void DrawFromProvider(TileProvider tileProvider, TileColor color, TileColor wildColor)
            {
                if (tileProvider.IsTable())
                {
                    this.DrawFromTable(color, wildColor);
                }
                else
                {
                    this.DrawFromFactory((Factory)tileProvider, color, wildColor);
                }
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
            protected override float GetRandomChanceOfStupidity()
            {
                return .4f;
            }
            public override AltarSpace ChooseSpace()
            {
                if (Random.Range(0f, 1f) < this.GetRandomChanceOfStupidity())
                {
                    return this.scoringSelection.ChooseRandomSpace();
                }
                else
                {
                    return this.scoringSelection.ChooseSpace();
                }
            }
            public override void Acquire()
            {
                TableController tableController = System.Instance.GetTableController();
                TileColor wildColor = System.Instance.GetRoundController().GetCurrentRound().GetWildColor();
                List<TileProviderCounts> allCounts = tableController.GetTileCountsByProvider();
                TileProviderCounts pickedCount;
                TileColor pickedColor;
                if (allCounts.Count == 1)
                {
                    pickedCount = allCounts[0];
                }
                else if (tableController.HasHadesToken() && this.ShouldRandomlyDrawFromTable())
                {
                    pickedCount = allCounts.Find(count => count.Provider.IsTable());
                }
                else
                {
                    pickedCount = ListUtils.GetRandomElement(allCounts);
                    UnityEngine.Debug.Log($"Player {this.playerNumber}: RandomGoal - picking random tile provider: {pickedCount.Provider}");
                }
                if (pickedCount.TileCounts.Count == 1)
                {
                    pickedColor = pickedCount.TileCounts[0].TileColor;
                    UnityEngine.Debug.Log($"Player {this.playerNumber}: RandomGoal - provider has one color: {pickedColor}");
                }
                else if (pickedCount.Provider.IsTable() && tableController.HasHadesToken())
                {
                    pickedColor = pickedCount.TileCounts.FindAll(tileCount => tileCount.TileColor != wildColor).OrderBy(tileCount => tileCount.Count).ToList()[0].TileColor;
                    UnityEngine.Debug.Log($"Player {this.playerNumber}: RandomGoal - table has hades token - finding lowest count {pickedColor}");
                }
                else
                {
                    pickedColor = ListUtils.GetRandomElement(pickedCount.TileCounts.FindAll(count => count.TileColor != wildColor)).TileColor;
                    UnityEngine.Debug.Log($"Player {this.playerNumber}: RandomGoal - picking a random color {pickedColor}");
                }
                this.DrawFromProvider(pickedCount.Provider, pickedColor, wildColor);
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
            protected override float GetRandomChanceOfStupidity()
            {
                return .1f;
            }
            public override AltarSpace ChooseSpace()
            {
                return this.scoringSelection.ChooseSpace();
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
                    else if (choice.TileColor == wildColor)
                    {
                        updateChoice = true;
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
                        if (tileProviderCount.Provider.HasHadesToken())
                        {
                            ColoredValue<int> min = tileProviderCount.GetMinColor(wildColor);
                            choice = new AcquireTileChoice()
                            {
                                Provider = tileProviderCount.Provider,
                                TileColor = min.GetTileColor(),
                                Count = min.GetValue(),
                            };
                        }
                        else
                        {
                            choice = new AcquireTileChoice()
                            {
                                Provider = tileProviderCount.Provider,
                                TileColor = localMax.GetTileColor(),
                                Count = localMax.GetValue()
                            };
                        }
                    }
                }
                this.DrawFromProvider(choice.Provider, choice.TileColor, wildColor);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.AIEvents;
using Azul.Controller;
using Azul.Controller.TableUtilities;
using Azul.Model;
using Azul.PlayerBoardEvents;
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
            private UnityEvent<OnDrawFromFactoryPayload> onDrawFromFactory = new();
            private UnityEvent<OnDrawFromTablePayload> onDrawFromTable = new();

            private RandomScoringSelection scoringSelection;

            public void Acquire()
            {
                // get first factory tile provider with tiles
                TableController tableController = System.Instance.GetTableController();
                // find out what's available on the table
                List<TileCount> tileCounts = tableController.GetTileCounts();
                TileColor wildColor = System.Instance.GetRoundController().GetCurrentRound().GetWildColor();
                // pick a random color that's available
                TileColor tileColor = tileCounts.Count > 1 ? ListUtils.GetRandomElement(tileCounts.Select(tileCount => tileCount.TileColor).Except(new TileColor[] { wildColor }).ToArray()) : tileCounts[0].TileColor;
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
                UnityEngine.Debug.Log($"Random Goal: Factory Tiles: {factoryTilesCount} / {factoryHasWild}");
                UnityEngine.Debug.Log($"Random Goal: Table Tiles: {tableTileCount} / {tableHasWild}");
                if (null != factory && factoryTilesCount >= tableTileCount)
                {
                    // prefer drawing from a factory over the center of the table.
                    UnityEngine.Debug.Log($"Drawing {tileColor} from factory");
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

            public void PrepareForScoring()
            {
                if (null != this.scoringSelection)
                {
                    this.scoringSelection.EndScoring();
                }
                this.scoringSelection = new RandomScoringSelection();
                this.scoringSelection.Evaluate(this.playerNumber);
            }

            public void Score()
            {
                this.scoringSelection.Score();
            }

            public int GetScoreProgress()
            {
                return 0;
            }

            public bool CanScore()
            {
                return this.scoringSelection.CanScore();
            }

            public void AddOnScoreSpaceSelectedListener(UnityAction<OnScoreSpaceSelectedPayload> listener)
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
    }
}

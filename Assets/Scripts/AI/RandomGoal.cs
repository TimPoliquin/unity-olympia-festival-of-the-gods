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
            private UnityEvent<OnDrawFromFactoryPayload> onDrawFromFactory = new();
            private UnityEvent<OnDrawFromTablePayload> onDrawFromTable = new();

            public void Act()
            {
                // TODO - draw a random tile
                // get first factory tile provider with tiles
                TableController tableController = System.Instance.GetTableController();
                List<TileCount> tileCounts = tableController.GetTileCounts();
                TileColor wildColor = System.Instance.GetRoundController().GetCurrentRound().GetWildColor();
                TileColor tileColor = ListUtils.GetRandomElement(tileCounts.Select(tileCount => tileCount.TileColor).Except(new TileColor[] { wildColor, TileColor.ONE }).ToArray());
                UnityEngine.Debug.Log($"Random Goal: ${tileColor} / ${wildColor}");
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
                    UnityEngine.Debug.Log("Drawing from factory");
                    this.DrawFromFactory(factory, tileColor, wildColor);
                }
                else
                {
                    UnityEngine.Debug.Log("Drawing from table");
                    this.DrawFromTable(tileColor, wildColor);
                }
            }

            public GoalFeasability CalculateFeasibility()
            {
                // do something random instead of something impossible, but prioritize everything else.
                return GoalFeasability.NOT_LIKELY;
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

            internal static RandomGoal Create()
            {
                return new RandomGoal();
            }

            public void AddOnDrawFromTableListener(UnityAction<OnDrawFromTablePayload> listener)
            {
                this.onDrawFromTable.AddListener(listener);
            }

            public void AddOnDrawFromFactoryListener(UnityAction<OnDrawFromFactoryPayload> listener)
            {
                this.onDrawFromFactory.AddListener(listener);
            }
        }
    }
}

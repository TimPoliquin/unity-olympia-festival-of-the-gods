using System.Collections;
using System.Collections.Generic;
using Azul.Controller;
using Azul.Model;
using UnityEditor.Rendering;
using UnityEngine;

namespace Azul
{
    namespace AI
    {
        /// <summary>
        /// An AI strategy for acquiring tiles. This strategy is Goal-based - the
        /// player will attempt to accomplish N number of goals (based on difficulty?).
        /// Goals are high-level, such as "Complete the Orange Star" or "Fill all 1 spaces", or low-level,
        /// such as "Fill the Orange one space".
        /// 
        /// At the start of each round, the strategy will order goals by their feasibility to be completed in the present round.
        /// At the start of each turn, the strategy will re-evaluate feasibility (marking new goals infeasible as they become so).
        /// The top feasible goal will be executed on.
        /// </summary>
        public class AcquireStrategy : MonoBehaviour
        {
            [SerializeField] private List<Goal> goals = new();

            public void CreateGoals()
            {
                this.goals.Add(GoalUtils.CreateRandomGoal());
            }

            public void EvaluateGoalsForRound()
            {
                this.goals.RemoveAll(goal => goal.EvaluateCompletion() == GoalStatus.COMPLETE);
                this.goals = GoalUtils.SortByFeasibility(this.goals);
            }

            public void Act()
            {
                Goal currentGoal = this.GetCurrentGoal();
                currentGoal.AddOnDrawFromFactoryListener(payload =>
                {
                    StartCoroutine(this.DrawFromFactory(payload.Factory, payload.DesiredColor, payload.WildColor));
                });
                currentGoal.AddOnDrawFromTableListener(payload =>
                {
                    this.StartCoroutine(this.DrawFromTable(payload.DesiredColor, payload.WildColor));
                });
                this.GetCurrentGoal().Act();
            }

            private Goal GetCurrentGoal()
            {
                if (this.goals.Count > 0)
                {
                    return this.goals[0];
                }
                else
                {
                    return null;
                }
            }

            private IEnumerator DrawFromFactory(Factory factory, TileColor desiredColor, TileColor wildColor)
            {
                // highlight the tiles
                FactorySelectableTileHolderController tileHolder = factory.GetComponent<FactorySelectableTileHolderController>();
                yield return new WaitForSeconds(.5f);
                tileHolder.HoverTiles(desiredColor);
                yield return new WaitForSeconds(1.0f);
                // draw the tiles
                tileHolder.SelectHoveredTiles();
            }

            private IEnumerator DrawFromTable(TileColor desiredColor, TileColor wildColor)
            {
                TableSelectableTileHolderController tileHolder = System.Instance.GetTableController().GetTableSelectableTileHolderController();
                yield return new WaitForSeconds(.5f);
                tileHolder.HoverTiles(desiredColor);
                yield return new WaitForSeconds(1.0f);
                // draw the tiles
                tileHolder.SelectHoveredTiles();
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.AIEvents;
using Azul.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace Azul
{
    namespace AI
    {
        public class ScoringStrategy : MonoBehaviour
        {
            [SerializeField] private List<Goal> goals;

            public void EvaluateGoals(List<Goal> goals)
            {
                // copy the goals
                UnityEngine.Debug.Log($"Starting Goals: {goals.Count}");
                this.goals = goals.Where(goal =>
                {
                    if (goal.EvaluateCompletion() == GoalStatus.COMPLETE)
                    {
                        UnityEngine.Debug.Log($"Removing completed goal");
                        return false;
                    }
                    return goal.CanScore();
                }).OrderBy(goal => goal.GetScoreProgress()).ToList();
                UnityEngine.Debug.Log($"Goals: {this.goals.Count}");
            }

            public void Score()
            {
                if (this.goals.Count > 0)
                {
                    Goal goal = this.goals[0];
                    goal.AddOnScoreSpaceSelectedListener(this.OnScoreSpaceSelected);
                    goal.Score();
                }
            }

            private void OnScoreSpaceSelected(OnScoreSpaceSelectedPayload payload)
            {
                StartCoroutine(this.SelectStarSpace(payload.Selection));
            }

            private IEnumerator SelectStarSpace(StarSpace starSpace)
            {
                UnityEngine.Debug.Log($"Trying to select space {starSpace.GetOriginColor()} * {starSpace.GetValue()}");
                starSpace.Select();
                yield return new WaitForSeconds(1.0f);
            }
        }
    }
}

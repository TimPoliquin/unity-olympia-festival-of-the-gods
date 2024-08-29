using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.AIEvents;
using Azul.Controller;
using Azul.Model;
using Azul.PlayerBoardEvents;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Azul
{
    namespace AIEvents
    {
        public struct OnAIScorePayload
        {

        }
    }
    namespace AI
    {
        public class ScoringStrategy : MonoBehaviour
        {
            [SerializeField] private List<Goal> goals;
            private UnityEvent<OnAIScorePayload> onScoreComplete = new();


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

            public bool CanScore()
            {
                return this.goals.Any(goal => goal.CanScore());
            }

            public void Score()
            {
                if (this.goals.Count > 0)
                {
                    Goal goal = this.goals[0];
                    goal.AddOnScoreSpaceSelectedListener(this.OnScoreSpaceSelected);
                    goal.AddOnTileSelectedListener(this.OnGoalScoreTilesSelected);
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

            private void OnGoalScoreTilesSelected(OnGoalScoreTilesSelectedPayload payload)
            {
                this.StartCoroutine(this.ChooseTilesToFillScoreSpace(
                    playerBoard: payload.PlayerBoard,
                    tileColor: payload.TileColor,
                    wildColor: System.Instance.GetRoundController().GetCurrentRound().GetWildColor(),
                    value: payload.Value,
                    OnConfirm: payload.OnConfirm
                ));
            }

            private IEnumerator ChooseTilesToFillScoreSpace(PlayerBoard playerBoard, TileColor tileColor, TileColor wildColor, int value, UnityAction<OnPlayerBoardScoreTileSelectionConfirmPayload> OnConfirm)
            {
                yield return new WaitForSeconds(1.0f);
                Dictionary<TileColor, int> selectedTiles = new();
                int chosenColorTileCount = playerBoard.GetTileCount(tileColor);
                if (chosenColorTileCount < value && tileColor != wildColor)
                {
                    selectedTiles[tileColor] = chosenColorTileCount;
                    selectedTiles[wildColor] = value - chosenColorTileCount;
                }
                else
                {
                    selectedTiles[tileColor] = value;
                }
                OnConfirm(new OnPlayerBoardScoreTileSelectionConfirmPayload
                {
                    TilesSelected = selectedTiles,
                    Color = tileColor
                });
                this.onScoreComplete.Invoke(new OnAIScorePayload());
            }

            public void AddOnScoreCompleteListener(UnityAction<OnAIScorePayload> listener)
            {
                this.onScoreComplete.AddListener(listener);
            }
        }
    }
}

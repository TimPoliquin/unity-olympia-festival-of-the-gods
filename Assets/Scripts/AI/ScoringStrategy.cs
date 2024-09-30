using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.AIEvents;
using Azul.Controller;
using Azul.Controller.TableUtilities;
using Azul.Model;
using Azul.PlayerBoardEvents;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Utils;

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
            [SerializeField] private List<Goal> actionableGoals;
            private UnityEvent<OnAIScorePayload> onScoreComplete = new();


            public void EvaluateGoals(List<Goal> goals)
            {
                // copy the goals
                UnityEngine.Debug.Log($"Starting Goals: {goals.Count}");
                this.actionableGoals = goals.Where(goal =>
                {
                    goal.PrepareForScoring();
                    if (goal.EvaluateCompletion() == GoalStatus.COMPLETE)
                    {
                        UnityEngine.Debug.Log($"Removing completed goal");
                        return false;
                    }
                    return goal.CanScore();
                }).OrderBy(goal => goal.GetScoreProgress()).ToList();
                UnityEngine.Debug.Log($"Goals: {this.actionableGoals.Count}");
            }

            public bool CanScore()
            {
                return this.actionableGoals.Any(goal => goal.CanScore());
            }

            public void Score()
            {
                if (this.actionableGoals.Count > 0)
                {
                    Goal goal = this.actionableGoals[0];
                    goal.AddOnScoreSpaceSelectedListener(this.OnScoreSpaceSelected);
                    goal.AddOnTileSelectedListener(this.OnGoalScoreTilesSelected);
                    goal.Score();
                }
            }

            private void OnScoreSpaceSelected(OnScoreSpaceSelectedPayload payload)
            {
                StartCoroutine(this.SelectStarSpace(payload.Selection));
            }

            private IEnumerator SelectStarSpace(AltarSpace starSpace)
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
                yield return new WaitUntil(() => !System.Instance.GetTileAnimationController().IsAnimating());
                yield return null;
                this.onScoreComplete.Invoke(new OnAIScorePayload());
            }

            public void AddOnScoreCompleteListener(UnityAction<OnAIScorePayload> listener)
            {
                this.onScoreComplete.AddListener(listener);
            }

            public TileColor ChooseReward()
            {
                // TODO - we really need to figure out something good here, but for now,
                // let's just pick the wild color to make things easy.
                return System.Instance.GetRoundController().GetCurrentRound().GetWildColor();
            }

            public Dictionary<TileColor, int> HandleOverflowDiscard(int playerNumber, int tilesAllowed)
            {
                RoundController roundController = System.Instance.GetRoundController();
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                PlayerBoard playerBoard = playerBoardController.GetPlayerBoard(playerNumber);
                List<TileCount> tileCounts = playerBoard.GetTileCounts();
                Dictionary<TileColor, int> toDiscard = new();
                // add all tiles to the discard set
                foreach (TileCount tileCount in tileCounts)
                {
                    toDiscard[tileCount.TileColor] = tileCount.Count;
                }
                if (tilesAllowed == 0 || roundController.IsLastRound())
                {
                    // just move 'em all over!
                    return toDiscard;
                }
                else
                {
                    int tilesKept = 0;
                    // see if there's anything we SHOULD keep
                    TileColor nextWildColor = roundController.GetNextWild();
                    if (toDiscard.ContainsKey(nextWildColor))
                    {
                        int numNextWild = toDiscard[nextWildColor];
                        if (numNextWild >= tilesAllowed)
                        {
                            toDiscard[nextWildColor] -= tilesAllowed;
                            tilesKept = tilesAllowed;
                        }
                    }
                    while (tilesKept < tilesAllowed)
                    {
                        // TODO - ignore any tiles for completed colors (star and wild space)

                        // pick a random tile to kep?
                        TileColor tileColor = ListUtils.GetRandomElement(toDiscard.Keys.ToList());
                        if (toDiscard[tileColor] > 0)
                        {
                            toDiscard[tileColor]--;
                            tilesKept++;
                        }
                    }
                }
                return toDiscard;
            }
        }
    }
}

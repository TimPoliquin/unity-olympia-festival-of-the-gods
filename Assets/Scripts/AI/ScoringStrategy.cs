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
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Utils;

namespace Azul
{
    namespace AI
    {
        public class ScoringStrategy : MonoBehaviour
        {
            [SerializeField] private List<Goal> actionableGoals;

            public CoroutineResult EvaluateGoals(List<Goal> goals)
            {
                CoroutineResult result = CoroutineResult.Single();
                this.StartCoroutine(this.EvaluateGoalsCoroutine(goals, result));
                // copy the goals
                return result;
            }

            private IEnumerator EvaluateGoalsCoroutine(List<Goal> goals, CoroutineResult result)
            {
                result.Start();
                UnityEngine.Debug.Log($"Starting Goals: {goals.Count}");
                List<Goal> actionableGoals = new();
                foreach (Goal goal in goals)
                {
                    yield return goal.PrepareForScoring().WaitUntilCompleted();
                    bool actionable;
                    if (goal.EvaluateCompletion() == GoalStatus.COMPLETE)
                    {
                        UnityEngine.Debug.Log($"Removing completed goal");
                        actionable = false;
                    }
                    actionable = goal.CanScore();
                    if (actionable)
                    {
                        actionableGoals.Add(goal);
                    }
                }
                this.actionableGoals = actionableGoals.OrderBy(goal => goal.GetScoreProgress()).ToList();
                UnityEngine.Debug.Log($"Goals: {this.actionableGoals.Count}");
                result.Finish();
            }

            public bool CanScore()
            {
                return this.actionableGoals.Any(goal => goal.CanScore());
            }

            public CoroutineResult Score(int playerNumber)
            {
                CoroutineResult result = CoroutineResult.Single();
                if (this.actionableGoals.Count > 0)
                {
                    this.StartCoroutine(this.ScoreCoroutine(playerNumber, this.actionableGoals[0], result));
                }
                else
                {
                    result.Finish();
                }
                return result;
            }

            private IEnumerator ScoreCoroutine(int playerNumber, Goal goal, CoroutineResult result)
            {
                result.Start();
                AltarSpace space = goal.ChooseSpace();
                space.Select();
                yield return new WaitForSeconds(1.0f);
                TileColor selectedColor = space.IsWild() ? goal.ChooseWildColor(space) : space.GetOriginColor();
                Dictionary<TileColor, int> selectedTiles = this.ChooseTilesToFillScoreSpace(
                    System.Instance.GetPlayerBoardController().GetPlayerBoard(playerNumber),
                    selectedColor,
                    wildColor: System.Instance.GetRoundController().GetCurrentRound().GetWildColor(),
                    value: space.GetValue()
                );
                string joinChar = ",";
                UnityEngine.Debug.Log($"Scoring Strategy: Player {playerNumber} filling space: {space.GetOriginColor()}{space.GetValue()}");
                UnityEngine.Debug.Log($"Scoring Strategy: Filling with tiles: {String.Join(joinChar, TileCount.FromDictionary(selectedTiles).Select(count => count.ToString()).ToList())}");
                yield return System.Instance.GetPlayerBoardController().PlaceTiles(playerNumber, space, selectedColor, selectedTiles).WaitUntilCompleted();
                this.actionableGoals.ForEach(goal => goal.EndScoring());
                result.Finish();
            }


            private Dictionary<TileColor, int> ChooseTilesToFillScoreSpace(PlayerBoard playerBoard, TileColor tileColor, TileColor wildColor, int value)
            {
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
                return selectedTiles;
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

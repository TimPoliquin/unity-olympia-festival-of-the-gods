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
using Utils;

namespace Azul
{
    namespace AI
    {
        public class ActionableSpace
        {
            public AltarSpace Space { get; init; }
            public int Score { get; init; }
        }
        public class RandomScoringSelection
        {
            private List<ActionableSpace> actionableSpaces;
            private List<TileCount> tileCounts;
            private int wildTileCount;
            private TileColor wildColor;
            private List<TileColor> usedWildColors;


            private AzulEvent<OnScoreSpaceSelectedPayload> onScoreSpaceSelected = new();
            private UnityEvent<OnGoalScoreTilesSelectedPayload> onScoreTileSelection = new();

            public void Evaluate(int playerNumber)
            {
                if (null != this.actionableSpaces)
                {
                    UnityEngine.Debug.Log($"Already evaluated scoring: {playerNumber}");
                    return;
                }
                ScoreBoardController scoreBoardController = System.Instance.GetScoreBoardController();
                PlayerBoard playerBoard = System.Instance.GetPlayerBoardController().GetPlayerBoard(playerNumber);
                this.wildColor = System.Instance.GetRoundController().GetCurrentRound().GetWildColor();
                this.tileCounts = playerBoard.GetTileCounts();
                this.wildTileCount = this.GetWildTileCount(tileCounts, wildColor);
                this.actionableSpaces = new();
                this.usedWildColors = playerBoard.GetWildTileColors();
                List<AltarSpace> openWildSpaces = playerBoard.GetWildOpenSpaces();
                foreach (TileCount tileCount in tileCounts)
                {
                    if (tileCount.Count == 0)
                    {
                        continue;
                    }
                    int count = tileCount.TileColor != wildColor ? tileCount.Count + wildTileCount : tileCount.Count;
                    UnityEngine.Debug.Log($"Finding open spaces: {tileCount.TileColor}/{tileCount.Count}/{count}");
                    if (tileCount.TileColor == TileColor.ONE)
                    {
                        continue;
                    }
                    List<AltarSpace> openSpaces = playerBoard.GetOpenSpaces(tileCount.TileColor);
                    if (!usedWildColors.Contains(tileCount.TileColor))
                    {
                        openSpaces.AddRange(openWildSpaces);
                    }
                    foreach (AltarSpace starSpace in openSpaces)
                    {

                        if (starSpace.GetValue() == 1 && tileCount.Count >= 1)
                        {
                            this.actionableSpaces.Add(new ActionableSpace()
                            {
                                Space = starSpace,
                                Score = scoreBoardController.CalculatePointsForTilePlacement(playerBoard.GetAltar(starSpace.GetOriginColor()), starSpace.GetValue()),
                            });
                            UnityEngine.Debug.Log($"Adding actionable space: {starSpace.GetOriginColor()}* {starSpace.GetValue()}");
                        }
                        else if (starSpace.GetValue() <= count)
                        {
                            UnityEngine.Debug.Log($"Adding actionable space: {starSpace.GetOriginColor()}* {starSpace.GetValue()}");
                            this.actionableSpaces.Add(new ActionableSpace()
                            {
                                Space = starSpace,
                                Score = scoreBoardController.CalculatePointsForTilePlacement(playerBoard.GetAltar(starSpace.GetOriginColor()), starSpace.GetValue()),
                            });
                        }
                    }
                }
                this.actionableSpaces = this.actionableSpaces.Distinct().OrderByDescending(space => space.Score).ToList();
            }

            public bool CanScore()
            {
                return this.actionableSpaces.Count > 0;
            }

            public bool WantsToWait()
            {
                if (System.Instance.GetRoundController().IsLastRound())
                {
                    return false;
                }
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                if (this.tileCounts.Sum(count => count.Count) <= playerBoardController.GetAllowedOverflow())
                {
                    return this.actionableSpaces[0].Score == 1;
                }
                return false;
            }

            public AltarSpace ChooseSpace()
            {
                AltarSpace chosenSpace;
                int highestScore = this.actionableSpaces[0].Score;
                if (highestScore == 1)
                {
                    chosenSpace = this.ChooseRandomSpace();
                }
                else
                {
                    chosenSpace = this.actionableSpaces.FindAll(space => space.Score == highestScore).OrderByDescending((space) => space.Space.GetValue()).ToList()[0].Space;
                }
                return chosenSpace;
            }

            public AltarSpace ChooseRandomSpace()
            {
                return ListUtils.GetRandomElement(this.actionableSpaces).Space;
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

            public TileColor ChooseWildColor(AltarSpace altarSpace)
            {
                List<TileColor> eligibleColors = this.tileCounts.Where(tileCount =>
                {
                    if (this.usedWildColors.Contains(tileCount.TileColor))
                    {
                        return false;
                    }
                    else if (altarSpace.GetValue() > tileCount.Count)
                    {
                        // check wild
                        if (altarSpace.GetValue() > 1 && tileCount.TileColor != this.wildColor)
                        {
                            return tileCount.Count + this.wildTileCount >= altarSpace.GetValue();
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }).Select(tileCount => tileCount.TileColor).Distinct().ToList();
                UnityEngine.Debug.Log($"Eligible color count: {eligibleColors.Count}");
                return ListUtils.GetRandomElement(eligibleColors);
            }

            public void AddOnScoreSpaceSelectedListener(UnityAction<EventTracker<OnScoreSpaceSelectedPayload>> listener)
            {
                this.onScoreSpaceSelected.AddListener(listener);
            }


            public void AddOnTileSelectedListener(UnityAction<OnGoalScoreTilesSelectedPayload> listener)
            {
                this.onScoreTileSelection.AddListener(listener);
            }

            public void RemoveOnTileSelectedListener(UnityAction<OnGoalScoreTilesSelectedPayload> listener)
            {
                this.onScoreTileSelection.RemoveListener(listener);
            }

            public void EndScoring()
            {
                UnityEngine.Debug.Log($"Ending scoring of Random Scoring Selection");
                this.onScoreSpaceSelected.RemoveAllListeners();
                this.onScoreTileSelection.RemoveAllListeners();
                this.actionableSpaces = null;
                this.tileCounts = null;
                this.usedWildColors = null;
            }
        }
    }
}

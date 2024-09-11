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
using Utils;

namespace Azul
{
    namespace AI
    {
        public class RandomScoringSelection
        {
            private List<AltarSpace> actionableSpaces;
            private AltarSpace chosenSpace;
            private List<TileCount> tileCounts;
            private int wildTileCount;
            private TileColor wildColor;
            private List<TileColor> usedWildColors;


            private UnityEvent<OnScoreSpaceSelectedPayload> onScoreSpaceSelected = new();
            private UnityEvent<OnGoalScoreTilesSelectedPayload> onScoreTileSelection = new();

            public RandomScoringSelection()
            {
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                playerBoardController.AddOnPlayerBoardScoreSpaceSelectionListener(this.OnScoreSpaceSelected);
                playerBoardController.AddOnPlayerBoardWildScoreSpaceSelectionListener(this.OnWildScoreSpaceSelected);
            }

            public void Evaluate(int playerNumber)
            {
                if (null != this.actionableSpaces)
                {
                    UnityEngine.Debug.Log($"Already evaluated scoring: {playerNumber}");
                    return;
                }
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
                            this.actionableSpaces.Add(starSpace);
                            UnityEngine.Debug.Log($"Adding actionable space: {starSpace.GetOriginColor()}* {starSpace.GetValue()}");
                        }
                        else if (starSpace.GetValue() <= count)
                        {
                            UnityEngine.Debug.Log($"Adding actionable space: {starSpace.GetOriginColor()}* {starSpace.GetValue()}");
                            this.actionableSpaces.Add(starSpace);
                        }
                    }
                }
                this.actionableSpaces = this.actionableSpaces.Distinct().OrderBy(space => space.GetValue()).ToList();
            }

            public bool CanScore()
            {
                return this.actionableSpaces.Count > 0;
            }

            public void Score()
            {
                this.chosenSpace = ListUtils.GetRandomElement(this.actionableSpaces);
                UnityEngine.Debug.Log($"Scoring: {chosenSpace.GetOriginColor()}* {chosenSpace.GetValue()}");
                this.onScoreSpaceSelected.Invoke(new OnScoreSpaceSelectedPayload
                {
                    Selection = this.chosenSpace
                });
                this.onScoreSpaceSelected.RemoveAllListeners();
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

            private void OnScoreSpaceSelected(OnPlayerBoardScoreSpaceSelectionPayload payload)
            {
                System.Instance.GetPlayerBoardController().RemoveOnPlayerBoardScoreSpaceSelectionListener(this.OnScoreSpaceSelected);
                TileColor chosenColor = this.chosenSpace.GetOriginColor();
                this.chosenSpace = null;
                // dispatch event
                this.onScoreTileSelection.Invoke(new OnGoalScoreTilesSelectedPayload
                {
                    PlayerBoard = payload.PlayerBoard,
                    TileColor = chosenColor,
                    Value = payload.Value,
                    OnConfirm = payload.OnConfirm,
                });
                this.onScoreTileSelection.RemoveAllListeners();

            }

            private void OnWildScoreSpaceSelected(OnPlayerBoardWildScoreSpaceSelectionPayload payload)
            {
                this.chosenSpace = null;
                System.Instance.GetPlayerBoardController().RemoveOnPlayerBoardWildScoreSpaceSelectionListener(this.OnWildScoreSpaceSelected);
                List<TileColor> eligibleColors = this.tileCounts.Where(tileCount =>
                {
                    if (this.usedWildColors.Contains(tileCount.TileColor))
                    {
                        return false;
                    }
                    else if (payload.Value > tileCount.Count)
                    {
                        // check wild
                        if (payload.Value > 1 && tileCount.TileColor != this.wildColor)
                        {
                            return tileCount.Count + this.wildTileCount >= payload.Value;
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
                TileColor chosenColor = ListUtils.GetRandomElement(eligibleColors);
                // dispatch event
                this.onScoreTileSelection.Invoke(new OnGoalScoreTilesSelectedPayload
                {
                    PlayerBoard = payload.PlayerBoard,
                    TileColor = chosenColor,
                    Value = payload.Value,
                    OnConfirm = payload.OnConfirm,
                });
                this.onScoreTileSelection.RemoveAllListeners();
            }

            public void AddOnScoreSpaceSelectedListener(UnityAction<OnScoreSpaceSelectedPayload> listener)
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
                this.chosenSpace = null;
                this.tileCounts = null;
                this.usedWildColors = null;
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                playerBoardController.RemoveOnPlayerBoardScoreSpaceSelectionListener(this.OnScoreSpaceSelected);
                playerBoardController.RemoveOnPlayerBoardWildScoreSpaceSelectionListener(this.OnWildScoreSpaceSelected);
            }
        }
    }
}

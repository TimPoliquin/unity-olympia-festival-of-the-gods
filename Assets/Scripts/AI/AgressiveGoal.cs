using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Controller;
using Azul.Controller.TableUtilities;
using Azul.Model;
using Azul.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace AI
    {
        public struct AltarSpaceScore
        {
            public Altar Altar { get; init; }
            public AltarSpace AltarSpace { get; init; }
            public int Score { get; init; }
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
                return .05f;
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
                Dictionary<TileColor, int> priorities = this.GetTileColorPreferences();
                AcquireTileChoice choice = null;
                if (tileCounts.Count == 1)
                {
                    ColoredValue<int> localMax = tileCounts[0].GetMaxColor(wildColor);
                    choice = new AcquireTileChoice()
                    {
                        Provider = tileCounts[0].Provider,
                        TileColor = localMax.GetTileColor(),
                        Count = localMax.GetValue(),
                        Priority = priorities[localMax.GetTileColor()]
                    };
                }
                else
                {
                    foreach (TileProviderCounts tileProviderCount in tileCounts)
                    {
                        List<ColoredValue<int>> localMaxes = TileColorUtils.GetTileColors()
                                                        .Where(tileColor => tileColor != wildColor)
                                                        .Select(color =>
                                                        {
                                                            int value = tileProviderCount.GetTileCount(color);
                                                            if (value > 0 && tileProviderCount.HasColor(wildColor))
                                                            {
                                                                value += 1;
                                                            }
                                                            return ColoredValue<int>.Create(color, value * priorities[color]);
                                                        })
                                                        .Where(count => count.GetValue() > 0)
                                                        .OrderByDescending(count => count.GetValue())
                                                        .ToList();
                        ColoredValue<int> localMax;
                        bool updateChoice;
                        if (localMaxes.Count == 0)
                        {
                            updateChoice = false;
                            continue;
                        }
                        else
                        {
                            localMax = localMaxes[0];
                        }
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
                            UnityEngine.Debug.Log($"AIPlayerController: Making choice: {localMax.GetTileColor()}{localMax.GetValue()} : P{priorities[localMax.GetTileColor()]}");
                            choice = new AcquireTileChoice()
                            {
                                Provider = tileProviderCount.Provider,
                                TileColor = localMax.GetTileColor(),
                                Count = localMax.GetValue(),
                                Priority = priorities[localMax.GetTileColor()]
                            };
                        }
                    }
                }
                this.DrawFromProvider(choice.Provider, choice.TileColor, wildColor);
            }
            private Dictionary<TileColor, int> GetTileColorPreferences()
            {
                TableController tableController = System.Instance.GetTableController();
                AIController aIController = System.Instance.GetAIController();
                Dictionary<TileColor, ColoredValue<int>> preferencesByColor = new();
                PlayerBoard playerBoard = System.Instance.GetPlayerBoardController().GetPlayerBoard(this.playerNumber);
                Dictionary<TileColor, int> boardTileCounts = TileUtils.MapToDictionary(playerBoard.GetTileCounts());
                Dictionary<TileColor, int> tableTileCounts = TileUtils.MapToDictionary(tableController.GetTileCounts());
                TileColor wildColor = System.Instance.GetRoundController().GetCurrentWild();
                Altar wildAltar = playerBoard.GetAltar(TileColor.WILD);
                List<AltarSpace> openWildSpaces = wildAltar.GetOpenSpaces();
                List<TileColor> availableWildColors = TileColorUtils.GetTileColors().Where(color => !wildAltar.GetFilledSpaces().Any(space => space.GetEffectiveColor() == color)).ToList();
                bool isLastRound = System.Instance.GetRoundController().IsLastRound();
                Dictionary<TileColor, int> priorities = new();
                // check altar progress
                foreach (TileColor color in TileColorUtils.GetTileColors())
                {
                    if (color == wildColor)
                    {
                        priorities[color] = 0;
                        continue;
                    }
                    int priority = 0;
                    Altar altar = playerBoard.GetAltar(color);
                    List<AltarSpaceScore> openSpaces = altar.GetOpenSpaces().Select(space => new AltarSpaceScore
                    {
                        AltarSpace = space,
                        Score = this.GetAltarSpaceScoreValue(altar, space)
                    }).OrderByDescending(spaceScore => spaceScore.Score).ToList();
                    if (openSpaces.Count == 0 && !availableWildColors.Contains(color))
                    {
                        priority = -1;
                    }
                    else
                    {
                        int acquiredTokenCount = boardTileCounts[color];
                        int wildTokenCount = boardTileCounts[color];
                        foreach (AltarSpaceScore space in openSpaces)
                        {
                            if (acquiredTokenCount >= space.AltarSpace.GetValue())
                            {
                                // player already has enough tokens to fill
                                priority += 0;
                                acquiredTokenCount -= space.AltarSpace.GetValue();
                            }
                            else if (acquiredTokenCount > 0 && wildTokenCount + acquiredTokenCount >= space.AltarSpace.GetValue())
                            {
                                // player has enough tokens to fill if they use wild tokens
                                priority += (int)Math.Pow(space.Score, 2);
                                wildTokenCount -= (space.AltarSpace.GetValue() - acquiredTokenCount);
                                acquiredTokenCount = 0;
                            }
                            else if (tableTileCounts[color] + acquiredTokenCount + wildTokenCount >= space.AltarSpace.GetValue())
                            {
                                // player has enough tokens to fill if they use tokens available on the board
                                priority += (int)Math.Pow(space.Score, 2);
                            }
                            else
                            {
                                // player would still not have enough to fill the space, even with all the tokens in their reserve and on the board.
                                priority += 1;
                            }
                        }
                        if (availableWildColors.Contains(color))
                        {
                            foreach (AltarSpace wildSpace in openWildSpaces)
                            {
                                if (tableTileCounts[color] + boardTileCounts[color] + boardTileCounts[wildColor] >= wildSpace.GetValue())
                                {
                                    // player has enough tokens to fill if they use tokens available on the board
                                    int score = this.GetAltarSpaceScoreValue(wildAltar, wildSpace);
                                    priority += (int)Math.Pow(score, 2);
                                }
                            }
                        }

                    }
                    priorities[color] = priority;
                }

                TileColor highestPriority = TileColor.PURPLE;
                foreach (KeyValuePair<TileColor, int> keyValuePair in priorities)
                {
                    if (keyValuePair.Value > priorities[highestPriority])
                    {
                        highestPriority = keyValuePair.Key;
                    }
                }
                UnityEngine.Debug.Log($"AIPlayerController Highest priority: {highestPriority}");
                return priorities;
            }

            private int GetAltarSpaceScoreValue(Altar altar, AltarSpace altarSpace)
            {
                if (altarSpace.IsEmpty())
                {
                    return System.Instance.GetScoreBoardController().CalculatePointsForTilePlacement(altar, altarSpace.GetValue());
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}

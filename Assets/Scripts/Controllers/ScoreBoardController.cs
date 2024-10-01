using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Model;
using Azul.PlayerBoardEvents;
using Azul.ScoreBoardEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace ScoreBoardEvents
    {
        public class OnScoreBoardUpdatePayload
        {
            public List<int> Scores { get; init; }
        }
    }
    namespace Controller
    {
        public class ScoreBoardController : MonoBehaviour
        {
            [SerializeField] private List<StarCompletedMilestone> starCompletedMilestones;
            [SerializeField] private List<NumberCompletedMilestone> numberCompletedMilestones;
            private UnityEvent<OnScoreBoardUpdatePayload> onScoreChange = new();

            private Dictionary<int, int> playerScores = new();


            public void SetupGame(int numPlayers)
            {
                this.InitializeScores(numPlayers);
            }

            public void InitializeListeners()
            {
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                playerBoardController.AddOnPlayerAcquiresOneTileListener(this.OnPlayerAcquireOneTile);
                playerBoardController.AddOnPlaceStarTileListener(this.OnPlayerPlaceTile);
                playerBoardController.AddOnPlayerBoardTilesDiscardedListener(this.OnPlayerDiscardTiles);
            }

            public int GetPlayerScore(int playerNumber)
            {
                return this.playerScores[playerNumber];
            }

            public void DeductPoints(int player, int points)
            {
                this.playerScores[player] = Math.Max(this.playerScores[player] - points, 0);
                this.NotifyScoreUpdate();
            }

            public void AddPoints(int player, int points)
            {
                this.playerScores[player] += points;
                this.NotifyScoreUpdate();
            }

            private void OnPlayerAcquireOneTile(OnPlayerAcquireOneTilePayload payload)
            {
                this.DeductPoints(payload.PlayerNumber, payload.AcquiredTiles.Count);
            }

            private void OnPlayerDiscardTiles(OnPlayerBoardTilesDiscardedPayload payload)
            {
                this.DeductPoints(payload.PlayerNumber, payload.NumberOfTilesDiscarded);
            }

            private void InitializeScores(int numPlayers)
            {
                for (int idx = 0; idx < numPlayers; idx++)
                {
                    this.playerScores[idx] = 5;
                }
            }

            private void OnPlayerPlaceTile(OnPlayerBoardPlaceStarTilePayload payload)
            {
                int points = this.CalculatePointsForTilePlacement(payload.Star, payload.TilePlaced);
                points += this.CalculatePointsForMilestones(payload);
                this.AddPoints(payload.PlayerNumber, points);
            }

            public int CalculatePointsForTilePlacement(Altar star, int tilePlaced)
            {
                int points;
                int numSpaces = star.GetNumberOfSpaces();
                List<int> filledSpaces = star.GetFilledSpaces().Select(space => space.GetValue()).ToList();
                if (filledSpaces.Count == numSpaces)
                {
                    // this one's easy - if you just filled up the star, you get 6 points
                    points = star.GetNumberOfSpaces();
                }
                else
                {
                    List<int> earnedSpaces = new();
                    for (int idx = 0; idx < numSpaces; idx++)
                    {
                        int value = tilePlaced + idx;
                        if (value > numSpaces)
                        {
                            value = numSpaces - value;
                        }
                        if (filledSpaces.Contains(value) || value == tilePlaced)
                        {
                            earnedSpaces.Add(value);
                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int idx = 0; idx < numSpaces; idx++)
                    {
                        int value = tilePlaced - idx;
                        if (value <= 0)
                        {
                            value = numSpaces + value;
                        }
                        if (filledSpaces.Contains(value) || value == tilePlaced)
                        {
                            earnedSpaces.Add(value);
                        }
                        else
                        {
                            break;
                        }
                    }
                    points = earnedSpaces.Distinct().Count();
                }
                return points;
            }

            private int CalculatePointsForMilestones(OnPlayerBoardPlaceStarTilePayload payload)
            {
                int points = 0;
                StarCompletedMilestone starCompletedMilestone = this.starCompletedMilestones.Find(milestone => milestone.GetColor() == payload.Star.GetColor());
                NumberCompletedMilestone numberCompletedMilestone = this.numberCompletedMilestones.Find(milestone => milestone.GetNumber() == payload.TilePlaced);
                if (starCompletedMilestone.IsMilestoneComplete(payload.Star))
                {
                    points += starCompletedMilestone.GetPoints();
                }
                if (numberCompletedMilestone.IsMilestoneComplete(payload.PlayerNumber, payload.TilePlaced))
                {
                    points += numberCompletedMilestone.GetPoints();
                }
                return points;
            }

            private void NotifyScoreUpdate()
            {
                int[] playerScores = new int[this.playerScores.Count];
                foreach (KeyValuePair<int, int> playerScore in this.playerScores)
                {
                    playerScores[playerScore.Key] = playerScore.Value;
                }
                this.onScoreChange.Invoke(new OnScoreBoardUpdatePayload
                {
                    Scores = playerScores.ToList()
                });
            }

            public void AddOnScoreBoardUpdatedListener(UnityAction<OnScoreBoardUpdatePayload> listener)
            {
                this.onScoreChange.AddListener(listener);
            }

            public List<StarCompletedMilestone> GetStarCompletedMilestones()
            {
                return this.starCompletedMilestones;
            }

            public List<NumberCompletedMilestone> GetNumberCompletedMilestones()
            {
                return this.numberCompletedMilestones;
            }
        }
    }
}

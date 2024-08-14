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
            [SerializeField] private Vector3 position = new Vector3(100, 1, -55);
            [SerializeField][Range(1, 10)] private int supplySize = 10;
            [SerializeField] private GameObject scoreBoardPrefab;
            [SerializeField] private GameObject tilePrefab;
            [SerializeField] private GameObject roundCounterPrefab;
            [SerializeField] private List<StarCompletedMilestone> starCompletedMilestones;
            [SerializeField] private List<NumberCompletedMilestone> numberCompletedMilestones;
            private UnityEvent<OnScoreBoardUpdatePayload> onScoreChange = new();

            private Dictionary<int, int> playerScores = new();

            private ScoreBoard scoreBoard;

            private List<TilePlaceholder> placeholderTiles;

            public void SetupGame(int numPlayers)
            {
                this.CreateScoreBoard();
                this.CreateSupplyStar();
                this.PlaceRoundCounter();
                this.InitializeScores(numPlayers);
            }

            public void InitializeListeners()
            {
                RoundController roundController = System.Instance.GetRoundController();
                roundController.AddOnRoundPhaseAcquireListener(this.OnRoundStart);
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                playerBoardController.AddOnPlayerAcquiresOneTileListener(this.OnPlayerAcquireOneTile);
                playerBoardController.AddOnPlaceStarTileListener(this.OnPlayerPlaceTile);
            }

            private void CreateScoreBoard()
            {
                this.scoreBoard = Instantiate(this.scoreBoardPrefab, this.position, Quaternion.identity).GetComponent<ScoreBoard>();
                this.scoreBoard.transform.position = this.position;
            }

            private void CreateSupplyStar()
            {
                this.placeholderTiles = new();
                for (int idx = 0; idx < this.supplySize; idx++)
                {
                    TilePlaceholder tile = TilePlaceholder.Create(this.tilePrefab, TileColor.WILD);
                    this.placeholderTiles.Add(tile);
                }
                this.scoreBoard.AddSupplyPlaceholders(this.placeholderTiles);
            }

            public void FillSupply(BagController bagController)
            {
                this.placeholderTiles.ForEach(placeholder =>
                {
                    if (placeholder.IsEmpty())
                    {
                        placeholder.PlaceTile(bagController.Draw());
                    }
                });
            }

            private void PlaceRoundCounter()
            {
                GameObject roundCounter = Instantiate(this.roundCounterPrefab);
                this.scoreBoard.PlaceCounter(roundCounter);
            }

            public ScoreBoard GetScoreBoard()
            {
                return this.scoreBoard;
            }

            public void DeductPoints(int player, int points)
            {
                this.playerScores[player] = Math.Max(this.playerScores[player] - points, 5);
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

            private void OnRoundStart(OnRoundPhaseAcquirePayload payload)
            {
                this.scoreBoard.StartRound(payload.RoundNumber);
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
                int points = this.CalculatePointsForTilePlacement(payload);
                points += this.CalculatePointsForMilestones(payload);
                this.AddPoints(payload.PlayerNumber, points);
            }

            private int CalculatePointsForTilePlacement(OnPlayerBoardPlaceStarTilePayload payload)
            {
                int points;
                int numSpaces = payload.Star.GetNumberOfSpaces();
                List<int> filledSpaces = payload.Star.GetFilledSpaces().Select(space => space.GetValue()).ToList();
                if (filledSpaces.Count == numSpaces)
                {
                    // this one's easy - if you just filled up the star, you get 6 points
                    points = payload.Star.GetNumberOfSpaces();
                }
                else if (filledSpaces.Count == 1)
                {
                    // also easy - if this is your first space, you get 1 point
                    points = 1;
                }
                else
                {
                    List<int> earnedSpaces = new();
                    for (int idx = 0; idx < numSpaces; idx++)
                    {
                        int value = payload.TilePlaced + idx;
                        if (value > numSpaces)
                        {
                            value = numSpaces - value;
                        }
                        if (filledSpaces.Contains(value))
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
                        int value = payload.TilePlaced - idx;
                        if (value <= 0)
                        {
                            value = numSpaces + value;
                        }
                        if (filledSpaces.Contains(value))
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
        }
    }
}

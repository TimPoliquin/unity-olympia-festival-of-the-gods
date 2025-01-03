using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.AIEvents;
using Azul.Controller;
using Azul.Event;
using Azul.Model;
using Azul.PlayerBoardEvents;
using Azul.Util;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace AIEvents
    {
        public struct OnDrawFromFactoryPayload
        {
            public Factory Factory { get; init; }
            public TileColor DesiredColor { get; init; }
            public TileColor WildColor { get; init; }
        }
        public struct OnDrawFromTablePayload
        {
            public TileColor DesiredColor { get; init; }
            public TileColor WildColor { get; init; }
        }
        public struct OnScoreSpaceSelectedPayload
        {
            public AltarSpace Selection;
        }
        public struct OnGoalScoreTilesSelectedPayload
        {
            public PlayerBoard PlayerBoard;
            public TileColor TileColor;
            public int Value;
            public UnityAction<OnPlayerBoardScoreTileSelectionConfirmPayload> OnConfirm;
        }
    }
    namespace AI
    {
        public enum GoalStatus
        {
            /// <summary>
            /// Goal has not been started
            /// </summary>
            NOT_STARTED,
            /// <summary>
            /// Goal has at least some progress
            /// </summary>
            IN_PROGRESS,
            /// <summary>
            /// Goal was completed
            /// </summary>
            COMPLETE,
        }

        public enum GoalAcquireFeasability
        {
            /// <summary>
            /// Goal has been acquired, either in the player's supply
            /// or star board.
            /// </summary>
            ACQUIRED,
            /// <summary>
            /// Goal can be accomplished in a single available move
            /// </summary>
            HIGHLY_LIKELY,
            /// <summary>
            /// Goal can be accomplished in 2 available moves
            /// </summary>
            LIKELY,
            /// <summary>
            /// Goal can be accomplished in N available moves
            /// </summary>
            NOT_LIKELY,
            /// <summary>
            /// Goal cannot be accomplished in this round
            /// </summary>
            IMPOSSIBLE
        }

        public interface Goal
        {
            public GoalStatus EvaluateCompletion();
            public GoalAcquireFeasability CalculateAcquireFeasibility();
            public void Acquire();

            public CoroutineStatus PrepareForScoring();

            public AltarSpace ChooseSpace();

            public TileColor ChooseWildColor(AltarSpace space);

            public void EndScoring();

            public int GetScoreProgress();

            public bool CanScore();

            public void AddOnDrawFromTableListener(UnityAction<OnDrawFromTablePayload> listener);
            public void AddOnDrawFromFactoryListener(UnityAction<OnDrawFromFactoryPayload> listener);
            public void AddOnScoreSpaceSelectedListener(UnityAction<EventTracker<OnScoreSpaceSelectedPayload>> listener);
            public void AddOnTileSelectedListener(UnityAction<OnGoalScoreTilesSelectedPayload> listener);
        }

        public class UnimplementedGoal : Goal
        {

            public void Acquire()
            {
                throw new NotImplementedException();
            }

            public void AddOnDrawFromFactoryListener(UnityAction<OnDrawFromFactoryPayload> listener)
            {
                throw new NotImplementedException();
            }

            public void AddOnDrawFromTableListener(UnityAction<OnDrawFromTablePayload> listener)
            {
                throw new NotImplementedException();
            }

            public void AddOnScoreSpaceSelectedListener(UnityAction<EventTracker<OnScoreSpaceSelectedPayload>> listener)
            {
                throw new NotImplementedException();
            }

            public void AddOnTileSelectedListener(UnityAction<OnGoalScoreTilesSelectedPayload> listener)
            {
                throw new NotImplementedException();
            }

            public GoalAcquireFeasability CalculateAcquireFeasibility()
            {
                throw new NotImplementedException();
            }

            public bool CanScore()
            {
                throw new NotImplementedException();
            }

            public AltarSpace ChooseSpace()
            {
                throw new NotImplementedException();
            }

            public TileColor ChooseWildColor(AltarSpace space)
            {
                throw new NotImplementedException();
            }

            public void EndScoring()
            {
                throw new NotImplementedException();
            }

            public GoalStatus EvaluateCompletion()
            {
                throw new NotImplementedException();
            }

            public int GetScoreProgress()
            {
                throw new NotImplementedException();
            }

            public CoroutineStatus PrepareForScoring()
            {
                throw new NotImplementedException();
            }
        }

        [Serializable]
        public class StarGoal : UnimplementedGoal
        {
            [SerializeField] private int playerNumber;
            [SerializeField] private TileColor starColor;
            [SerializeField] private GoalStatus goalStatus;

            internal static StarGoal Create(int playerNumber, TileColor starColor)
            {
                return new StarGoal
                {
                    playerNumber = playerNumber,
                    starColor = starColor,
                    goalStatus = GoalStatus.NOT_STARTED
                };
            }

            public bool IsComplete()
            {
                return this.goalStatus == GoalStatus.COMPLETE;
            }

        }

        public class NumberGoal : UnimplementedGoal
        {
            [SerializeField] private int playerNumber;
            [SerializeField] private int number;
            [SerializeField] private GoalStatus goalStatus;

            internal static NumberGoal Create(int playerNumber, int number)
            {
                return new NumberGoal
                {
                    playerNumber = playerNumber,
                    number = number,
                    goalStatus = GoalStatus.NOT_STARTED
                };
            }

            public bool IsComplete()
            {
                return this.goalStatus == GoalStatus.COMPLETE;
            }
        }

        public sealed class GoalUtils
        {
            public static Goal CreateStarGoal(int playerNumber, TileColor starColor)
            {
                return StarGoal.Create(playerNumber, starColor);
            }

            public static Goal CreateNumberGoal(int playerNumber, int number)
            {
                return NumberGoal.Create(playerNumber, number);
            }
            public static Goal CreateRandomGoal(int playerNumber)
            {
                return RandomGoal.Create(playerNumber);
            }

            public static Goal CreateAgressiveGoal(int playerNumber)
            {
                return AgressiveGoal.Create(playerNumber);
            }

            public static List<Goal> SortByFeasibility(List<Goal> goals)
            {
                return goals.OrderBy(goal => goal.CalculateAcquireFeasibility()).ToList();
            }

        }

    }
}

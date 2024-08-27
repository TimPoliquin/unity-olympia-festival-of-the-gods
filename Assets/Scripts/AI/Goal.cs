using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.AIEvents;
using Azul.Controller;
using Azul.Model;
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

        public enum GoalFeasability
        {
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
            public GoalFeasability CalculateFeasibility();
            public void Act();

            public void AddOnDrawFromTableListener(UnityAction<OnDrawFromTablePayload> listener);
            public void AddOnDrawFromFactoryListener(UnityAction<OnDrawFromFactoryPayload> listener);
        }

        [Serializable]
        public class StarGoal : Goal
        {
            [SerializeField] private TileColor starColor;
            [SerializeField] private GoalStatus goalStatus;


            internal static StarGoal Create(TileColor starColor)
            {
                return new StarGoal
                {
                    starColor = starColor,
                    goalStatus = GoalStatus.NOT_STARTED
                };
            }

            public bool IsComplete()
            {
                return this.goalStatus == GoalStatus.COMPLETE;
            }

            public GoalFeasability CalculateFeasibility()
            {
                throw new NotImplementedException();
            }

            public GoalStatus EvaluateCompletion()
            {
                throw new NotImplementedException();
            }

            public void Act()
            {
                throw new NotImplementedException();
            }

            public void AddOnDrawFromTableListener(UnityAction<OnDrawFromTablePayload> listener)
            {
                throw new NotImplementedException();
            }

            public void AddOnDrawFromFactoryListener(UnityAction<OnDrawFromFactoryPayload> listener)
            {
                throw new NotImplementedException();
            }
        }

        public class NumberGoal : Goal
        {
            [SerializeField] private int number;
            [SerializeField] private GoalStatus goalStatus;

            internal static NumberGoal Create(int number)
            {
                return new NumberGoal
                {
                    number = number,
                    goalStatus = GoalStatus.NOT_STARTED
                };
            }

            public void Act()
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

            public GoalFeasability CalculateFeasibility()
            {
                throw new NotImplementedException();
            }

            public GoalStatus EvaluateCompletion()
            {
                throw new NotImplementedException();
            }

            public bool IsComplete()
            {
                return this.goalStatus == GoalStatus.COMPLETE;
            }
        }

        public sealed class GoalUtils
        {
            public static Goal CreateStarGoal(TileColor starColor)
            {
                return StarGoal.Create(starColor);
            }

            public static Goal CreateNumberGoal(int number)
            {
                return NumberGoal.Create(number);
            }
            public static Goal CreateRandomGoal()
            {
                return RandomGoal.Create();
            }

            public static List<Goal> SortByFeasibility(List<Goal> goals)
            {
                return goals.OrderBy(goal => goal.CalculateFeasibility()).ToList();
            }

        }

    }
}

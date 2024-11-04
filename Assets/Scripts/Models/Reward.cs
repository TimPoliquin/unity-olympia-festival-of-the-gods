using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Model;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace Model
    {
        [Serializable]
        public class RewardCondition
        {
            [SerializeField] private TileColor Color;
            [SerializeField] private int TileNumber;

            public TileColor GetTileColor()
            {
                return this.Color;
            }

            public int GetTileNumber()
            {
                return this.TileNumber;
            }

            public bool IsConditionMet(int playerNumber)
            {
                PlayerBoard playerBoard = System.Instance.GetPlayerBoardController().GetPlayerBoard(playerNumber);
                return playerBoard.IsSpaceFilled(this.Color, this.TileNumber);
            }

            public bool IsConditionParameter(TileColor color, int tileNumber)
            {
                return this.Color == color && this.TileNumber == tileNumber;
            }

            public override string ToString()
            {
                return $"{this.Color} {this.TileNumber}";
            }
        }

        [Serializable]
        public class Reward
        {
            [SerializeField] private int numberOfTiles;

            public int GetValue()
            {
                return this.numberOfTiles;
            }
        }

        [Serializable]
        public class RewardConfiguration
        {
            [SerializeField] private List<RewardCondition> conditions;
            [SerializeField] private Reward reward;

            public bool IsConditionMet(int playerNumber)
            {
                return this.conditions.All(condition => condition.IsConditionMet(playerNumber));
            }

            public Reward GetReward()
            {
                return this.reward;
            }

            public bool IsConditionParameter(TileColor color, int tileNumber)
            {
                return this.conditions.Any(condition => condition.IsConditionParameter(color, tileNumber));
            }

            public override string ToString()
            {
                return string.Join(" AND ", this.conditions.Select(condition => condition.ToString()).ToArray());
            }

            public int GetNumberOfConditions()
            {
                return this.conditions.Count;
            }

            public int GetNumberOfCompletedConditions(int playerNumber)
            {
                return this.conditions.FindAll(condition => condition.IsConditionMet(playerNumber)).Count;
            }

            public List<RewardCondition> GetRewardConditions()
            {
                return this.conditions;
            }
        }

        public class RewardBehavior : MonoBehaviour
        {
            public RewardConfiguration RewardConfiguration { get; set; }
            private bool Completed;
            private int playerNumber;

            private UnityEvent onComplete = new();

            public bool IsCompleted()
            {
                return this.Completed;
            }

            public void MarkCompleted()
            {
                this.Completed = true;
                this.onComplete.Invoke();
            }

            public bool IsConditionParameter(TileColor color, int tileNumber)
            {
                return this.RewardConfiguration.IsConditionParameter(color, tileNumber);
            }

            public bool IsConditionMet()
            {
                return this.RewardConfiguration.IsConditionMet(this.playerNumber);
            }

            public int GetReward()
            {
                return this.RewardConfiguration.GetReward().GetValue();
            }

            public int GetNumberOfConditions()
            {
                return this.RewardConfiguration.GetNumberOfConditions();
            }

            public int GetNumberOfCompletedConditions()
            {
                return this.RewardConfiguration.GetNumberOfCompletedConditions(playerNumber);
            }

            public override string ToString()
            {
                return $"{this.RewardConfiguration.ToString()}: {this.Completed}";
            }

            public void AddOnCompleteListener(UnityAction listener)
            {
                this.onComplete.AddListener(listener);
            }

            public static RewardBehavior Create(GameObject parent, int playerNumber, RewardConfiguration rewardConfiguration)
            {
                RewardBehavior rewardBehavior = parent.AddComponent<RewardBehavior>();
                rewardBehavior.playerNumber = playerNumber;
                rewardBehavior.Completed = false;
                rewardBehavior.RewardConfiguration = rewardConfiguration;
                return rewardBehavior;
            }
        }
    }
}

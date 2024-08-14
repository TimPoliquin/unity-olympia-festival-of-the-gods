using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Model;
using Unity.VisualScripting;
using UnityEngine;

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
        }

        public class RewardBehavior : MonoBehaviour
        {
            private RewardConfiguration RewardConfiguration { get; set; }
            private bool Completed;
            private int playerNumber;

            public bool IsCompleted()
            {
                return this.Completed;
            }

            public void MarkCompleted()
            {
                this.Completed = true;
                // update presentation to show completed
            }

            public bool IsConditionParameter(TileColor color, int tileNumber)
            {
                return this.RewardConfiguration.IsConditionParameter(color, tileNumber);
            }

            public bool IsConditionMet()
            {
                return this.RewardConfiguration.IsConditionMet(this.playerNumber);
            }

            public static RewardBehavior Create(int playerNumber, RewardConfiguration rewardConfiguration, GameObject prefab)
            {
                RewardBehavior behavior = Instantiate(prefab).AddComponent<RewardBehavior>();
                behavior.playerNumber = playerNumber;
                behavior.Completed = false;
                behavior.RewardConfiguration = rewardConfiguration;
                return behavior;
            }
        }
    }
}

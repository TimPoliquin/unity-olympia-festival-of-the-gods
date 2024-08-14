using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Layout;
using Azul.Model;
using Azul.PlayerBoardEvents;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{

    namespace Controller
    {
        public class RewardController : MonoBehaviour
        {
            [Serializable]
            public class RewardLayout
            {
                [SerializeField] private int Value;
                [SerializeField] private CircularLayout Layout;
                [SerializeField] private GameObject Prefab;

                public GameObject GetPrefab()
                {
                    return this.Prefab;
                }

                public int GetValue()
                {
                    return this.Value;
                }

                public CircularLayout GetLayout()
                {
                    return this.Layout;
                }
            }
            [SerializeField] private List<RewardConfiguration> configurations;
            [SerializeField] private List<RewardLayout> layouts;
            private int playerNumber;

            private List<RewardBehavior> rewardBehaviors;


            public void SetupGame(int playerNumber)
            {
                this.playerNumber = playerNumber;
                this.CreateRewardSpaces();
                // TODO - this should be triggered...elsewhere.
                this.InitializeListeners();
            }

            public void InitializeListeners()
            {
                System.Instance.GetPlayerBoardController().AddOnPlaceStarTileListener((payload) =>
                {
                    if (payload.PlayerNumber == this.playerNumber)
                    {
                        this.OnPlaceStarTile(payload);
                    }
                });
            }

            private void CreateRewardSpaces()
            {
                this.rewardBehaviors = new();
                foreach (RewardLayout rewardLayout in this.layouts)
                {
                    List<RewardConfiguration> layoutConfigurations = this.configurations.FindAll(configuration => configuration.GetReward().GetValue() == rewardLayout.GetValue());
                    List<RewardBehavior> rewards = new();
                    foreach (RewardConfiguration configuration in layoutConfigurations)
                    {
                        RewardBehavior rewardBehavior = this.CreateRewardSpace(rewardLayout.GetPrefab(), configuration);
                        rewards.Add(rewardBehavior);
                    }
                    rewardBehaviors.AddRange(rewards);
                    rewardLayout.GetLayout().AddChildren(rewards.Select(reward => reward.gameObject).ToList());
                }
            }

            private RewardBehavior CreateRewardSpace(GameObject prefab, RewardConfiguration rewardConfiguration)
            {
                return RewardBehavior.Create(this.playerNumber, rewardConfiguration, prefab);
            }

            private void OnPlaceStarTile(OnPlayerBoardPlaceStarTilePayload payload)
            {
                List<RewardBehavior> behaviors = this.rewardBehaviors.FindAll(
                    behavior => !behavior.IsCompleted() && behavior.IsConditionParameter(payload.Star.GetColor(), payload.TilePlaced)
                ).ToList();
                UnityEngine.Debug.Log($"Checking for completed status... {behaviors.Count}");
                foreach (RewardBehavior rewardBehavior in behaviors)
                {
                    if (rewardBehavior.IsConditionMet())
                    {
                        rewardBehavior.MarkCompleted();
                        // TODO - dispatch event to grant reward
                    }
                }
            }


        }

    }
}

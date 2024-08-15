using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Layout;
using Azul.Model;
using Azul.PlayerBoardEvents;
using Azul.PlayerBoardRewardEvents;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace PlayerBoardRewardEvents
    {
        public class OnPlayerBoardEarnRewardPayload
        {
            public int PlayerNumber { get; init; }
            public int NumberOfTiles { get; init; }
        }
    }

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

            private UnityEvent<OnPlayerBoardEarnRewardPayload> onEarnReward = new();


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
                int rewardCount = 0;
                foreach (RewardBehavior rewardBehavior in behaviors)
                {
                    UnityEngine.Debug.Log($"Checking reward: {rewardBehavior.ToString()}");
                    if (rewardBehavior.IsConditionMet())
                    {
                        UnityEngine.Debug.Log($"Reward condition met!");
                        rewardBehavior.MarkCompleted();
                        rewardCount += rewardBehavior.GetReward();
                    }
                }
                if (rewardCount > 0)
                {
                    UnityEngine.Debug.Log($"Earned reward! {rewardCount}");
                    this.onEarnReward.Invoke(new OnPlayerBoardEarnRewardPayload
                    {
                        PlayerNumber = this.playerNumber,
                        NumberOfTiles = rewardCount
                    });
                }
            }

            public void AddOnPlayerBoardEarnRewardListener(UnityAction<OnPlayerBoardEarnRewardPayload> listener)
            {
                this.onEarnReward.AddListener(listener);
            }
        }

    }
}

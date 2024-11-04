using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Event;
using Azul.Layout;
using Azul.Model;
using Azul.PlayerBoardEvents;
using Azul.PlayerBoardRewardEvents;
using Azul.PointerEvents;
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

        public struct OnPlayerBoardHoverEnterReward
        {
            public RewardIndicator RewardIndicator { get; init; }
        }

        public struct OnPlayerBoardHoverExitReward
        {
            public RewardIndicator RewardIndicator { get; init; }
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
                [SerializeField] private RewardIndicator Prefab;

                public RewardIndicator GetPrefab()
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
            private List<RewardIndicator> rewardIndicators;

            private AzulEvent<OnPlayerBoardEarnRewardPayload> onEarnReward = new();
            private UnityEvent<OnPointerEnterPayload<RewardIndicator>> onHoverEnterReward = new();
            private UnityEvent<OnPointerExitPayload<RewardIndicator>> onHoverExitReward = new();

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
                    if (payload.Payload.PlayerNumber == this.playerNumber)
                    {
                        this.StartCoroutine(this.OnPlaceStarTile(payload.Payload, payload.Done));
                    }
                    else
                    {
                        payload.Done();
                    }
                });
            }

            public List<RewardBehavior> GetRewardBehaviors(TileColor tileColor, int value)
            {
                return this.rewardIndicators.Select(indicator => indicator.GetRewardBehavior()).Where(behavior => behavior.IsConditionParameter(tileColor, value)).ToList();
            }

            private void CreateRewardSpaces()
            {
                this.rewardIndicators = new();
                foreach (RewardLayout rewardLayout in this.layouts)
                {
                    List<RewardConfiguration> layoutConfigurations = this.configurations.FindAll(configuration => configuration.GetReward().GetValue() == rewardLayout.GetValue());
                    List<RewardIndicator> rewards = new();
                    foreach (RewardConfiguration configuration in layoutConfigurations)
                    {
                        RewardIndicator rewardIndicator = this.CreateRewardSpace(rewardLayout.GetPrefab(), configuration);
                        rewards.Add(rewardIndicator);
                    }
                    rewardIndicators.AddRange(rewards);
                    rewardLayout.GetLayout().AddChildren(rewards.Select(reward => reward.gameObject).ToList());
                }
            }

            private RewardIndicator CreateRewardSpace(RewardIndicator prefab, RewardConfiguration rewardConfiguration)
            {
                RewardIndicator rewardIndicator = Instantiate(prefab);
                RewardBehavior rewardBehavior = RewardBehavior.Create(rewardIndicator.gameObject, this.playerNumber, rewardConfiguration);
                rewardIndicator.SetRewardBehavior(rewardBehavior);
                rewardIndicator.GetPointerEventController().AddOnPointerEnterListener(this.OnPointerEnter);
                rewardIndicator.GetPointerEventController().AddOnPointerExitListener(this.OnPointerExit);
                return rewardIndicator;
            }

            private IEnumerator OnPlaceStarTile(OnPlayerBoardPlaceStarTilePayload payload, Action Done)
            {
                List<RewardBehavior> behaviors = this.rewardIndicators.Select(indicator => indicator.GetRewardBehavior()).Where(
                    behavior => !behavior.IsCompleted() && behavior.IsConditionParameter(payload.Star.GetColor(), payload.TilePlaced)
                ).ToList();
                int rewardCount = 0;
                foreach (RewardBehavior rewardBehavior in behaviors)
                {
                    if (rewardBehavior.IsConditionMet())
                    {
                        rewardBehavior.MarkCompleted();
                        rewardCount += rewardBehavior.GetReward();
                    }
                }
                if (rewardCount > 0)
                {
                    UnityEngine.Debug.Log($"Reward Controller: Player {payload.PlayerNumber} earned {rewardCount} rewards");
                    yield return this.onEarnReward.Invoke(new OnPlayerBoardEarnRewardPayload
                    {
                        PlayerNumber = this.playerNumber,
                        NumberOfTiles = rewardCount
                    }).WaitUntilCompleted();
                    UnityEngine.Debug.Log($"Reward Controller: Player {payload.PlayerNumber} claimed rewards");
                }
                Done();
            }

            public void AddOnPlayerBoardEarnRewardListener(UnityAction<EventTracker<OnPlayerBoardEarnRewardPayload>> listener)
            {
                this.onEarnReward.AddListener(listener);
            }

            public void AddOnPointerEnterRewardListener(UnityAction<OnPointerEnterPayload<RewardIndicator>> listener)
            {
                this.onHoverEnterReward.AddListener(listener);
            }

            public void AddOnPointerExitRewardListener(UnityAction<OnPointerExitPayload<RewardIndicator>> listener)
            {
                this.onHoverExitReward.AddListener(listener);
            }

            private void OnPointerEnter(OnPointerEnterPayload<RewardIndicator> payload)
            {
                this.onHoverEnterReward.Invoke(payload);
            }

            private void OnPointerExit(OnPointerExitPayload<RewardIndicator> payload)
            {
                this.onHoverExitReward.Invoke(payload);

            }
        }

    }
}

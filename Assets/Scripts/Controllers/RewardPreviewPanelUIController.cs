using System;
using System.Collections;
using System.Collections.Generic;
using Azul.GameEvents;
using Azul.Model;
using Azul.PointerEvents;
using UnityEngine;
using UnityEngine.UIElements;

namespace Azul
{
    namespace Controller
    {
        public class RewardPreviewPanelUIController : MonoBehaviour
        {
            [SerializeField] private float hoverDelay = .25f;
            [SerializeField] private Vector3 offset = Vector3.right * 40f;

            private RewardPreviewPanelUI panel;
            private Dictionary<RewardIndicator, bool> rewardIndicatorStatus = new();
            void Start()
            {
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnGameSetupComplete);
            }

            public void OnGameSetupComplete(OnGameSetupCompletePayload payload)
            {
                System.Instance.GetPlayerBoardController().AddOnPointerEnterRewardListener(this.OnHoverEnter);
                System.Instance.GetPlayerBoardController().AddOnPointerExitRewardListener(this.OnHoverExit);
            }

            private void OnHoverEnter(OnPointerEnterPayload<RewardIndicator> payload)
            {
                rewardIndicatorStatus[payload.Target] = true;
                this.StartCoroutine(this.ShowPanel(this.hoverDelay, payload.Target));
            }

            private void OnHoverExit(OnPointerExitPayload<RewardIndicator> payload)
            {
                this.rewardIndicatorStatus[payload.Target] = false;
                if (null != this.panel)
                {
                    this.StartCoroutine(this.HidePanel(this.panel));
                    this.panel = null;
                }
            }

            private IEnumerator ShowPanel(float hoverDelay, RewardIndicator target)
            {
                yield return new WaitForSeconds(hoverDelay);
                if (this.rewardIndicatorStatus[target])
                {
                    this.rewardIndicatorStatus[target] = false;
                    this.panel = System.Instance.GetPrefabFactory().CreateRewardPreviewPanelUI();
                    this.panel.SetBoonValue(target.GetRewardBehavior().GetReward());
                    foreach (RewardCondition rewardCondition in target.GetRewardBehavior().RewardConfiguration.GetRewardConditions())
                    {
                        RewardRitualUI rewardRitualUI = System.Instance.GetPrefabFactory().CreateRewardRitualUI();
                        rewardRitualUI.Setup(rewardCondition.GetTileColor(), rewardCondition.GetTileNumber(), rewardCondition.IsConditionMet(target.GetRewardBehavior().GetPlayerNumber()));
                        this.panel.AddRequirement(rewardRitualUI);
                    }
                    this.panel.transform.position = Camera.main.WorldToScreenPoint(target.transform.position) + this.offset;
                    yield return this.panel.Show().WaitUntilCompleted();
                }
            }

            private IEnumerator HidePanel(RewardPreviewPanelUI panel)
            {
                yield return panel.Hide();
                if (null != panel && null != panel.gameObject)
                {
                    Destroy(panel.gameObject);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Azul.AltarSpaceEvents;
using Azul.GameEvents;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class ScoreTilesPreviewPanelUIController : MonoBehaviour
        {
            private ScoreTilesPreviewPanelUI panel;

            void Start()
            {
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.InitializeListeners);
            }

            void InitializeListeners(OnGameSetupCompletePayload payload)
            {
                List<PlayerBoard> playerBoards = System.Instance.GetPlayerBoardController().GetPlayerBoards();
                foreach (PlayerBoard playerBoard in playerBoards)
                {
                    List<Altar> altars = playerBoard.GetAltars();
                    foreach (Altar altar in altars)
                    {
                        List<AltarSpace> altarSpaces = altar.GetSpaces();
                        foreach (AltarSpace altarSpace in altarSpaces)
                        {
                            altarSpace.AddOnStarSpaceHoverEnterListener((payload) =>
                            {
                                this.OnAltarSpaceHoverEnter(playerBoard, altar, payload);
                            });
                            altarSpace.AddOnStarSpaceHoverExitListener(this.OnAltarSpaceHoverExit);
                        }
                    }
                }
            }

            void OnAltarSpaceHoverEnter(PlayerBoard playerBoard, Altar altar, OnAltarSpaceHoverEnterPayload payload)
            {
                this.CleanupPanel();
                if (System.Instance.GetUIController().GetScoreTileSelectionUIController().IsPanelOpen())
                {
                    return;
                }
                else if (System.Instance.GetUIController().GetSelectRewardUIController().IsRewardPanelOpen())
                {
                    return;
                }
                else if (payload.Target.IsEmpty())
                {
                    this.panel = System.Instance.GetPrefabFactory().CreateScoreTilesPreviewPanelUI();
                    this.panel.SetSpaceValue(payload.Target.GetOriginColor(), payload.Target.GetValue());
                    this.panel.SetScoreValue(System.Instance.GetScoreBoardController().CalculatePointsForTilePlacement(altar, payload.Target.GetValue()));
                    this.panel.SetColorProgress(payload.Target.GetOriginColor(), altar.GetFilledSpaces().Count, altar.GetNumberOfSpaces());
                    if (payload.Target.GetValue() < 5)
                    {
                        this.panel.SetValueProgress(payload.Target.GetValue(), this.GetValueProgress(playerBoard, payload.Target.GetValue()), playerBoard.GetAltars().Count);
                    }
                    this.GetRewardProgress(playerBoard, payload.Target).ForEach(rewardProgressUI =>
                    {
                        this.panel.AddRewardProgress(rewardProgressUI);
                    });
                    this.panel.Show(payload.Target.transform.position);
                    payload.Target.AddOnStarSpaceSelectListener(this.OnAltarSpaceSelect);
                }
            }

            void OnAltarSpaceHoverExit(OnAltarSpaceHoverExitPayload payload)
            {
                this.CleanupPanel();
                payload.Target.RemoveOnStarSpaceSelectListener(this.OnAltarSpaceSelect);
            }

            void OnAltarSpaceSelect(OnAltarSpaceSelectPayload payload)
            {
                this.CleanupPanel();
                payload.Target.RemoveOnStarSpaceSelectListener(this.OnAltarSpaceSelect);
            }

            void CleanupPanel()
            {
                if (this.panel != null)
                {
                    this.panel.Hide((panel) =>
                    {
                        Destroy(panel.gameObject);
                    });
                    this.panel = null;
                }
            }

            int GetValueProgress(PlayerBoard playerBoard, int value)
            {
                int valueProgress = 0;
                foreach (Altar altar in playerBoard.GetAltars())
                {
                    if (altar.IsSpaceFilled(value))
                    {
                        valueProgress++;
                    }
                }
                return valueProgress;
            }

            List<RewardProgressFieldUI> GetRewardProgress(PlayerBoard playerBoard, AltarSpace altarSpace)
            {
                RewardController rewardController = playerBoard.GetRewardController();
                List<RewardBehavior> behaviors = rewardController.GetRewardBehaviors(altarSpace.GetOriginColor(), altarSpace.GetValue());
                List<RewardProgressFieldUI> rewardProgressFieldUIs = new();
                foreach (RewardBehavior behavior in behaviors)
                {
                    RewardProgressFieldUI rewardProgressFieldUI = System.Instance.GetPrefabFactory().CreateRewardProgressFieldUI();
                    rewardProgressFieldUI.SetRewardValue(behavior.GetReward());
                    rewardProgressFieldUI.SetProgress(behavior.GetNumberOfCompletedConditions(), behavior.GetNumberOfConditions());
                    rewardProgressFieldUIs.Add(rewardProgressFieldUI);
                }
                return rewardProgressFieldUIs;
            }
        }
    }
}

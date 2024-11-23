using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.PlayerBoardEvents;
using UnityEngine;
using System;
using UnityEngine.Events;
using System.Linq;
using Azul.PlayerBoardRewardEvents;
using Azul.RewardUIEvents;
using Azul.Event;
using Azul.ScoringSelectionWizardEvents;
using Azul.Utils;
using Azul.TileAnimation;

namespace Azul
{
    namespace Controller
    {
        public class ScoreTileSelectionReadyCondition : ReadyCondition
        {
            public static readonly ScoreTileSelectionReadyCondition Instance = new ScoreTileSelectionReadyCondition();
            public bool IsReady()
            {
                return !System.Instance.GetUIController().GetScoreTileSelectionUIController().IsPanelOpen();
            }
        }
        public class ScoreTileSelectionUIController : MonoBehaviour
        {
            [SerializeField] private EndTurnPanelUI endTurnPanelUI;
            private ScoringSelectionWizardPanelUI currentPanel;
            private bool isCurrentPlayerHuman = false;
            private bool isEndTurnPanelPending = false;
            private ReadyChecker readyChecker;

            void Awake()
            {
                this.readyChecker = this.gameObject.AddComponent<ReadyChecker>();
                this.endTurnPanelUI.AddOnEndTurnListener(this.OnEndTurn);
                this.HideEndTurnPanel();
            }

            public void InitializeListeners()
            {
                RoundController roundController = System.Instance.GetRoundController();
                roundController.AddOnRoundPhaseAcquireListener(this.OnRoundPhaseAcquire);
                roundController.AddOnRoundPhasePrepareListener(this.OnRoundPhasePrepare);
                PlayerController playerController = System.Instance.GetPlayerController();
                playerController.AddOnPlayerTurnStartListener(this.OnPlayerTurnStart);
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                playerBoardController.AddOnPlayerBoardScoreSpaceSelectionListener(this.OnScoreSpaceSelection);
                playerBoardController.AddOnPlayerBoardWildScoreSpaceSelectionListener(this.OnWildScoreSpaceSelection);
                OverflowTileSelectionUIController overflowTileSelectionUIController = System.Instance.GetUIController().GetOverflowTileSelectionUIController();
                overflowTileSelectionUIController.AddOnCancelListener(this.OnOverflowSelectionCancel);
                playerBoardController.AddOnPlayerBoardEarnRewardListener(this.OnEarnReward);
                SelectRewardUIController selectRewardUIController = System.Instance.GetUIController().GetSelectRewardUIController();
                selectRewardUIController.AddOnClaimRewardListener(this.OnClaimReward);
            }

            public bool IsPanelOpen()
            {
                return this.currentPanel != null;
            }

            private void OnRoundPhaseAcquire(OnRoundPhaseAcquirePayload payload)
            {
                this.HideEndTurnPanel(); ;
            }
            private void OnRoundPhasePrepare(OnRoundPhasePreparePayload payload)
            {
                this.HideEndTurnPanel();
            }

            private void OnPlayerTurnStart(OnPlayerTurnStartPayload payload)
            {
                if (payload.Phase == Phase.SCORE)
                {
                    this.isCurrentPlayerHuman = payload.Player.IsHuman();
                    this.ShowEndTurnPanel();
                }
            }

            private void OnWildScoreSpaceSelection(OnPlayerBoardWildScoreSpaceSelectionPayload payload)
            {
                if (System.Instance.GetTileAnimationController().IsAnimating())
                {
                    return;
                }
                if (this.isCurrentPlayerHuman)
                {
                    this.HideEndTurnPanel();
                    this.currentPanel = this.CreateScoringSelectionWizardPanelUI(payload.PlayerNumber, payload.Space, payload.OnConfirm);
                    this.currentPanel.Show();
                }
            }


            private void OnScoreSpaceSelection(OnPlayerBoardScoreSpaceSelectionPayload payload)
            {
                if (System.Instance.GetTileAnimationController().IsAnimating())
                {
                    return;
                }
                if (this.isCurrentPlayerHuman)
                {
                    this.HideEndTurnPanel();
                    this.currentPanel = this.CreateScoringSelectionWizardPanelUI(payload.PlayerNumber, payload.Space, payload.OnConfirm);
                    this.currentPanel.Show();
                }
            }

            private void OnEndTurn()
            {
                this.HideEndTurnPanel();
                PlayerController playerController = System.Instance.GetPlayerController();
                playerController.EndPlayerScoringTurn();
            }

            private void OnOverflowSelectionCancel()
            {
                this.ShowEndTurnPanel();
            }

            private void OnEarnReward(EventTracker<OnPlayerBoardEarnRewardPayload> payload)
            {
                this.HideEndTurnPanel();
                payload.Done();
            }

            private void OnClaimReward(OnClaimRewardPayload payload)
            {
                this.ShowEndTurnPanel();
            }

            public void HideEndTurnPanel()
            {
                this.endTurnPanelUI.Hide();
            }

            public void ShowEndTurnPanel()
            {
                if (this.isEndTurnPanelPending)
                {
                    return;
                }
                else if (this.isCurrentPlayerHuman)
                {
                    this.StartCoroutine(this.ShowEndTurnPanelCoroutine());
                }
            }

            private IEnumerator ShowEndTurnPanelCoroutine()
            {
                this.isEndTurnPanelPending = true;
                yield return this.readyChecker.CheckReadiness(
                    ScoreTileSelectionReadyCondition.Instance,
                    TileAnimationControllerReadyCondition.Instance,
                    MilestoneCompletionReadyCondition.Instance,
                    SelectRewardReadyCondition.Instance
                ).WaitUntilCompleted();
                if (this.isCurrentPlayerHuman)
                {
                    this.endTurnPanelUI.Show();
                }
                this.isEndTurnPanelPending = false;
            }

            private ScoringSelectionWizardPanelUI CreateScoringSelectionWizardPanelUI(int playerNumber, AltarSpace space, UnityAction<OnPlayerBoardScoreTileSelectionConfirmPayload> onConfirm)
            {
                ScoringSelectionWizardPanelUI panel = System.Instance.GetPrefabFactory().CreateScoringSelectionWizardPanelUI();
                panel.SetPlayerNumber(playerNumber);
                panel.SetAltarSpace(space);
                panel.AddOnMtOlympusColorSelectionStepInitializeListener(this.OnMtOlympusColorSelectionStepInitialize);
                panel.AddOnTokenCountSelectionStepInitializeListener(this.OnTokenCountSelectStepInitialize);
                panel.AddOnCancel(() =>
                    {
                        this.StartCoroutine(this.HidePanelCoroutine(this.currentPanel));
                        if (ReferenceEquals(panel, this.currentPanel))
                        {
                            this.currentPanel = null;
                        }
                    });
                panel.AddOnConfirmListener((payload) =>
                {
                    Dictionary<TileColor, int> selectedTiles = payload.SelectedCounts;
                    onConfirm.Invoke(new OnPlayerBoardScoreTileSelectionConfirmPayload
                    {
                        TilesSelected = selectedTiles,
                        Color = payload.Color,
                    });
                    this.StartCoroutine(this.HidePanelCoroutine(this.currentPanel));
                    this.currentPanel = null;
                });
                return panel;
            }

            private void OnMtOlympusColorSelectionStepInitialize(OnMtOlympusColorSelectionStepInitializePayload payload)
            {
                List<TileColor> availableColors = this.GetListOfAvailableMtOlympusColors(payload.PlayerNumber, payload.Space);
                payload.Step.SetAvailableTokenColors(availableColors);
                if (availableColors.Count == 1)
                {
                    payload.Step.Select(availableColors[0]);
                }
            }

            private void OnTokenCountSelectStepInitialize(OnTokenCountSelectionStepInitializePayload payload)
            {
                TileColor wildColor = System.Instance.GetRoundController().GetCurrentRound().GetWildColor();
                PlayerBoard playerBoard = System.Instance.GetPlayerBoardController().GetPlayerBoard(payload.PlayerNumber);
                int value = payload.Space.GetValue();
                int numColor = playerBoard.GetTileCount(payload.Color);
                int numWild = playerBoard.GetTileCount(wildColor);
                int wildsNeeded = Math.Max(value - numColor, 0);
                int min;
                if (wildsNeeded > 0)
                {
                    min = Math.Max(1, value - numWild);
                }
                else
                {
                    min = Math.Max(1, Math.Min(value - numWild, value));
                }
                payload.Step.SetAltarSpace(payload.Space);
                payload.Step.ConfigurePrimarySelection(payload.Color, min, Math.Min(value, numColor), numColor, Math.Min(numColor, value));
                if (wildColor != payload.Color && value > 1 && numWild > 0)
                {
                    payload.Step.ConfigureWildSelection(wildColor, wildsNeeded, Math.Min(numWild, value - 1), numWild, wildsNeeded);
                }
                else
                {
                    payload.Step.DisableWildSelection();
                }
            }

            private List<TileColor> GetListOfAvailableMtOlympusColors(int playerNumber, AltarSpace space)
            {
                PlayerBoard playerBoard = System.Instance.GetPlayerBoardController().GetPlayerBoard(playerNumber);
                List<TileColor> usedColors = playerBoard.GetWildTileColors();
                TileColor wildColor = System.Instance.GetRoundController().GetCurrentRound().GetWildColor();
                List<TileColor> availableColors = TileColorUtils.GetTileColors().ToList().FindAll(color =>
                    {
                        int numColor = playerBoard.GetTileCount(color);
                        int numWild = playerBoard.GetTileCount(wildColor);
                        if (numColor == 0 || usedColors.Contains(color))
                        {
                            return false;
                        }
                        else
                        {
                            if (color == wildColor)
                            {
                                return numWild >= space.GetValue();
                            }
                            else
                            {
                                return numColor + numWild >= space.GetValue();
                            }
                        }
                    });
                return availableColors;
            }

            private IEnumerator HidePanelCoroutine(ScoringSelectionWizardPanelUI panel)
            {
                yield return panel.Hide().WaitUntilCompleted();
                Destroy(panel.gameObject);
                this.ShowEndTurnPanel();
            }
        }
    }
}

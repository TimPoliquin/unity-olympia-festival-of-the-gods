using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.PlayerBoardEvents;
using UnityEngine;
using UnityEngine.UI;
using System;
using Azul.ScoreTileSelectionUIEvent;
using UnityEngine.Events;
using System.Linq;
using Azul.PlayerBoardRewardEvents;
using Azul.RewardUIEvents;

namespace Azul
{
    namespace Controller
    {
        public class ScoreTileSelectionUIController : MonoBehaviour
        {
            [SerializeField] private EndTurnPanelUI endTurnPanelUI;
            private bool isCurrentPlayerHuman = false;

            private int countNeeded = 0;
            private TileColor selectedColor;
            private UnityAction<OnPlayerBoardScoreTileSelectionConfirmPayload> onConfirm;

            void Awake()
            {
                this.endTurnPanelUI.AddOnEndTurnListener(this.OnEndTurn);
                this.endTurnPanelUI.Hide();
            }

            public void InitializeListeners()
            {
                RoundController roundController = System.Instance.GetRoundController();
                roundController.AddOnRoundPhaseAcquireListener(this.OnRoundPhaseAcquire);
                roundController.AddOnRoundPhaseScoreListener(this.OnRoundPhaseScore);
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
                selectRewardUIController.AddOnGrantRewardListener(this.OnGrantReward);
            }

            private void OnRoundPhaseAcquire(OnRoundPhaseAcquirePayload payload)
            {
                this.CleanupScoreSelectionUIElements();
                this.endTurnPanelUI.Hide();
            }
            private void OnRoundPhaseScore(OnRoundPhaseScorePayload payload)
            {
                this.CleanupScoreSelectionUIElements();
                this.endTurnPanelUI.Hide();
            }
            private void OnRoundPhasePrepare(OnRoundPhasePreparePayload payload)
            {
                this.CleanupScoreSelectionUIElements();
                this.endTurnPanelUI.Hide();
            }

            private void OnPlayerTurnStart(OnPlayerTurnStartPayload payload)
            {
                if (payload.Phase == Phase.SCORE)
                {
                    this.isCurrentPlayerHuman = payload.Player.IsHuman();
                    if (this.isCurrentPlayerHuman)
                    {
                        this.endTurnPanelUI.Show();
                    }
                }
            }

            private void OnWildScoreSpaceSelection(OnPlayerBoardWildScoreSpaceSelectionPayload payload)
            {
                if (System.Instance.GetPlayerController().GetPlayer(payload.PlayerNumber).IsHuman())
                {
                    TileColor wildColor = System.Instance.GetRoundController().GetCurrentRound().GetWildColor();
                    int numWild = payload.PlayerBoard.GetTileCount(wildColor);
                    List<TileColor> usedColors = payload.PlayerBoard.GetWildTileColors();
                    List<TileColor> availableColors = TileColorUtils.GetTileColors().ToList().FindAll(color =>
                    {
                        int numColor = payload.PlayerBoard.GetTileCount(color);
                        int numWild = payload.PlayerBoard.GetTileCount(wildColor);
                        if (numColor == 0 || usedColors.Contains(color))
                        {
                            return false;
                        }
                        else
                        {
                            if (color == wildColor)
                            {
                                return numWild >= payload.Value;
                            }
                            else
                            {
                                return numColor + numWild >= payload.Value;
                            }
                        }
                    });
                    WildColorSelectionUI wildColorSelectionUI = System.Instance.GetPrefabFactory().CreateWildColorSelectionUI();
                    wildColorSelectionUI.Activate(availableColors, true, true);
                    wildColorSelectionUI.AddOnColorSelectionListener((colorSelectedPayload) =>
                    {
                        Destroy(wildColorSelectionUI.gameObject);
                        this.CreateScoreTileSelectionUIs(
                            playerBoard: payload.PlayerBoard,
                            selectedColor: colorSelectedPayload.Color,
                            value: payload.Value,
                            onConfirm: payload.OnConfirm
                        );
                    });
                }
            }


            private void OnScoreSpaceSelection(OnPlayerBoardScoreSpaceSelectionPayload payload)
            {
                if (System.Instance.GetPlayerController().GetPlayer(payload.PlayerNumber).IsHuman())
                {
                    this.CreateScoreTileSelectionUIs(
                        playerBoard: payload.PlayerBoard,
                        selectedColor: payload.Color,
                        value: payload.Value,
                        onConfirm: payload.OnConfirm
                    );
                }
            }

            private void CreateScoreTileSelectionUIs(PlayerBoard playerBoard, TileColor selectedColor, int value, UnityAction<OnPlayerBoardScoreTileSelectionConfirmPayload> onConfirm)
            {
                IconUIFactory iconUIFactory = System.Instance.GetUIController().GetIconUIFactory();
                ScoreTileSelectionPanelUI panel = System.Instance.GetPrefabFactory().CreateScoreTileSelectionPanelUI();
                panel.AddOnCancelListener(this.OnCancelSelection);
                panel.AddOnConfirmListener(this.OnConfirmSelection);
                panel.Show(value, iconUIFactory.GetIcon(selectedColor), iconUIFactory.GetBackgroundColor(selectedColor));
                TileColor wildColor = System.Instance.GetRoundController().GetCurrentRound().GetWildColor();
                this.selectedColor = selectedColor;
                this.countNeeded = value;
                int numColor = playerBoard.GetTileCount(selectedColor);
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
                this.CreateScoreTileSelectionUI(
                    panel,
                    playerBoard,
                    selectedColor,
                    min,
                    Math.Min(value, numColor),
                    Math.Min(numColor, value)
                );
                if (wildColor != selectedColor && value > 1 && numWild > 0)
                {
                    this.CreateScoreTileSelectionUI(panel, playerBoard, wildColor, wildsNeeded, Math.Min(numWild, value - 1), wildsNeeded);
                }
                this.onConfirm = onConfirm;
                this.endTurnPanelUI.Hide();
            }

            private void CreateScoreTileSelectionUI(ScoreTileSelectionPanelUI panel, PlayerBoard playerBoard, TileColor color, int min, int max, int defaultValue)
            {
                IconUIFactory iconUIFactory = System.Instance.GetUIController().GetIconUIFactory();
                ScoreTileSelectionUI scoreTileSelectionUI = System.Instance.GetPrefabFactory().CreateScoreTileSelectionUI(panel);
                scoreTileSelectionUI.SetColor(color, iconUIFactory.GetIcon(color), iconUIFactory.GetBackgroundColor(color));
                scoreTileSelectionUI.SetCounterRange(min, max, playerBoard.GetTileCount(color));
                scoreTileSelectionUI.SetDefaultValue(defaultValue);
                scoreTileSelectionUI.gameObject.name = $"Score Selection: {color}";
            }

            private void CleanupScoreSelectionUIElements()
            {
                this.countNeeded = 0;
                this.selectedColor = TileColor.ONE;
            }

            private void OnCancelSelection(OnScoreTileSelectionCancelPayload payload)
            {
                this.CleanupScoreSelectionUIElements();
                payload.Panel.Hide();
                if (this.isCurrentPlayerHuman)
                {
                    this.endTurnPanelUI.Show();
                }
            }

            private void OnConfirmSelection(OnScoreTileSelectionConfirmPayload payload)
            {
                Dictionary<TileColor, int> selectedTiles = payload.SelectedCounts;
                int countSelected = selectedTiles.Values.Aggregate((total, current) => total + current);
                if (countSelected == this.countNeeded)
                {
                    this.onConfirm.Invoke(new OnPlayerBoardScoreTileSelectionConfirmPayload
                    {
                        TilesSelected = selectedTiles,
                        Color = this.selectedColor
                    });
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(this.countNeeded), "Wrong number of tiles selected!");
                }
                this.CleanupScoreSelectionUIElements();
                payload.Panel.Hide();
                if (this.isCurrentPlayerHuman)
                {
                    this.endTurnPanelUI.Show();
                }
            }

            private void OnEndTurn()
            {
                this.endTurnPanelUI.Hide();
                PlayerController playerController = System.Instance.GetPlayerController();
                playerController.EndPlayerScoringTurn();
            }

            private void OnOverflowSelectionCancel()
            {
                this.CleanupScoreSelectionUIElements();
                if (this.isCurrentPlayerHuman)
                {
                    this.endTurnPanelUI.Show();
                }
            }

            private void OnEarnReward(OnPlayerBoardEarnRewardPayload payload)
            {
                this.CleanupScoreSelectionUIElements();
                this.endTurnPanelUI.Hide();
            }

            private void OnGrantReward(OnGrantRewardPayload payload)
            {
                this.CleanupScoreSelectionUIElements();
                if (this.isCurrentPlayerHuman)
                {
                    this.endTurnPanelUI.Show();
                }
            }

        }
    }
}

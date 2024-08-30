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

            [SerializeField] private GameObject scoreTileSelectionUIPrefab;
            [SerializeField] private GameObject scoreTileSelectionUIContainer;
            [SerializeField] private Button confirmButton;
            [SerializeField] private Button cancelButton;
            [SerializeField] private Button endTurnButton;
            [SerializeField] private WildColorSelectionUI wildColorSelectionUI;
            private List<ScoreTileSelectionUI> scoreTileSelectionUIs = new();

            private int countNeeded = 0;
            private TileColor selectedColor;
            private UnityAction<OnPlayerBoardScoreTileSelectionConfirmPayload> onConfirm;

            void Awake()
            {
                this.cancelButton.onClick.AddListener(this.OnCancelSelection);
                this.confirmButton.onClick.AddListener(this.OnConfirmSelection);
                this.endTurnButton.onClick.AddListener(this.OnEndTurn);
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
                this.endTurnButton.gameObject.SetActive(false);
            }
            private void OnRoundPhaseScore(OnRoundPhaseScorePayload payload)
            {
                this.CleanupScoreSelectionUIElements();
                this.endTurnButton.gameObject.SetActive(true);
            }
            private void OnRoundPhasePrepare(OnRoundPhasePreparePayload payload)
            {
                this.CleanupScoreSelectionUIElements();
                this.endTurnButton.gameObject.SetActive(false);
            }

            private void OnPlayerTurnStart(OnPlayerTurnStartPayload payload)
            {
                if (payload.Phase == Phase.SCORE)
                {
                    this.endTurnButton.gameObject.SetActive(true);
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
                    this.wildColorSelectionUI.Activate(payload.Space.gameObject, availableColors, true);
                    this.wildColorSelectionUI.AddOnColorSelectionListener((colorSelectedPayload) =>
                    {
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
                this.CleanupScoreSelectionUIElements();
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
                    playerBoard,
                    selectedColor,
                    min,
                    Math.Min(value, numColor),
                    Math.Min(numColor, value)
                );
                if (wildColor != selectedColor && value > 1 && numWild > 0)
                {
                    this.CreateScoreTileSelectionUI(playerBoard, wildColor, wildsNeeded, Math.Min(numWild, value - 1), wildsNeeded);
                }
                this.onConfirm = onConfirm;
                this.confirmButton.gameObject.SetActive(true);
                this.cancelButton.gameObject.SetActive(true);
                this.endTurnButton.gameObject.SetActive(false);
            }

            private void CreateScoreTileSelectionUI(PlayerBoard playerBoard, TileColor color, int min, int max, int defaultValue)
            {
                ScoreTileSelectionUI scoreTileSelectionUI = Instantiate(this.scoreTileSelectionUIPrefab, this.scoreTileSelectionUIContainer.transform).GetComponent<ScoreTileSelectionUI>();
                scoreTileSelectionUI.SetAnchor(playerBoard.GetDrawnTilesContainer(color));
                scoreTileSelectionUI.SetColor(color);
                scoreTileSelectionUI.SetCounterRange(min, max);
                scoreTileSelectionUI.SetDefaultValue(defaultValue);
                scoreTileSelectionUI.gameObject.name = $"Score Selection: {color}";
                scoreTileSelectionUI.AddOnSelectionCountChangeListener(this.OnSelectionCountChange);
                this.scoreTileSelectionUIs.Add(scoreTileSelectionUI);
            }

            private void CleanupScoreSelectionUIElements()
            {
                foreach (ScoreTileSelectionUI ui in this.scoreTileSelectionUIs)
                {
                    Destroy(ui.gameObject);
                }
                this.scoreTileSelectionUIs.Clear();
                this.cancelButton.gameObject.SetActive(false);
                this.confirmButton.gameObject.SetActive(false);
                this.countNeeded = 0;
                this.selectedColor = TileColor.ONE;
            }

            private void OnCancelSelection()
            {
                this.CleanupScoreSelectionUIElements();
                this.endTurnButton.gameObject.SetActive(true);
            }

            private void OnConfirmSelection()
            {
                int countSelected = 0;
                Dictionary<TileColor, int> selectedTiles = new();
                foreach (ScoreTileSelectionUI ui in this.scoreTileSelectionUIs)
                {
                    selectedTiles[ui.GetColor()] = ui.GetSelectedCount();
                    countSelected += ui.GetSelectedCount();
                }
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
                this.endTurnButton.gameObject.SetActive(true);
            }

            private void OnSelectionCountChange(OnSelectionCountChangePayload payload)
            {
                foreach (ScoreTileSelectionUI ui in this.scoreTileSelectionUIs)
                {
                    if (payload.Color != ui.GetColor())
                    {
                        ui.SetDefaultValue(this.countNeeded - payload.Count);
                    }
                }
            }

            private void OnEndTurn()
            {
                this.endTurnButton.gameObject.SetActive(false);
                PlayerController playerController = System.Instance.GetPlayerController();
                playerController.EndPlayerScoringTurn();
            }

            private void OnOverflowSelectionCancel()
            {
                this.CleanupScoreSelectionUIElements();
                this.endTurnButton.gameObject.SetActive(true);
            }

            private void OnEarnReward(OnPlayerBoardEarnRewardPayload payload)
            {
                this.CleanupScoreSelectionUIElements();
                this.endTurnButton.gameObject.SetActive(false);
            }

            private void OnGrantReward(OnGrantRewardPayload payload)
            {
                this.CleanupScoreSelectionUIElements();
                this.endTurnButton.gameObject.SetActive(true);
            }

        }
    }
}

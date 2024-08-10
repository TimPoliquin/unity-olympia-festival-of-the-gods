using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.PlayerBoardEvents;
using UnityEngine;
using UnityEngine.UI;
using System;
using Azul.ScoreTileSelectionUIEvent;
using UnityEngine.Events;

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
            private List<ScoreTileSelectionUI> scoreTileSelectionUIs = new();

            private int countNeeded = 0;
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
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                playerBoardController.AddOnPlayerBoardScoreSpaceSelectionListener(this.OnScoreSpaceSelection);
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


            private void OnScoreSpaceSelection(OnPlayerBoardScoreSpaceSelectionPayload payload)
            {
                this.CleanupScoreSelectionUIElements();
                TileColor wildColor = System.Instance.GetRoundController().GetCurrentRound().GetWildColor();
                this.countNeeded = payload.Value;
                int numColor = payload.PlayerBoard.GetTileCount(payload.Color);
                int numWild = payload.PlayerBoard.GetTileCount(wildColor);
                int wildsNeeded = Math.Max(payload.Value - numColor, 0);
                int min;
                if (wildsNeeded > 0)
                {
                    min = Math.Max(1, payload.Value - numWild);
                }
                else
                {
                    min = Math.Max(1, Math.Min(payload.Value - numWild, payload.Value));
                }
                this.CreateScoreTileSelectionUI(
                    payload.PlayerBoard,
                    payload.Color,
                    min,
                    Math.Min(payload.Value, numColor),
                    Math.Min(numColor, payload.Value)
                );
                if (wildColor != payload.Color && payload.Value > 1 && numWild > 0)
                {
                    this.CreateScoreTileSelectionUI(payload.PlayerBoard, wildColor, wildsNeeded, Math.Min(numWild, payload.Value - 1), wildsNeeded);
                }
                this.onConfirm = payload.OnConfirm;
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
                        TilesSelected = selectedTiles
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
                // end the turn?
                this.endTurnButton.gameObject.SetActive(false);
            }
        }
    }
}

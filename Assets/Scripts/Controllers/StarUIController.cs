using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.PlayerBoardEvents;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class StarUIController : MonoBehaviour
        {
            [SerializeField] private GameObject container;
            [SerializeField] private GameObject spaceValueUIPrefab;
            [SerializeField] private GameObject scoreTileSelectionUIPrefab;
            [SerializeField] private GameObject scoreTileSelectionUIContainer;

            private List<StarSpaceUI> starSpaceUIs = new();
            private List<ScoreTileSelectionUI> scoreTileSelectionUIs = new();

            public void CreateStarSpaceUI(StarSpace starSpace)
            {
                StarSpaceUI ui = Instantiate(this.spaceValueUIPrefab, this.container.transform).GetComponent<StarSpaceUI>();
                ui.SetStarSpace(starSpace);
                ui.ScaleToTableView();
                this.starSpaceUIs.Add(ui);
            }

            public void InitializeListeners()
            {
                CameraController cameraController = System.Instance.GetCameraController();
                cameraController.AddOnFocusOnTableListener(this.OnFocusOnTable);
                cameraController.AddOnFocusOnPlayerBoardListener(this.OnFocusOnPlayerBoard);
                PlayerBoardController playerBoardController = System.Instance.GetPlayerBoardController();
                playerBoardController.AddOnPlayerBoardScoreSpaceSelectionListener(this.OnScoreSpaceSelection);
            }

            private void OnFocusOnTable()
            {
                this.CleanupScoreSelectionUIElements();
                foreach (StarSpaceUI starSpaceUI in this.starSpaceUIs)
                {
                    starSpaceUI.ScaleToTableView();
                }
            }

            private void OnFocusOnPlayerBoard()
            {
                foreach (StarSpaceUI starSpaceUI in this.starSpaceUIs)
                {
                    starSpaceUI.ScaleToPlayerBoardView();
                }
            }

            private void OnScoreSpaceSelection(OnPlayerBoardScoreSpaceSelectionPayload payload)
            {
                this.CleanupScoreSelectionUIElements();
                TileColor wildColor = System.Instance.GetRoundController().GetCurrentRound().GetWildColor();
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
                if (wildColor != payload.Color && payload.Value > 1)
                {
                    this.CreateScoreTileSelectionUI(payload.PlayerBoard, wildColor, wildsNeeded, Math.Min(numWild, payload.Value - 1), wildsNeeded);
                }
            }

            private void CreateScoreTileSelectionUI(PlayerBoard playerBoard, TileColor color, int min, int max, int defaultValue)
            {
                ScoreTileSelectionUI scoreTileSelectionUI = Instantiate(this.scoreTileSelectionUIPrefab, this.scoreTileSelectionUIContainer.transform).GetComponent<ScoreTileSelectionUI>();
                scoreTileSelectionUI.SetAnchor(playerBoard.GetDrawnTilesContainer(color));
                scoreTileSelectionUI.SetCounterRange(min, max);
                scoreTileSelectionUI.SetDefaultValue(defaultValue);
                scoreTileSelectionUI.gameObject.name = $"Score Selection: {color}";
                this.scoreTileSelectionUIs.Add(scoreTileSelectionUI);
            }

            private void CleanupScoreSelectionUIElements()
            {
                foreach (ScoreTileSelectionUI ui in this.scoreTileSelectionUIs)
                {
                    Destroy(ui.gameObject);
                }
                this.scoreTileSelectionUIs.Clear();
            }
        }

    }
}

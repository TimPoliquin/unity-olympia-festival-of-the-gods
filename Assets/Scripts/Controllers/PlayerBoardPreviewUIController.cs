using System;
using System.Collections;
using System.Collections.Generic;
using Azul.GameEvents;
using Azul.Model;
using Azul.PreviewEvents;
using Azul.RoundEvents;
using Azul.ScreenEvents;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class PlayerBoardPreviewUIController : MonoBehaviour
        {
            [Serializable]
            public struct PreviewConfig
            {
                public GameObject Container;
                public RenderTexture Texture;
            }
            [SerializeField] private Canvas playerBoardPreviewCanvas;
            [SerializeField] private List<PreviewConfig> previewConfigs;
            private static readonly string PANEL_LAYER = "preview";
            private bool isPreviewing = false;

            // Start is called before the first frame update
            void Start()
            {
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnGameSetupComplete);
                System.Instance.GetScreenController().AddOnScreenSizeChangeListener(this.OnScreenSizeChange);
            }

            public RenderTexture GetPreviewTexture(int playerNumber)
            {
                return this.previewConfigs[playerNumber].Texture;
            }

            private void OnGameSetupComplete(OnGameSetupCompletePayload payload)
            {
                System.Instance.GetRoundController().AddOnBeforeRoundStartListener(this.OnBeforeRoundStart);
                System.Instance.GetRoundController().AddOnRoundPhaseScoreListener(this.OnPhaseScore);
                System.Instance.GetRoundController().AddOnAllRoundsCompleteListener(this.OnAllRoundsComplete);
                for (int playerNumber = 0; playerNumber < payload.NumberOfPlayers; playerNumber++)
                {
                    this.CreatePreviewUI(playerNumber);
                }
                this.ShowCanvas();
            }

            private void CreatePreviewUI(int playerNumber)
            {
                PreviewConfig previewConfig = this.previewConfigs[playerNumber];
                PlayerBoardPreviewUI previewUI = System.Instance.GetPrefabFactory().CreatePlayerBoardPreviewUI(previewConfig.Container.transform);
                previewUI.SetTexture(previewConfig.Texture);
                previewUI.SetPlayerNumber(playerNumber);
                previewUI.AddOnZoomInListener(this.OnZoomIn);
            }

            private void OnBeforeRoundStart(OnBeforeRoundStartPayload payload)
            {
                this.ShowCanvas();
            }

            private void OnPhaseScore(OnRoundPhaseScorePayload payload)
            {
                this.HideCanvas();

            }

            private void OnAllRoundsComplete(OnAllRoundsCompletePayload payload)
            {
                this.HideCanvas();

            }

            private void OnZoomIn(OnZoomPayload payload)
            {
                this.isPreviewing = true;
                this.HideCanvas();
                System.Instance.GetCameraController().FocusMainCameraOnPlayerBoard(payload.PlayerNumber);
                PreviewingBannerUI previewingBannerUI = System.Instance.GetPrefabFactory().CreatePreviewBannerUI(PANEL_LAYER);
                previewingBannerUI.Show(System.Instance.GetPlayerController().GetPlayer(payload.PlayerNumber).GetPlayerName());
                previewingBannerUI.AddOnDismissListener(() =>
                {
                    this.OnZoomOut();
                    Destroy(previewingBannerUI.gameObject);
                    System.Instance.GetUIController().GetPanelManagerController().HideLayer(PANEL_LAYER);
                });
                System.Instance.GetUIController().GetPanelManagerController().ShowLayer(PANEL_LAYER);
                this.StartCoroutine(this.PauseIfNotPlayerTurn());
            }

            private void OnZoomOut()
            {
                this.isPreviewing = false;
                System.Instance.GetCameraController().FocusMainCameraOnTable();
                this.ShowCanvas();
                Time.timeScale = 1;
            }

            private IEnumerator PauseIfNotPlayerTurn()
            {
                while (this.isPreviewing)
                {
                    if (System.Instance.GetPlayerController().GetCurrentPlayer().IsAI())
                    {
                        Time.timeScale = 0;
                    }
                    yield return null;
                }
            }

            private void OnScreenSizeChange(OnScreenSizeChangePayload payload)
            {
                if (this.playerBoardPreviewCanvas.gameObject.activeInHierarchy)
                {
                    this.ShowCanvas();
                }
            }

            private void ShowCanvas()
            {
                float aspectRatio = System.Instance.GetScreenController().GetAspectRatio();
                RectTransform rectTransform = this.playerBoardPreviewCanvas.GetComponent<RectTransform>();
                if (aspectRatio >= (16f / 9f))
                {
                    rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.width * (9f / 16f) - 200f);
                }
                else
                {
                    rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, Mathf.Min(rectTransform.rect.width / aspectRatio - 220f, rectTransform.rect.width * (9f / 16f) + 200f));
                }
                this.playerBoardPreviewCanvas.gameObject.SetActive(true);
            }

            private void HideCanvas()
            {
                this.playerBoardPreviewCanvas.gameObject.SetActive(false);
            }
        }
    }
}

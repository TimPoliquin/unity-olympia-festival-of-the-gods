using System;
using System.Collections;
using System.Collections.Generic;
using Azul.GameEvents;
using Azul.Model;
using Azul.PreviewEvents;
using Azul.RoundEvents;
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
            [SerializeField] private List<PreviewConfig> previewConfigs;
            private static readonly string PANEL_LAYER = "preview";
            private bool isPreviewing = false;

            // Start is called before the first frame update
            void Start()
            {
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnGameSetupComplete);
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
                foreach (PreviewConfig config in this.previewConfigs)
                {
                    config.Container.SetActive(true);
                }
            }

            private void OnPhaseScore(OnRoundPhaseScorePayload payload)
            {
                foreach (PreviewConfig config in this.previewConfigs)
                {
                    config.Container.SetActive(false);
                }
            }

            private void OnAllRoundsComplete(OnAllRoundsCompletePayload payload)
            {
                foreach (PreviewConfig config in this.previewConfigs)
                {
                    config.Container.SetActive(false);
                }
            }

            private void OnZoomIn(OnZoomPayload payload)
            {
                this.isPreviewing = true;
                System.Instance.GetCameraController().FocusMainCameraOnPlayerBoard(payload.PlayerNumber);
                for (int idx = 0; idx < this.previewConfigs.Count; idx++)
                {
                    this.previewConfigs[idx].Container.SetActive(false);
                }
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
                for (int idx = 0; idx < this.previewConfigs.Count; idx++)
                {
                    this.previewConfigs[idx].Container.SetActive(true);
                }
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
        }
    }
}

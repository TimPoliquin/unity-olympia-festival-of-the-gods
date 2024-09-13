using System.Collections;
using System.Collections.Generic;
using Azul.GameEvents;
using Azul.Model;
using Azul.PreviewEvents;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class PlayerBoardPreviewUIController : MonoBehaviour
        {
            [SerializeField] private PlayerBoardPreviewUI previewUI;

            // Start is called before the first frame update
            void Start()
            {
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnGameSetupComplete);
                this.previewUI.gameObject.SetActive(false);
                this.previewUI.AddOnZoomInListener(this.OnZoomIn);
                this.previewUI.AddOnZoomOutListener(this.OnZoomOut);
                this.previewUI.AddOnPreviewSelectionChangeListener(this.OnPreviewChange);
            }

            private void OnGameSetupComplete(OnGameSetupCompletePayload payload)
            {
                this.previewUI.SetPlayerNames(System.Instance.GetPlayerController().GetPlayers());
                System.Instance.GetRoundController().AddOnRoundPhaseAcquireListener(this.OnPhaseAcquire);
                System.Instance.GetRoundController().AddOnRoundPhaseScoreListener(this.OnPhaseScore);
                System.Instance.GetRoundController().AddOnAllRoundsCompleteListener(this.OnAllRoundsComplete);
                System.Instance.GetPlayerController().AddOnPlayerTurnStartListener(this.OnPlayerTurnStart);
            }

            private void OnPhaseAcquire(OnRoundPhaseAcquirePayload payload)
            {
                this.previewUI.gameObject.SetActive(true);
            }

            private void OnPhaseScore(OnRoundPhaseScorePayload payload)
            {
                this.previewUI.gameObject.SetActive(false);
            }

            private void OnAllRoundsComplete(OnAllRoundsCompletePayload payload)
            {
                this.previewUI.gameObject.SetActive(false);
            }

            private void OnPlayerTurnStart(OnPlayerTurnStartPayload payload)
            {
                this.previewUI.SetActivePlayer(payload.Player.GetPlayerNumber());
            }

            private void OnZoomIn(OnZoomPayload payload)
            {
                System.Instance.GetCameraController().FocusMainCameraOnPlayerBoard(payload.PlayerNumber);
            }

            private void OnZoomOut(OnZoomPayload payload)
            {
                System.Instance.GetCameraController().FocusMainCameraOnTable();
            }

            private void OnPreviewChange(OnPreviewSelectionChangePayload payload)
            {
                System.Instance.GetCameraController().FocusPreviewCameraOnPlayerBoard(payload.PlayerNumber);
            }
        }
    }
}

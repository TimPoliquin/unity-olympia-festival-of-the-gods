using Azul.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Azul
{
    namespace Controller
    {
        public class CameraController : MonoBehaviour
        {
            [SerializeField] private Camera mainCamera;
            [SerializeField] private CameraSettings acquireSettings;
            [SerializeField] private CameraSettings scoreSettings;

            public void SetupGame()
            {
                UnityEngine.Debug.Log($"{this.scoreSettings.GetRotation().eulerAngles}");

                // nothing to do here yet
            }

            public void InitializeListeners()
            {
                PlayerController playerController = System.Instance.GetPlayerController();
                playerController.AddOnPlayerTurnStartListener(this.OnPlayerTurnStart);
            }


            private void OnPlayerTurnStart(OnPlayerTurnStartPayload payload)
            {
                UnityEngine.Debug.Log($"Player: {payload.PlayerNumber} / Phase: {payload.Phase}");
                if (payload.Phase == Phase.SCORE)
                {
                    this.FocusOnPlayerBoard(payload.PlayerNumber);
                }
                else
                {
                    this.FocusOnTable(payload.PlayerNumber);
                }
            }

            private void FocusOnPlayerBoard(int playerNumber)
            {
                PlayerBoard playerBoard = System.Instance.GetPlayerBoardController().GetPlayerBoard(playerNumber);
                this.mainCamera.transform.position = this.scoreSettings.GetOffset() + playerBoard.transform.position;
                this.mainCamera.transform.rotation = Quaternion.Euler(90.0f, 90.0f * (playerNumber), 0);
                this.mainCamera.orthographicSize = scoreSettings.GetSize();
            }

            private void FocusOnTable(int playerNumber)
            {
                this.mainCamera.transform.SetPositionAndRotation(acquireSettings.GetOffset(), acquireSettings.GetRotation());
                this.mainCamera.orthographicSize = acquireSettings.GetSize();
                // should we rotate the camera?
            }

        }
    }
}

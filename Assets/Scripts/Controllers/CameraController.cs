using Azul.Model;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Azul
{
    namespace Controller
    {
        public class CameraController : MonoBehaviour
        {
            [SerializeField] private Camera mainCamera;
            [SerializeField] private Camera playerBoardCamera;
            [SerializeField] private CameraSettings acquireSettings;
            [SerializeField] private CameraSettings scoreSettings;
            [SerializeField] private CameraSettings previewSettings;

            private UnityEvent onFocusOnTable = new();
            private UnityEvent onFocusOnPlayerBoard = new();

            public Camera GetMainCamera()
            {
                return this.mainCamera;
            }

            public Camera GetPreviewCamera()
            {
                return this.playerBoardCamera;
            }

            public void SetupGame()
            {
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
                    this.FocusOnPlayerBoard(this.mainCamera, this.scoreSettings, payload.PlayerNumber);
                    this.playerBoardCamera.gameObject.SetActive(false);
                }
                else
                {
                    this.playerBoardCamera.gameObject.SetActive(true);
                    this.FocusOnTable(this.mainCamera);
                    this.FocusOnPlayerBoard(this.playerBoardCamera, this.previewSettings, payload.PlayerNumber);
                }
            }

            public void FocusMainCameraOnPlayerBoard(int playerNumber)
            {
                this.FocusOnPlayerBoard(this.mainCamera, this.scoreSettings, playerNumber);
            }

            public void FocusPreviewCameraOnPlayerBoard(int playerNumber)
            {
                this.FocusOnPlayerBoard(this.playerBoardCamera, this.previewSettings, playerNumber);
            }

            public void FocusMainCameraOnTable()
            {
                this.FocusOnTable(this.mainCamera);
            }

            private void FocusOnPlayerBoard(Camera camera, CameraSettings cameraSettings, int playerNumber)
            {
                PlayerController playerController = System.Instance.GetPlayerController();
                int numPlayers = playerController.GetNumberOfPlayers();
                float perPlayerRotation = 360.0f / (float)numPlayers;
                PlayerBoard playerBoard = System.Instance.GetPlayerBoardController().GetPlayerBoard(playerNumber);
                camera.transform.rotation = Quaternion.Euler(90.0f, perPlayerRotation * playerNumber, 0);
                camera.transform.position = new Vector3(playerBoard.transform.position.x, camera.transform.position.y, playerBoard.transform.position.z);
                camera.transform.position += camera.transform.right * cameraSettings.GetOffset().x;
                camera.transform.position += camera.transform.up * cameraSettings.GetOffset().z;
                camera.orthographicSize = cameraSettings.GetSize();
                this.onFocusOnPlayerBoard.Invoke();
            }

            private void FocusOnTable(Camera camera)
            {
                camera.transform.SetPositionAndRotation(acquireSettings.GetOffset(), acquireSettings.GetRotation());
                camera.orthographicSize = acquireSettings.GetSize();
                this.onFocusOnTable.Invoke();
                // should we rotate the camera?
            }

            public void AddOnFocusOnTableListener(UnityAction listener)
            {
                this.onFocusOnTable.AddListener(listener);
            }

            public void AddOnFocusOnPlayerBoardListener(UnityAction listener)
            {
                this.onFocusOnPlayerBoard.AddListener(listener);
            }

            public bool IsInView(GameObject gameObject)
            {
                Vector3 position = this.mainCamera.WorldToViewportPoint(gameObject.transform.position);
                return position.x >= 0 & position.x <= 1 && position.y >= 0 && position.y <= 1 && position.z > 0;
            }

        }
    }
}

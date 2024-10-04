using System.Collections;
using Azul.Model;
using Azul.PlayerEvents;
using Azul.RoundEvents;
using Azul.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Azul
{
    namespace Controller
    {
        public class CameraController : TimeBasedCoroutine
        {
            [SerializeField] private Camera mainCamera;
            [SerializeField] private Camera playerBoardCamera;
            [SerializeField] private CameraSettings acquireSettings;
            [SerializeField] private CameraSettings scoreSettings;
            [SerializeField] private CameraSettings previewSettings;
            [SerializeField] private CameraSettings altarSettings;

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
                playerController.AddOnBeforePlayerTurnStartListener(this.OnPlayerTurnStart);
                RoundController roundController = System.Instance.GetRoundController();
                roundController.AddOnBeforeRoundStartListener(this.OnBeforeRoundStart);
            }

            private void OnBeforeRoundStart(OnBeforeRoundStartPayload payload)
            {
                this.FocusOnTable(this.mainCamera);
            }


            private void OnPlayerTurnStart(OnBeforePlayerTurnStartPayload payload)
            {
                if (payload.Phase == Phase.SCORE)
                {
                    this.FocusOnPlayerBoard(this.mainCamera, this.scoreSettings, payload.Player.GetPlayerNumber());
                    this.playerBoardCamera.gameObject.SetActive(false);
                }
                else
                {
                    this.playerBoardCamera.gameObject.SetActive(true);
                    this.FocusOnTable(this.mainCamera);
                    this.FocusOnPlayerBoard(this.playerBoardCamera, this.previewSettings, payload.Player.GetPlayerNumber());
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

            public CoroutineResult FocusMainCameraOnAltar(Altar altar, float rotationTime)
            {
                Camera camera = this.mainCamera;
                CameraSettings cameraSettings = this.altarSettings;
                Vector3 position = altar.transform.position;
                Vector3 originalPosition = camera.transform.position;
                Quaternion originalRotation = camera.transform.rotation;
                float originalSize = camera.orthographicSize;
                Vector3 rotationDirection = cameraSettings.GetRotation().eulerAngles;
                rotationDirection.y = camera.transform.rotation.eulerAngles.y;
                Vector3 targetPosition = position + (altar.transform.forward * cameraSettings.GetOffset().z) + Vector3.up * cameraSettings.GetOffset().y;
                CoroutineResult result = this.Execute((t) =>
                {
                    UnityEngine.Debug.Log($"Camera Move: {t}/{rotationTime}");
                    camera.transform.rotation = Quaternion.Lerp(originalRotation, Quaternion.Euler(rotationDirection), t / rotationTime);
                    camera.transform.position = Vector3.Lerp(originalPosition, targetPosition, t / rotationTime);
                    camera.orthographicSize = Mathf.Lerp(originalSize, cameraSettings.GetSize(), t / rotationTime);

                }, rotationTime);
                return result;
            }

        }
    }
}

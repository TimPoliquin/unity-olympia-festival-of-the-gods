using System;
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
        [Serializable]
        public struct AspectRatio
        {
            [SerializeField] private float width;
            [SerializeField] private float height;

            public float GetAspectRatio()
            {
                return this.width / this.height;
            }
        }
        public class CameraController : TimeBasedCoroutine
        {
            [SerializeField] private Camera mainCamera;
            [SerializeField] private Camera playerBoardCamera;
            [SerializeField] private CameraSettings acquireSettings;
            [SerializeField] private CameraSettings scoreSettings;
            [SerializeField] private CameraSettings previewSettings;
            [SerializeField] private CameraSettings altarSettings;
            [SerializeField] private AspectRatio aspectRatio;

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

            void Start()
            {
                this.EnforceAspectRatio();
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
                System.Instance.GetScreenController().AddOnScreenSizeChangeListener((payload) =>
                {
                    this.EnforceAspectRatio();
                });
            }

            private void EnforceAspectRatio()
            {
                float targetAspectRatio = this.aspectRatio.GetAspectRatio();
                float windowAspectRatio = System.Instance.GetScreenController().GetAspectRatio();
                float scaleHeight = windowAspectRatio / targetAspectRatio;
                if (scaleHeight < 1.0f)
                {
                    Rect rect = this.mainCamera.rect;
                    rect.width = 1.0f;
                    rect.height = scaleHeight;
                    rect.x = 0;
                    rect.y = (1.0f - scaleHeight) / 2.0f;
                    this.mainCamera.rect = rect;
                }
                else
                {
                    float scaleWidth = 1.0f / scaleHeight;
                    Rect rect = this.mainCamera.rect;
                    rect.width = scaleWidth;
                    rect.height = 1.0f;
                    rect.x = (1.0f - scaleWidth) / 2.0f;
                    rect.y = 0;
                    this.mainCamera.rect = rect;
                }
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
                Vector3 targetRotation = this.altarSettings.GetRotation().eulerAngles;
                targetRotation.y = this.mainCamera.transform.rotation.eulerAngles.y;
                Vector3 targetPosition = altar.transform.position + (altar.transform.forward * this.altarSettings.GetOffset().z) + Vector3.up * this.altarSettings.GetOffset().y;
                return this.RefocusCameraAnimated(this.mainCamera, targetPosition, targetRotation, this.altarSettings.GetSize(), rotationTime);
            }

            public CoroutineResult AnimateFocusMainCameraOnPlayerBoard(int playerNumber, float time)
            {
                PlayerController playerController = System.Instance.GetPlayerController();
                PlayerBoard playerBoard = System.Instance.GetPlayerBoardController().GetPlayerBoard(playerNumber);
                int numPlayers = playerController.GetNumberOfPlayers();
                float perPlayerRotation = 360.0f / (float)numPlayers;
                Vector3 targetRotation = new Vector3(90.0f, perPlayerRotation * playerBoard.GetPlayerNumber(), 0);
                Vector3 targetPosition = new Vector3(playerBoard.transform.position.x, this.scoreSettings.GetOffset().y, playerBoard.transform.position.z);
                targetPosition += playerBoard.transform.right * this.scoreSettings.GetOffset().x;
                targetPosition += playerBoard.transform.forward * this.scoreSettings.GetOffset().z;
                return this.RefocusCameraAnimated(this.mainCamera, targetPosition, targetRotation, this.scoreSettings.GetSize(), time);
            }

            public CoroutineResult RefocusCameraAnimated(Camera camera, Vector3 targetPosition, Vector3 targetRotation, float targetSize, float time)
            {
                Vector3 originalPosition = camera.transform.position;
                Quaternion originalRotation = camera.transform.rotation;
                float originalSize = camera.orthographicSize;
                CoroutineResult result = this.Execute((t) =>
                {
                    camera.transform.rotation = Quaternion.Lerp(originalRotation, Quaternion.Euler(targetRotation), t / time);
                    camera.transform.position = Vector3.Lerp(originalPosition, targetPosition, t / time);
                    camera.orthographicSize = Mathf.Lerp(originalSize, targetSize, t / time);

                }, time);
                return result;
            }

        }
    }
}

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
            [SerializeField] private CameraSettings acquireSettings;
            [SerializeField] private CameraSettings scoreSettings;

            private UnityEvent onFocusOnTable = new();
            private UnityEvent onFocusOnPlayerBoard = new();

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
                PlayerController playerController = System.Instance.GetPlayerController();
                int numPlayers = playerController.GetNumberOfPlayers();
                float perPlayerRotation = 360.0f / (float)numPlayers;
                PlayerBoard playerBoard = System.Instance.GetPlayerBoardController().GetPlayerBoard(playerNumber);
                this.mainCamera.transform.rotation = Quaternion.Euler(90.0f, perPlayerRotation * playerNumber, 0);
                this.mainCamera.transform.position = new Vector3(playerBoard.transform.position.x, this.mainCamera.transform.position.y, playerBoard.transform.position.z);
                this.mainCamera.transform.position += this.mainCamera.transform.right * 20.0f;
                this.mainCamera.orthographicSize = scoreSettings.GetSize();
                this.onFocusOnPlayerBoard.Invoke();
            }

            private void FocusOnTable(int playerNumber)
            {
                this.mainCamera.transform.SetPositionAndRotation(acquireSettings.GetOffset(), acquireSettings.GetRotation());
                this.mainCamera.orthographicSize = acquireSettings.GetSize();
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
                Vector3 position = Camera.main.WorldToViewportPoint(gameObject.transform.position);
                return position.x >= 0 & position.x <= 1 && position.y >= 0 && position.y <= 1 && position.z > 0;
            }

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Model;
using Azul.OnGameEndEvents;
using Azul.Util;
using Castle.DynamicProxy;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class FinaleController : MonoBehaviour
        {

            [SerializeField] private float rotationSpeed = 5f;
            void Start()
            {
                System.Instance.GetRoundController().AddOnAllRoundsCompleteListener((payload) => this.OnAllRoundsComplete());
            }

            private void OnAllRoundsComplete()
            {
                OnGameEndPayload onGameEndPayload = this.CreateOnGameEndPayload();
                this.StartCoroutine(this.FinaleCoroutine(onGameEndPayload));
            }

            private OnGameEndPayload CreateOnGameEndPayload()
            {
                PlayerController playerController = System.Instance.GetPlayerController();
                ScoreBoardController scoreBoardController = System.Instance.GetScoreBoardController();
                List<PlayerScore> playerScores = new();
                foreach (Player player in playerController.GetPlayers())
                {
                    playerScores.Add(new()
                    {
                        Player = player,
                        Score = scoreBoardController.GetPlayerScore(player.GetPlayerNumber())
                    });
                }
                playerScores = playerScores.OrderBy(score => score.Score).Reverse().ToList();
                return new OnGameEndPayload
                {
                    Winner = playerScores[0].Player,
                    PlayerScores = playerScores
                };
            }

            private IEnumerator FinaleCoroutine(OnGameEndPayload payload)
            {
                yield return new WaitForSeconds(1.0f);
                // move camera to winning player board
                yield return this.MoveCameraToWinnerPlayerBoard(payload).WaitUntilCompleted();
                // show UI
                System.Instance.GetUIController().GetGameEndUIController().OnGameEnd(payload);
                // spin the camera
                while (true)
                {
                    yield return this.RotateCameraAroundWinnerBoard(payload).WaitUntilCompleted();
                }

            }

            private CoroutineResult MoveCameraToWinnerPlayerBoard(OnGameEndPayload payload)
            {
                PlayerBoard winnerBoard = System.Instance.GetPlayerBoardController().GetPlayerBoard(payload.Winner.GetPlayerNumber());
                return System.Instance.GetCameraController().FocusMainCameraOnPlayerBoard(winnerBoard, 1f);
            }

            private CoroutineResult RotateCameraAroundWinnerBoard(OnGameEndPayload payload)
            {
                // rotate the camera around the object
                Camera camera = System.Instance.GetCameraController().GetMainCamera();
                PlayerBoard winnerBoard = System.Instance.GetPlayerBoardController().GetPlayerBoard(payload.Winner.GetPlayerNumber());
                Vector3 center = new Vector3(winnerBoard.transform.position.x, camera.transform.position.y, winnerBoard.transform.position.z);
                float radius = Vector3.Distance(camera.transform.position, center);
                return System.Instance.GetCameraController().RotateCameraAroundPoint(center, radius, this.rotationSpeed);
            }

        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Model;
using Azul.OnGameEndEvents;
using Azul.Util;
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
                playerScores = playerScores.OrderByDescending(score => score.Score).ToList();
                return new OnGameEndPayload
                {
                    Winner = playerScores[0].Player,
                    PlayerScores = playerScores
                };
            }

            private IEnumerator FinaleCoroutine(OnGameEndPayload payload)
            {
                CoroutineResultValue<int> leaderboardRankStatus = new();
                this.UpdateLeaderboard(
                    payload.PlayerScores.Find(playerScore => playerScore.Player.GetUsername() == System.Instance.GetUsername()).Score,
                    leaderboardRankStatus);
                yield return new WaitForSeconds(.5f);
                yield return System.Instance.GetUIController().GetBlackScreenUIController().FadeToBlack(1.0f).WaitUntilCompleted();
                // wait until the leaderboard is updated, or timeout after 2s
                yield return leaderboardRankStatus.WaitUntilCompleted(2f);
                // move camera to winning player board
                yield return this.MoveCameraToWinnerPlayerBoard(payload).WaitUntilCompleted();
                yield return System.Instance.GetUIController().GetBlackScreenUIController().FadeIn(1.0f).WaitUntilCompleted();
                // show UI
                if (leaderboardRankStatus.IsCompleted() && !leaderboardRankStatus.IsError())
                {
                    payload.Rank = leaderboardRankStatus.GetValue();
                }
                else
                {
                    payload.Rank = -1;
                }
                System.Instance.GetUIController().GetGameEndUIController().ShowEndUI(payload);
                // spin the camera
                while (true)
                {
                    yield return this.RotateCameraAroundWinnerBoard(payload).WaitUntilCompleted();
                }
            }

            private CoroutineStatus MoveCameraToWinnerPlayerBoard(OnGameEndPayload payload)
            {
                PlayerBoard winnerBoard = System.Instance.GetPlayerBoardController().GetPlayerBoard(payload.Winner.GetPlayerNumber());
                return System.Instance.GetCameraController().FocusMainCameraOnPlayerBoard(winnerBoard, 1f);
            }

            private CoroutineStatus RotateCameraAroundWinnerBoard(OnGameEndPayload payload)
            {
                // rotate the camera around the object
                Camera camera = System.Instance.GetCameraController().GetMainCamera();
                PlayerBoard winnerBoard = System.Instance.GetPlayerBoardController().GetPlayerBoard(payload.Winner.GetPlayerNumber());
                Vector3 center = new Vector3(winnerBoard.transform.position.x, camera.transform.position.y, winnerBoard.transform.position.z);
                float radius = Vector3.Distance(camera.transform.position, center);
                return System.Instance.GetCameraController().RotateCameraAroundPoint(center, radius, this.rotationSpeed);
            }

            private void UpdateLeaderboard(int score, CoroutineResultValue<int> status)
            {
                ILeaderboardController leaderboardController = System.Instance.GetLeaderboardController();
                if (null != leaderboardController)
                {
                    leaderboardController.UpdateScore(score, status);
                }
                else
                {
                    UnityEngine.Debug.Log($"No leaderboard controller registered!");
                    status.Error();
                }
            }
        }
    }
}

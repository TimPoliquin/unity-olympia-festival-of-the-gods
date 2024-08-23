using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Model;
using Azul.OnGameEndEvents;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace OnGameEndEvents
    {
        public class PlayerScore
        {
            public string PlayerName { get; init; }
            public int Score { get; init; }
        }
        public class OnGameEndPayload
        {

            public List<PlayerScore> PlayerScores { get; init; }
            public string Winner { get; init; }
        }
    }
    namespace Controller
    {
        public class GameEndUIController : MonoBehaviour
        {
            [SerializeField] private GameEndUI gameEndUI;
            [SerializeField] private ScoreRowUI scoreRowUIPrefab;

            public void InitializeListeners()
            {
                System.Instance.GetRoundController().AddOnAllRoundsCompleteListener(this.OnAllRoundsComplete);
            }

            private void OnAllRoundsComplete(OnAllRoundsCompletePayload payload)
            {
                PlayerController playerController = System.Instance.GetPlayerController();
                ScoreBoardController scoreBoardController = System.Instance.GetScoreBoardController();
                List<PlayerScore> playerScores = new();
                foreach (Player player in playerController.GetPlayers())
                {
                    playerScores.Add(new()
                    {
                        PlayerName = player.GetPlayerName(),
                        Score = scoreBoardController.GetPlayerScore(player.GetPlayerNumber())
                    });
                }
                playerScores = playerScores.OrderBy(score => score.Score).Reverse().ToList();
                this.OnGameEnd(new OnGameEndPayload
                {
                    Winner = playerScores[0].PlayerName,
                    PlayerScores = playerScores
                });
            }

            public void OnGameEnd(OnGameEndPayload payload)
            {
                this.gameEndUI.SetWinnerName(payload.Winner);
                payload.PlayerScores.ForEach(playerScore =>
                {
                    this.gameEndUI.AddScoreRow(this.scoreRowUIPrefab, playerScore.PlayerName, playerScore.Score);
                });
                this.gameEndUI.gameObject.SetActive(true);
            }
        }
    }
}

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
            public Player Player { get; init; }
            public int Score { get; init; }
        }
        public class OnGameEndPayload
        {

            public List<PlayerScore> PlayerScores { get; init; }
            public Player Winner { get; init; }
        }
    }
    namespace Controller
    {
        public class GameEndUIController : MonoBehaviour
        {
            [SerializeField] private GameEndUI gameEndUI;
            [SerializeField] private ScoreRowUI scoreRowUIPrefab;

            public void OnGameEnd(OnGameEndPayload payload)
            {
                this.gameEndUI.SetWinnerName(payload.Winner.GetPlayerName());
                payload.PlayerScores.ForEach(playerScore =>
                {
                    this.gameEndUI.AddScoreRow(this.scoreRowUIPrefab, playerScore.Player.GetPlayerName(), playerScore.Score);
                });
                this.gameEndUI.gameObject.SetActive(true);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Model;
using Azul.OnGameEndEvents;
using Azul.Util;
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
            [SerializeField] private bool playOnStart = false;
            void Start()
            {
                if (this.playOnStart)
                {
                    System.Instance.GetGameController().AddOnGameSetupCompleteListener((payload) =>
                    {
                        List<Player> players = System.Instance.GetPlayerController().GetPlayers();
                        this.ShowGameEnd(new()
                        {
                            Winner = players[0],
                            PlayerScores = players.Select(player => new PlayerScore
                            {
                                Player = player,
                                Score = Random.Range(0, 200),
                            }).ToList(),
                        });
                    });
                }
            }
            public CoroutineStatus ShowGameEnd(OnGameEndPayload payload)
            {
                CoroutineStatus status = CoroutineStatus.Single();
                this.StartCoroutine(this.ShowGameEndCoroutine(payload, status));
                return status;
            }

            private IEnumerator ShowGameEndCoroutine(OnGameEndPayload payload, CoroutineStatus status)
            {
                status.Start();
                GameEndUI gameEndUI = System.Instance.GetPrefabFactory().CreateGameEndUI();
                gameEndUI.SetPlayerCount(payload.PlayerScores.Count);
                foreach (PlayerScore playerScore in payload.PlayerScores)
                {
                    this.CreatePlayerStatsUI(gameEndUI, playerScore, ReferenceEquals(payload.Winner, playerScore.Player));
                }
                yield return gameEndUI.Show().WaitUntilCompleted();
                yield return gameEndUI.ShowPlayerStats();
                yield return gameEndUI.ShowButtons();
                status.Finish();
            }


            private PlayerStatsUI CreatePlayerStatsUI(GameEndUI gameEndUI, PlayerScore playerScore, bool winner)
            {
                GameObject parent = gameEndUI.GetPlayerStatsContainer();
                PlayerStatsUI playerStatsUI = System.Instance.GetPrefabFactory().CreatePlayerStatsUI(parent.transform);
                playerStatsUI.SetPlayerName(playerScore.Player.GetPlayerName());
                playerStatsUI.SetWinner(winner);
                this.CreateAltarsStatBox(playerStatsUI, playerScore.Player.GetPlayerNumber());
                this.CreateRitualsStatBox(playerStatsUI, playerScore.Player.GetPlayerNumber());
                this.CreateScoreStatBox(playerStatsUI, playerScore.Score);
                return playerStatsUI;
            }

            private void CreateAltarsStatBox(PlayerStatsUI playerStatsUI, int playerNumber)
            {
                List<TileColor> completedAltarColors = System.Instance.GetPlayerBoardController().GetCompletedAltarColors(playerNumber);
                // List<TileColor> completedAltarColors = new() { TileColor.WILD, TileColor.RED, TileColor.PURPLE };
                playerStatsUI.ShowAltars(completedAltarColors);
            }

            private void CreateRitualsStatBox(PlayerStatsUI playerStatsUI, int playerNumber)
            {
                List<int> completedRituals = System.Instance.GetPlayerBoardController().GetCompletedRitualNumbers(playerNumber);
                // List<int> completedRituals = new() { 1, 2, 3, 4 };
                playerStatsUI.ShowRitualNumbers(completedRituals);
            }

            private void CreateScoreStatBox(PlayerStatsUI playerStatsUI, int score)
            {
                playerStatsUI.SetScore(score);
            }
        }
    }
}

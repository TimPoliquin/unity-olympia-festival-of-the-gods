using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.GameEvents;
using Azul.Model;
using Azul.Util;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class PlayerDataController : MonoBehaviour
        {
            private static string PLAYER_DATAS_FILENAME = "playerdata.dat";
            private List<PlayerData> playerDatas;
            void Start()
            {
                this.StartCoroutine(this.LoadPlayerDatas());
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnGameSetupComplete);
            }

            private void OnGameSetupComplete(OnGameSetupCompletePayload payload)
            {
                System.Instance.GetRoundController().AddOnAllRoundsCompleteListener(this.OnAllRoundsComplete);
            }

            private void OnAllRoundsComplete(OnAllRoundsCompletePayload payload)
            {
                this.UpdatePlayerScores();
                this.SavePlayerData();
            }

            public void SavePlayerData()
            {
                System.Instance.GetFileController().WriteFile(PLAYER_DATAS_FILENAME, new PlayerDataList { playerData = this.playerDatas });
            }

            private IEnumerator LoadPlayerDatas()
            {
                CoroutineResultValue<PlayerDataList> result = System.Instance.GetFileController().ReadFile<PlayerDataList>(PLAYER_DATAS_FILENAME);
                yield return result.WaitUntilCompleted();
                if (null != result.GetValue())
                {
                    this.playerDatas = result.GetValue().playerData;
                }
                else
                {
                    this.playerDatas = new();
                }
            }

            private void UpdatePlayerScores()
            {
                List<Player> players = System.Instance.GetPlayerController().GetPlayers().Where(player => player.GetUsername() != null).ToList();
                foreach (Player player in players)
                {
                    int score = System.Instance.GetScoreBoardController().GetPlayerScore(player.GetPlayerNumber());
                    PlayerData playerData = this.GetOrCreatePlayerData(player.GetUsername());
                    playerData.AddScore(score);
                }
            }

            private PlayerData GetOrCreatePlayerData(string username)
            {
                PlayerData playerData = this.playerDatas.Find(playerData => playerData.PlayerName == username);
                if (null == playerData)
                {
                    playerData = PlayerData.Create(username);
                    this.playerDatas.Add(playerData);
                }
                return playerData;
            }

            public bool IsNewPlayer()
            {
                return this.GetOrCreatePlayerData(System.Instance.GetUsername()).IsNewPlayer();
            }

            public int GetPlayerHighScore(string username)
            {
                List<int> scores = this.GetOrCreatePlayerData(username).Scores;
                if (scores.Count > 0)
                {
                    return scores.OrderByDescending(score => score).ToList()[0];
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}

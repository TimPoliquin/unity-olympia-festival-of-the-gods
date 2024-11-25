using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Azul.Util;
using Codice.Client.Commands;




#if !DISABLESTEAMWORKS
using Steamworks;
#endif

namespace Azul
{
    namespace Controller
    {
        public interface ILeaderboardController
        {
            public void UpdateScore(int score, CoroutineResultValue<int> status);
        }
        public class LeaderboardController : MonoBehaviour, ILeaderboardController
        {
#if !DISABLESTEAMWORKS
            [SerializeField] private string leaderboardName = "High Scores";
            private SteamLeaderboard_t leaderboard;
            private bool leaderboardFound = false;
            private readonly CallResult<LeaderboardFindResult_t> findCallback = new();
            private readonly CallResult<LeaderboardScoreUploaded_t> uploadCallback = new();

            void Awake()
            {
                SteamManager.Instance.AddOnInitListener(this.OnSteamInitialized);
            }

            public void OnSteamInitialized()
            {
                if (!SteamManager.Initialized)
                {
                    return;
                }
                UnityEngine.Debug.Log($"Loading leaderboard: {this.leaderboardName}");
                SteamAPICall_t findLeaderboard = SteamUserStats.FindLeaderboard(this.leaderboardName);
                findCallback.Set(findLeaderboard, this.OnLeaderboardFound);
                SteamAPI.RunCallbacks();
            }

            public void UpdateScore(int score, CoroutineResultValue<int> status)
            {
                if (!SteamManager.Initialized)
                {
                    UnityEngine.Debug.Log($"Steam manager not initialized!");
                    status.Error();
                }
                else if (!this.leaderboardFound)
                {
                    UnityEngine.Debug.Log($"Leaderboard not found - cannot update score!");
                    status.Error();
                }
                else
                {
                    status.Start();
                    SteamAPICall_t hSteamAPICall = SteamUserStats.UploadLeaderboardScore(this.leaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, score, null, 0);
                    this.uploadCallback.Set(hSteamAPICall, (result, error) => this.OnLeaderboardUploaded(result, error, status));
                    SteamAPI.RunCallbacks();
                }
            }

            private void OnLeaderboardFound(LeaderboardFindResult_t result, bool bIOFailure)
            {
                if (!bIOFailure)
                {
                    this.leaderboard = result.m_hSteamLeaderboard;
                    this.leaderboardFound = true;
                    System.Instance.RegisterLeaderboardController(this);
                    UnityEngine.Debug.Log($"Leaderboard {this.leaderboardName} found!");
                }
                else
                {
                    UnityEngine.Debug.Log($"Encountered error finding leaderboard.");
                }
            }

            private void OnLeaderboardUploaded(LeaderboardScoreUploaded_t result, bool bIOFailure, CoroutineResultValue<int> status)
            {
                if (!bIOFailure)
                {
                    status.Finish(result.m_nGlobalRankNew);
                    UnityEngine.Debug.Log($"Successfully updated leaderboard: {result.m_nScore}|{result.m_nGlobalRankPrevious}->{result.m_nGlobalRankNew}");
                }
                else
                {
                    status.Error();
                    UnityEngine.Debug.Log($"Failed to update steam leaderboard!");
                }
            }

#endif
        }
    }
}

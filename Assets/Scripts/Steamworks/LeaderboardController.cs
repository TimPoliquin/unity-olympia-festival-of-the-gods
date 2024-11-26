using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Azul.Util;
using Azul.Model;


#if !DISABLESTEAMWORKS
using Steamworks;
#endif

namespace Azul
{
    namespace Controller
    {
        public interface ILeaderboardController
        {
            public void UpdateScore(int score, CoroutineResultValue<LeaderboardRanking> status);
        }
        public class LeaderboardController : MonoBehaviour, ILeaderboardController
        {
#if !DISABLESTEAMWORKS
            [SerializeField] private System registrar;
            [SerializeField] private string leaderboardName = "High Scores";
            [SerializeField] private bool pushRankingOnReady = false;
            private SteamLeaderboard_t leaderboard;
            private bool leaderboardFound = false;
            private readonly CallResult<LeaderboardFindResult_t> findCallback = new();
            private readonly CallResult<LeaderboardScoreUploaded_t> uploadCallback = new();
            private readonly CallResult<LeaderboardScoresDownloaded_t> downloadCallback = new();

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

            public void UpdateScore(int score, CoroutineResultValue<LeaderboardRanking> status)
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
                    this.registrar.RegisterLeaderboardController(this);
                    UnityEngine.Debug.Log($"Leaderboard {this.leaderboardName} found!");
                    if (this.pushRankingOnReady)
                    {
                        this.UpdateScore(0, new CoroutineResultValue<LeaderboardRanking>());
                    }
                }
                else
                {
                    UnityEngine.Debug.Log($"Encountered error finding leaderboard.");
                }
            }

            public void FindCurrentRanking(CoroutineResultValue<LeaderboardRanking> status)
            {
                status.Start();
                SteamAPICall_t hSteamAPICall = SteamUserStats.DownloadLeaderboardEntriesForUsers(this.leaderboard, new CSteamID[1] { SteamUser.GetSteamID() }, 1);
                this.downloadCallback.Set(hSteamAPICall, (result, bIOFailure) => this.OnDownloadRanking(result, bIOFailure, status));
            }

            private void OnDownloadRanking(LeaderboardScoresDownloaded_t result, bool bIOFailure, CoroutineResultValue<LeaderboardRanking> status)
            {
                if (!bIOFailure)
                {
                    if (result.m_cEntryCount > 0)
                    {
                        LeaderboardEntry_t leaderboardEntry;
                        SteamUserStats.GetDownloadedLeaderboardEntry(result.m_hSteamLeaderboardEntries, 0, out leaderboardEntry, null, 0);
                        // player already exists on the leaderboard
                        status.Finish(new LeaderboardRanking
                        {
                            Score = leaderboardEntry.m_nScore,
                            Ranking = leaderboardEntry.m_nGlobalRank
                        });
                        UnityEngine.Debug.Log($"LeaderboardController: Ranking retrieved: {status.GetValue()}");
                    }
                    else
                    {
                        // player does not exist on the leaderboard
                        status.Finish(null);
                        UnityEngine.Debug.Log($"LeaderboardController: Player not found on the leaderboard");
                    }
                }
                else
                {
                    status.Error();
                    UnityEngine.Debug.Log("Failed to get leaderboard results!");
                }
            }

            private void OnLeaderboardUploaded(LeaderboardScoreUploaded_t result, bool bIOFailure, CoroutineResultValue<LeaderboardRanking> status)
            {
                if (!bIOFailure)
                {
                    UnityEngine.Debug.Log($"LeaderboardController: Successfully updated leaderboard: {result.m_nScore}|{result.m_nGlobalRankPrevious}->{result.m_nGlobalRankNew}");
                    if (result.m_bScoreChanged == 0 || result.m_nGlobalRankNew <= 0)
                    {
                        UnityEngine.Debug.Log($"LeaderboardController: Score did not change leaderboard entry. Finding ranking...");
                        this.FindCurrentRanking(status);
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Leaderboard updated with new ranking!");
                        status.Finish(new LeaderboardRanking
                        {
                            Score = result.m_nScore,
                            Ranking = result.m_nGlobalRankNew
                        });

                    }
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

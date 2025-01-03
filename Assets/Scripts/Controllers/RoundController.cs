using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Controller.TableEvents;
using Azul.Model;
using Azul.PlayerEvents;
using Azul.RoundEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace RoundEvents
    {
        public struct OnBeforeRoundStartPayload
        {
            public int RoundNumber { get; init; }
            public TileColor WildColor { get; init; }
            public Action Next { get; init; }
        }
    }

    namespace Controller
    {
        public class RoundController : MonoBehaviour
        {
            private int currentRound = 0;
            [SerializeField][Range(1, 6)] private int totalRounds = 6;
            [SerializeField] private List<TileColor> wildColors;
            [SerializeField] private List<Round> rounds;
            [SerializeField] private UnityEvent<OnBeforeRoundStartPayload> onBeforeRoundStart = new();
            [SerializeField] private UnityEvent<OnRoundPhasePreparePayload> onRoundPhasePrepare;
            [SerializeField] private UnityEvent<OnRoundPhaseAcquirePayload> onRoundPhaseAcquire;
            [SerializeField] private UnityEvent<OnRoundPhaseScorePayload> onRoundPhaseScore;
            [SerializeField] private UnityEvent<OnAllRoundsCompletePayload> onAllRoundsComplete;

            public void SetupGame()
            {
                this.rounds = new();
                for (int idx = 0; idx < this.totalRounds; idx++)
                {
                    this.rounds.Add(this.CreateRound(idx));
                }
            }

            public void InitializeListeners()
            {
                TableController tableController = System.Instance.GetTableController();
                tableController.AddOnTableDrawCompleteListener(this.OnDrawComplete);
                FactoryController factoryController = System.Instance.GetFactoryController();
                factoryController.AddOnFactoryDrawCompleteListener(this.OnDrawComplete);
                PlayerController playerController = System.Instance.GetPlayerController();
                playerController.AddOnAllPlayersTurnScoreFinished(this.OnAllPlayersScoreTurnFinished);
            }

            public Round GetCurrentRound()
            {
                return this.rounds[this.currentRound];
            }

            public Phase GetCurrentPhase()
            {
                return this.GetCurrentRound().GetCurrentPhase();
            }

            public bool IsCurrentPhaseAcquire()
            {
                if (this.IsAfterLastRound())
                {
                    return false;
                }
                else
                {
                    return this.GetCurrentPhase() == Phase.ACQUIRE;
                }
            }

            public bool IsCurrentPhaseScore()
            {
                if (this.IsAfterLastRound())
                {
                    return false;
                }
                else
                {
                    return this.GetCurrentPhase() == Phase.SCORE;
                }
            }

            public TileColor GetCurrentWild()
            {
                return this.rounds[this.currentRound].GetWildColor();
            }

            public TileColor GetNextWild()
            {
                if (this.IsLastRound())
                {
                    throw new IndexOutOfRangeException("There is no next round");
                }
                return this.rounds[this.currentRound + 1].GetWildColor();
            }


            private void OnDrawComplete()
            {
                this.CheckAcquirePhaseComplete();
            }

            private void OnAllPlayersScoreTurnFinished(OnAllPlayersTurnScoreFinishedPayload payload)
            {
                this.NextPhase();
            }

            private Round CreateRound(int idx)
            {
                GameObject gameObject = new GameObject($"Round {idx + 1}");
                gameObject.transform.SetParent(this.transform);
                Round round = gameObject.AddComponent<Round>();
                round.SetRoundNumber(idx);
                round.SetWildColor(this.wildColors[idx]);
                round.enabled = false;
                return round;
            }

            public void StartRound()
            {
                if (this.currentRound == 0)
                {
                    this.onRoundPhasePrepare.Invoke(new OnRoundPhasePreparePayload
                    {
                        RoundNumber = this.currentRound,
                        WildColor = this.GetCurrentWild()
                    });
                }
                this.onBeforeRoundStart.Invoke(new OnBeforeRoundStartPayload
                {
                    RoundNumber = this.currentRound,
                    WildColor = this.GetCurrentWild(),
                    Next = () =>
                    {
                        this.StartPhase();
                    }
                });

            }

            public void NextPhase()
            {
                Round currentRound = this.rounds[this.currentRound];
                Phase currentPhase = currentRound.GetCurrentPhase();
                Phase nextPhase;
                switch (currentPhase)
                {
                    case Phase.ACQUIRE:
                        nextPhase = Phase.SCORE;
                        break;
                    case Phase.SCORE:
                        nextPhase = Phase.PREPARE;
                        break;
                    case Phase.PREPARE:
                    default:
                        nextPhase = Phase.COMPLETE;
                        break;
                }
                currentRound.SetPhase(nextPhase);
                this.StartPhase();
            }

            private void StartPhase()
            {
                Round currentRound = this.rounds[this.currentRound];
                switch (currentRound.GetCurrentPhase())
                {
                    case Phase.ACQUIRE:
                        UnityEngine.Debug.Log($"Round {this.currentRound + 1} Phase Acquire");
                        this.onRoundPhaseAcquire.Invoke(new OnRoundPhaseAcquirePayload
                        {
                            RoundNumber = currentRound.GetRoundNumber(),
                            WildColor = currentRound.GetWildColor()
                        });
                        break;
                    case Phase.SCORE:
                        UnityEngine.Debug.Log($"Round {this.currentRound + 1} Phase Score");
                        this.onRoundPhaseScore.Invoke(new OnRoundPhaseScorePayload
                        {
                            RoundNumber = currentRound.GetRoundNumber()
                        });
                        break;
                    case Phase.PREPARE:
                        UnityEngine.Debug.Log($"Round {this.currentRound + 1} Phase Prepare");
                        this.onRoundPhasePrepare.Invoke(new OnRoundPhasePreparePayload
                        {
                            RoundNumber = this.currentRound,
                            WildColor = this.GetCurrentWild(),
                        });
                        this.NextPhase();
                        break;
                    case Phase.COMPLETE:
                        this.FinishRound();
                        break;
                }
            }

            private void FinishRound()
            {
                this.rounds[this.currentRound].enabled = false;
                this.currentRound++;
                if (this.currentRound < this.totalRounds)
                {
                    this.rounds[this.currentRound].enabled = true;
                    this.rounds[this.currentRound].SetPhase(Phase.ACQUIRE);
                    this.StartRound();
                }
                else
                {
                    this.onAllRoundsComplete.Invoke(new OnAllRoundsCompletePayload { });
                }
            }

            private void CheckAcquirePhaseComplete()
            {
                if (this.IsCurrentPhaseAcquire())
                {
                    if (System.Instance.GetTableController().IsCompletelyEmpty())
                    {
                        this.NextPhase();
                    }
                }
            }

            public void AddOnBeforeRoundStartListener(UnityAction<OnBeforeRoundStartPayload> listener)
            {
                this.onBeforeRoundStart.AddListener(listener);
            }

            public void AddOnRoundPhasePrepareListener(UnityAction<OnRoundPhasePreparePayload> listener)
            {
                this.onRoundPhasePrepare.AddListener(listener);
            }

            public void AddOnRoundPhaseAcquireListener(UnityAction<OnRoundPhaseAcquirePayload> listener)
            {
                this.onRoundPhaseAcquire.AddListener(listener);
            }
            public void AddOnRoundPhaseScoreListener(UnityAction<OnRoundPhaseScorePayload> listener)
            {
                this.onRoundPhaseScore.AddListener(listener);
            }

            public void AddOnAllRoundsCompleteListener(UnityAction<OnAllRoundsCompletePayload> listener)
            {
                this.onAllRoundsComplete.AddListener(listener);
            }

            public bool IsLastRound()
            {
                return this.currentRound == this.rounds.Count - 1;
            }

            public bool IsAfterLastRound()
            {
                return this.currentRound >= this.rounds.Count;
            }

        }
    }
}

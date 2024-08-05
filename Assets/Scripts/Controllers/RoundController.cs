using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace Controller
    {
        public class RoundController : MonoBehaviour
        {
            private int currentRound = 0;
            [SerializeField][Range(1, 6)] private int totalRounds = 6;
            [SerializeField] private List<TileColor> wildColors;
            [SerializeField] private List<Round> rounds;
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
                        RoundNumber = this.currentRound
                    });
                }
                this.StartPhase();

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
                        this.onRoundPhaseAcquire.Invoke(new OnRoundPhaseAcquirePayload
                        {
                            RoundNumber = currentRound.GetRoundNumber(),
                            WildColor = currentRound.GetWildColor()
                        });
                        break;
                    case Phase.SCORE:
                        this.onRoundPhaseScore.Invoke(new OnRoundPhaseScorePayload
                        {
                            RoundNumber = currentRound.GetRoundNumber()
                        });
                        break;
                    case Phase.PREPARE:
                        this.onRoundPhasePrepare.Invoke(new OnRoundPhasePreparePayload
                        {
                            RoundNumber = this.currentRound
                        });
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
                if (this.currentRound <= this.totalRounds)
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
        }
    }
}

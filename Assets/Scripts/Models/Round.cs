using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace Model
    {
        public enum Phase
        {
            ACQUIRE,
            SCORE,
            PREPARE,
            COMPLETE
        }

        public class OnPhaseCompletePayload
        {
            public Phase CurrentPhase { get; init; }
            public Phase NextPhase { get; init; }
        }

        public class OnRoundStartPayload
        {
            public int RoundNumber { get; init; }
            public Phase Phase { get; init; }
            public TileColor WildColor { get; init; }
        }

        public class OnRoundPhaseAcquirePayload
        {
            public int RoundNumber { get; init; }
            public readonly Phase Phase = Phase.ACQUIRE;
            public TileColor WildColor { get; init; }
        }

        public class OnRoundPhaseScorePayload
        {
            public int RoundNumber { get; init; }
            public readonly Phase Phase = Phase.SCORE;
        }

        public class OnRoundPhasePreparePayload
        {
            public int RoundNumber { get; init; }
            public readonly Phase Phase = Phase.PREPARE;
            public TileColor WildColor { get; init; }
        }

        public class OnAllRoundsCompletePayload
        {

        }

        public class Round : MonoBehaviour
        {
            [SerializeField] private int roundNumber;
            [SerializeField] private Phase currentPhase = Phase.ACQUIRE;
            [SerializeField] private TileColor wildColor;

            public int GetRoundNumber()
            {
                return this.roundNumber;
            }

            public int GetDisplayRoundNumber()
            {
                return this.roundNumber + 1;
            }

            public void SetRoundNumber(int roundNumber)
            {
                this.roundNumber = roundNumber;
            }

            public Phase GetCurrentPhase()
            {
                return this.currentPhase;
            }

            public TileColor GetWildColor()
            {
                return this.wildColor;
            }

            public void SetWildColor(TileColor wildColor)
            {
                this.wildColor = wildColor;
            }

            public void SetPhase(Phase phase)
            {
                this.currentPhase = phase;
            }

        }
    }
}

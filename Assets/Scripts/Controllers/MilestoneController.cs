using System.Collections;
using System.Linq;
using Azul.Event;
using Azul.MilestoneEvents;
using Azul.Model;
using Azul.Util;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace MilestoneEvents
    {
        public struct OnAltarMilestoneCompletedPayload
        {
            public int PlayerNumber { get; init; }
            public Altar CompletedMilestone { get; init; }
        }
        public struct OnNumberMilestoneCompletedPayload
        {
            public int PlayerNumber { get; init; }
            public int Value { get; init; }
        }
    }
    namespace Controller
    {
        [RequireComponent(typeof(PlayerBoard))]
        public class MilestoneController : MonoBehaviour
        {
            private PlayerBoard playerBoard;
            private readonly AzulEvent<OnAltarMilestoneCompletedPayload> onAltarMilestoneComplete = new();
            private readonly AzulEvent<OnNumberMilestoneCompletedPayload> onNumberMilestoneCompleted = new();

            void Start()
            {
                this.playerBoard = this.GetComponent<PlayerBoard>();
            }


            public CoroutineStatus OnStarTilePlaced(Altar altar, AltarSpace space)
            {
                var status = CoroutineStatus.Single();
                this.StartCoroutine(this.ExecuteMilestonesCompleted(this.playerBoard, altar, space, status));
                return status;
            }

            private IEnumerator ExecuteMilestonesCompleted(PlayerBoard playerBoard1, Altar altar, AltarSpace space, CoroutineStatus status)
            {
                status.Start();
                if (altar.GetFilledSpaces().Count == altar.GetNumberOfSpaces())
                {
                    yield return this.onAltarMilestoneComplete.Invoke(new OnAltarMilestoneCompletedPayload
                        {
                            PlayerNumber = this.playerBoard.GetPlayerNumber(),
                            CompletedMilestone = altar,
                        }
                    ).WaitUntilCompleted();
                }
                if (this.playerBoard.GetAltars().All(checkAltar => checkAltar.IsSpaceFilled(space.GetValue())))
                {
                    yield return this.onNumberMilestoneCompleted.Invoke(new OnNumberMilestoneCompletedPayload
                    {
                        PlayerNumber = this.playerBoard.GetPlayerNumber(),
                        Value = space.GetValue(),
                    }).WaitUntilCompleted();                
                }
                status.Finish();
            }


            public void AddOnAltarMilestoneCompleteListener(UnityAction<EventTracker<OnAltarMilestoneCompletedPayload>> listener)
            {
                this.onAltarMilestoneComplete.AddListener(listener);
            }
            public void AddOnNumberMilestoneCompleteListener(UnityAction<EventTracker<OnNumberMilestoneCompletedPayload>> listener)
            {
                this.onNumberMilestoneCompleted.AddListener(listener);
            }
        }
    }
}

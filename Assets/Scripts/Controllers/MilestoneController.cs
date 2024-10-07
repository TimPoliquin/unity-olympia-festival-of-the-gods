using System;
using System.Collections;
using System.Collections.Generic;
using Azul.GameEvents;
using Azul.MilestoneEvents;
using Azul.Model;
using Azul.PlayerBoardEvents;
using Azul.Util;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace MilestoneEvents
    {
        public struct OnMilestoneCompletePayload
        {
            public int PlayerNumber { get; init; }
            public Altar CompletedMilestone { get; init; }
            public Action Done { get; init; }
        }
    }
    namespace Controller
    {
        [RequireComponent(typeof(PlayerBoard))]
        public class MilestoneController : MonoBehaviour
        {
            private PlayerBoard playerBoard;
            private UnityEvent<OnMilestoneCompletePayload> onMilestoneComplete = new();

            void Start()
            {
                this.playerBoard = this.GetComponent<PlayerBoard>();
            }


            public CoroutineResult OnStarTilePlaced(Altar altar)
            {
                CoroutineResult result = CoroutineResult.Single();
                result.Start();
                if (altar.GetFilledSpaces().Count == altar.GetNumberOfSpaces())
                {
                    this.onMilestoneComplete.Invoke(new OnMilestoneCompletePayload
                    {
                        PlayerNumber = this.playerBoard.GetPlayerNumber(),
                        CompletedMilestone = altar,
                        Done = () => result.Finish(),
                    }
                    );
                }
                else
                {
                    result.Finish();
                }
                return result;
            }

            public void AddOnMilestoneCompleteListener(UnityAction<OnMilestoneCompletePayload> listener)
            {
                this.onMilestoneComplete.AddListener(listener);
            }
        }
    }
}

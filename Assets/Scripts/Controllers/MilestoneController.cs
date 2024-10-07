using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public struct OnAltarMilestoneCompletedPayload
        {
            public int PlayerNumber { get; init; }
            public Altar CompletedMilestone { get; init; }
            public Action Done { get; init; }
        }
        public struct OnNumberMilestoneCompletedPayload
        {
            public int PlayerNumber { get; init; }
            public int Value { get; init; }
            public Action Done { get; init; }
        }
    }
    namespace Controller
    {
        [RequireComponent(typeof(PlayerBoard))]
        public class MilestoneController : MonoBehaviour
        {
            private PlayerBoard playerBoard;
            private UnityEvent<OnAltarMilestoneCompletedPayload> onAltarMilestoneComplete = new();
            private UnityEvent<OnNumberMilestoneCompletedPayload> onNumberMilestoneCompleted = new();

            void Start()
            {
                this.playerBoard = this.GetComponent<PlayerBoard>();
            }


            public CoroutineResult OnStarTilePlaced(PlayerBoard playerBoard, Altar altar, AltarSpace space)
            {
                CoroutineResult result = CoroutineResult.Single();
                result.Start();
                if (altar.GetFilledSpaces().Count == altar.GetNumberOfSpaces())
                {
                    this.onAltarMilestoneComplete.Invoke(new OnAltarMilestoneCompletedPayload
                    {
                        PlayerNumber = this.playerBoard.GetPlayerNumber(),
                        CompletedMilestone = altar,
                        Done = () => result.Finish(),
                    }
                    );
                }
                else if (playerBoard.GetAltars().All(altar => altar.IsSpaceFilled(space.GetValue())))
                {
                    this.onNumberMilestoneCompleted.Invoke(new OnNumberMilestoneCompletedPayload
                    {
                        PlayerNumber = this.playerBoard.GetPlayerNumber(),
                        Value = space.GetValue(),
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

            public void AddOnAltarMilestoneCompleteListener(UnityAction<OnAltarMilestoneCompletedPayload> listener)
            {
                this.onAltarMilestoneComplete.AddListener(listener);
            }
            public void AddOnNumberMilestoneCompleteListener(UnityAction<OnNumberMilestoneCompletedPayload> listener)
            {
                this.onNumberMilestoneCompleted.AddListener(listener);
            }
        }
    }
}

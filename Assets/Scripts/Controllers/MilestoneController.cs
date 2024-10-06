using System.Collections;
using System.Collections.Generic;
using Azul.GameEvents;
using Azul.MilestoneEvents;
using Azul.Model;
using Azul.PlayerBoardEvents;
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
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnGameSetupComplete);
            }

            void OnGameSetupComplete(OnGameSetupCompletePayload payload)
            {
                System.Instance.GetPlayerBoardController().AddOnPlaceStarTileListener(this.OnStarTilePlaced);
            }

            void OnStarTilePlaced(OnPlayerBoardPlaceStarTilePayload payload)
            {
                if (payload.PlayerNumber != this.playerBoard.GetPlayerNumber())
                {
                    return;
                }
                if (payload.Star.GetFilledSpaces().Count == payload.Star.GetNumberOfSpaces())
                {
                    this.onMilestoneComplete.Invoke(new OnMilestoneCompletePayload
                    {
                        PlayerNumber = payload.PlayerNumber,
                        CompletedMilestone = payload.Star
                    }
                    );
                }
            }

            public void AddOnMilestoneCompleteListener(UnityAction<OnMilestoneCompletePayload> listener)
            {
                this.onMilestoneComplete.AddListener(listener);
            }
        }
    }
}

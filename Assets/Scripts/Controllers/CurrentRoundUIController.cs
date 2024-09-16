using System.Collections;
using System.Collections.Generic;
using Azul.GameEvents;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class CurrentRoundUIController : MonoBehaviour
        {
            [SerializeField] private GameObject currentRoundContainer;
            [SerializeField] private CurrentRoundUI currentRoundUI;
            void Start()
            {
                System.Instance.GetGameController().AddOnGameSetupCompleteListener((_payload) => this.InitializeListeners());
                this.currentRoundContainer.gameObject.SetActive(false);
            }

            void InitializeListeners()
            {
                RoundController roundController = System.Instance.GetRoundController();
                roundController.AddOnRoundPhaseAcquireListener(this.OnPhaseAcquire);
                roundController.AddOnAllRoundsCompleteListener(this.OnAllRoundsCompleted);
            }

            private void OnPhaseAcquire(OnRoundPhaseAcquirePayload payload)
            {
                this.currentRoundContainer.gameObject.SetActive(true);
                this.currentRoundUI.SetActiveColor(payload.WildColor);
            }

            private void OnAllRoundsCompleted(OnAllRoundsCompletePayload payload)
            {
                this.currentRoundContainer.gameObject.SetActive(false);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Azul.GameEvents;
using Azul.Model;
using Azul.RoundEvents;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class CurrentRoundUIController : MonoBehaviour
        {
            [SerializeField] private GameObject topBanner;
            [SerializeField] private GameObject footer;
            [SerializeField] private CurrentRoundUI currentRoundUI;
            void Start()
            {
                System.Instance.GetGameController().AddOnGameSetupCompleteListener((_payload) => this.InitializeListeners());
                this.topBanner.gameObject.SetActive(false);
                this.footer.gameObject.SetActive(false);
            }

            void InitializeListeners()
            {
                RoundController roundController = System.Instance.GetRoundController();
                roundController.AddOnBeforeRoundStartListener(this.OnBeforeRoundStart);
                roundController.AddOnAllRoundsCompleteListener(this.OnAllRoundsCompleted);
            }

            private void OnBeforeRoundStart(OnBeforeRoundStartPayload payload)
            {
                this.topBanner.gameObject.SetActive(true);
                if (System.Instance.GetPlayerController().GetNumberOfPlayers() > 2)
                {
                    this.footer.gameObject.SetActive(true);
                }
                this.currentRoundUI.SetActiveColor(payload.WildColor);
            }

            private void OnAllRoundsCompleted(OnAllRoundsCompletePayload payload)
            {
                this.topBanner.gameObject.SetActive(false);
                this.footer.gameObject.SetActive(false);

            }
        }
    }
}

using System;
using Azul.GameEvents;
using Azul.Model;
using Azul.PlayerEvents;
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
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnGameSetupComplete);
                this.topBanner.gameObject.SetActive(false);
                this.footer.gameObject.SetActive(false);
            }

            void OnGameSetupComplete(OnGameSetupCompletePayload payload)
            {
                this.InitializeListeners();
                if (payload.NumberOfPlayers > 2)
                {
                    this.footer.SetActive(true);
                }
                System.Instance.GetPlayerController().AddOnBeforePlayerTurnStartListener(this.OnPlayerTurnStart);
            }

            private void OnPlayerTurnStart(OnBeforePlayerTurnStartPayload payload)
            {
                this.currentRoundUI.SetCurrentPlayerName(payload.Player.GetPlayerName());
            }

            void InitializeListeners()
            {
                RoundController roundController = System.Instance.GetRoundController();
                roundController.AddOnBeforeRoundStartListener(this.OnBeforeRoundStart);
                roundController.AddOnAllRoundsCompleteListener(this.OnAllRoundsCompleted);
                roundController.AddOnRoundPhaseScoreListener(this.OnScoreStart);
            }

            private void OnBeforeRoundStart(OnBeforeRoundStartPayload payload)
            {
                this.topBanner.gameObject.SetActive(true);
                this.footer.SetActive(System.Instance.GetPlayerController().GetNumberOfPlayers() > 2);
                this.currentRoundUI.SetActiveColor(payload.WildColor);
                this.currentRoundUI.SetCurrentPlayerName(null);
            }

            private void OnAllRoundsCompleted(OnAllRoundsCompletePayload payload)
            {
                this.topBanner.gameObject.SetActive(false);
                this.footer.gameObject.SetActive(false);

            }

            private void OnScoreStart(OnRoundPhaseScorePayload arg0)
            {
                this.footer.SetActive(true);
            }
        }
    }
}

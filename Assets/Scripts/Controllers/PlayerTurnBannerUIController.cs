using System;
using System.Collections;
using System.Collections.Generic;
using Azul.GameEvents;
using Azul.Model;
using Azul.PlayerEvents;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class PlayerTurnBannerUIController : MonoBehaviour
        {
            [SerializeField] private float hideAfterSeconds = 3.0f;
            [SerializeField] private float repeatHideAfterSeconds = 1.0f;

            private Phase currentPhase = Phase.PREPARE;
            private Dictionary<int, List<Phase>> playerPhases = new();

            void Start()
            {
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnGameSetupComplete);
            }

            private void OnGameSetupComplete(OnGameSetupCompletePayload payload)
            {
                System.Instance.GetPlayerController().AddOnBeforePlayerTurnStartListener(this.OnPlayerTurnStart);
                for (int idx = 0; idx < payload.NumberOfPlayers; idx++)
                {
                    this.playerPhases[idx] = new();
                }
            }

            private void OnPlayerTurnStart(OnBeforePlayerTurnStartPayload payload)
            {
                switch (payload.Phase)
                {
                    case Phase.ACQUIRE:
                    case Phase.SCORE:
                        {
                            if (payload.Phase != this.currentPhase)
                            {
                                this.currentPhase = payload.Phase;
                            }
                            float hideDelay;
                            bool showInstructions;
                            Action callback;
                            List<Phase> shownPhases = this.playerPhases[payload.Player.GetPlayerNumber()];
                            if (shownPhases.Contains(this.currentPhase) || payload.Player.IsAI())
                            {
                                showInstructions = false;
                                hideDelay = this.repeatHideAfterSeconds;
                                callback = null;
                            }
                            else
                            {
                                showInstructions = true;
                                hideDelay = this.hideAfterSeconds;
                                shownPhases.Add(this.currentPhase);
                                callback = payload.Done;
                            }
                            PlayerTurnBannerUI playerTurnBannerUI = System.Instance.GetPrefabFactory().CreatePlayerTurnBannerUI();
                            this.StartCoroutine(this.ShowBannerForTime(playerTurnBannerUI, payload.Player, payload.Phase, hideDelay, showInstructions, callback));
                            if (!showInstructions)
                            {
                                payload.Done.Invoke();
                            }
                            break;
                        }
                    default:
                        {
                            payload.Done.Invoke();
                            break;
                        }
                }
            }

            private IEnumerator ShowBannerForTime(PlayerTurnBannerUI playerTurnBannerUI, Player player, Phase phase, float time, bool showInstructions, Action callback)
            {
                yield return playerTurnBannerUI.Show(player.GetPlayerName(), phase, showInstructions);
                yield return new WaitForSeconds(time);
                yield return playerTurnBannerUI.Hide();
                Destroy(playerTurnBannerUI.gameObject);
                if (null != callback)
                {
                    callback.Invoke();
                }
            }
        }
    }
}

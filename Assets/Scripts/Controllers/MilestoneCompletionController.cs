using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.GameEvents;
using Azul.MilestoneEvents;
using Azul.Model;
using Azul.Util;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class MilestoneCompletionController : MonoBehaviour
        {
            [SerializeField] private float cameraRotationSeconds = 1.0f;
            void Start()
            {
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnGameSetupComplete);
            }

            void OnGameSetupComplete(OnGameSetupCompletePayload payload)
            {
                System.Instance.GetPlayerBoardController().AddOnMilestoneCompleteListener(this.OnMilestoneComplete);
            }

            void OnMilestoneComplete(OnMilestoneCompletePayload payload)
            {
                this.StartCoroutine(this.PlayMilestoneCompletionEvent(payload));
            }

            void ShowMilestoneCompleteUI(int playerNumber)
            {
                System.Instance.GetPlayerBoardController().HideScoringUI(playerNumber);
                System.Instance.GetUIController().GetStarUIController().Hide();
                // TODO
            }

            IEnumerator PlayMilestoneCompletionEvent(OnMilestoneCompletePayload payload)
            {
                List<CoroutineResult> results = new()
                {
                    this.MoveCameraToAltar(payload.CompletedMilestone)
                };
                results.AddRange(this.FadeOutAltars(payload.PlayerNumber, payload.CompletedMilestone));
                yield return new WaitUntil(() => results.All(result => result.IsCompleted()));
                this.ShowMilestoneCompleteUI(payload.PlayerNumber);
            }

            CoroutineResult MoveCameraToAltar(Altar completedAltar)
            {
                return System.Instance.GetCameraController().FocusMainCameraOnAltar(completedAltar, this.cameraRotationSeconds);
            }

            List<CoroutineResult> FadeOutAltars(int playerNumber, Altar altarToKeep)
            {
                PlayerBoard playerBoard = System.Instance.GetPlayerBoardController().GetPlayerBoard(playerNumber);
                List<Altar> altarsToFade = playerBoard.GetAltars().FindAll(altar => !ReferenceEquals(altar, altarToKeep));
                List<CoroutineResult> results = new();
                foreach (Altar altar in altarsToFade)
                {
                    results.Add(altar.FadeOut(this.cameraRotationSeconds));
                }
                return results;
            }

        }
    }
}

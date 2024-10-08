using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.GameEvents;
using Azul.MilestoneEvents;
using Azul.Model;
using Azul.Util;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class MilestoneCompletionController : MonoBehaviour
        {
            [SerializeField] private float cameraRotationSeconds = 1.0f;
            [SerializeField] private float lightBloomTime = 1.0f;
            [SerializeField] private float lightBloomIntensity = 250.0f;
            [SerializeField] private float particleStartDelay = .75f;
            [SerializeField] private float bannerHoldTime = 5.0f;
            [SerializeField] private float scoreAnimateTime = 1.5f;
            [SerializeField] private List<ColoredValue<GameObject>> milestoneParticles;
            [SerializeField] private AudioSource completionSFX;

            private bool isShowingMilestoneCompletion = false;
            void Start()
            {
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnGameSetupComplete);
            }

            void OnGameSetupComplete(OnGameSetupCompletePayload payload)
            {
                System.Instance.GetPlayerBoardController().AddOnAltarMilestoneCompleteListener(this.OnAltarMilestoneComplete);
                System.Instance.GetPlayerBoardController().AddOnNumberMilestoneCompleteListener(this.OnNumberMilestoneComplete);
            }

            void OnAltarMilestoneComplete(OnAltarMilestoneCompletedPayload payload)
            {
                this.StartCoroutine(this.PlayAltarMilestoneCompletionEvent(payload));
            }

            void OnNumberMilestoneComplete(OnNumberMilestoneCompletedPayload payload)
            {
                this.StartCoroutine(this.PlayNumberMilestoneCompletionEvent(payload));
            }

            void HideScoringUI(int playerNumber)
            {
                System.Instance.GetPlayerBoardController().HideScoringUI(playerNumber);
                System.Instance.GetUIController().GetScoreTileSelectionUIController().HideEndTurnPanel();
                System.Instance.GetUIController().GetStarUIController().Hide();
            }
            void RestoreScoringUI()
            {
                System.Instance.GetPlayerController().ContinueTurn();
                System.Instance.GetUIController().GetStarUIController().Show();
                System.Instance.GetUIController().GetScoreTileSelectionUIController().ShowEndTurnPanel();
            }

            IEnumerator PlayAltarMilestoneCompletionEvent(OnAltarMilestoneCompletedPayload payload)
            {
                this.isShowingMilestoneCompletion = true;
                this.HideScoringUI(payload.PlayerNumber);
                yield return CoroutineResult.Multi(
                    this.MoveCameraToAltar(payload.CompletedMilestone),
                    this.FadeOutAltars(payload.PlayerNumber, payload.CompletedMilestone),
                    this.BloomAltarLight(payload.CompletedMilestone),
                    this.ShowCompletionParticles(payload.CompletedMilestone)
                ).WaitUntilCompleted();
                yield return this.ShowAltarCompletionUI(payload.PlayerNumber, payload.CompletedMilestone).WaitUntilCompleted();
                // restore everything to the way it was!
                yield return CoroutineResult.Multi(
                    this.MoveCameraToPlayerBoard(payload.PlayerNumber),
                    this.FadeInAltars(payload.PlayerNumber, payload.CompletedMilestone)
                ).WaitUntilCompleted();
                this.RestoreScoringUI();
                this.isShowingMilestoneCompletion = false;
                payload.Done();
            }

            IEnumerator PlayNumberMilestoneCompletionEvent(OnNumberMilestoneCompletedPayload payload)
            {
                this.isShowingMilestoneCompletion = true;
                yield return null;
                this.HideScoringUI(payload.PlayerNumber);
                // TODO just show a banner?
                yield return this.ShowNumberCompletionUI(payload.PlayerNumber, payload.Value).WaitUntilCompleted();
                this.RestoreScoringUI();
                this.isShowingMilestoneCompletion = false;
                payload.Done();
            }

            CoroutineResult MoveCameraToAltar(Altar completedAltar)
            {
                return System.Instance.GetCameraController().FocusMainCameraOnAltar(completedAltar, this.cameraRotationSeconds);
            }

            CoroutineResult MoveCameraToPlayerBoard(int playerNumber)
            {
                return System.Instance.GetCameraController().AnimateFocusMainCameraOnPlayerBoard(playerNumber, this.cameraRotationSeconds);
            }

            CoroutineResult FadeAltars(int playerNumber, Altar altarToKeep, float alpha)
            {
                PlayerBoard playerBoard = System.Instance.GetPlayerBoardController().GetPlayerBoard(playerNumber);
                List<Altar> altarsToFade = playerBoard.GetAltars().FindAll(altar => !ReferenceEquals(altar, altarToKeep));
                List<CoroutineResult> results = new();
                foreach (Altar altar in altarsToFade)
                {
                    results.Add(altar.Fade(this.cameraRotationSeconds, alpha));
                }
                return CoroutineResult.Multi(results);
            }

            CoroutineResult FadeOutAltars(int playerNumber, Altar altarToKeep)
            {
                return this.FadeAltars(playerNumber, altarToKeep, 0.0f);
            }
            CoroutineResult FadeInAltars(int playerNumber, Altar altarToKeep)
            {
                return this.FadeAltars(playerNumber, altarToKeep, 1.0f);
            }

            CoroutineResult BloomAltarLight(Altar altar)
            {
                return altar.TurnOnMilestoneCompletionLight(System.Instance.GetTileMaterialProvider().GetColor(altar.GetColor()), this.lightBloomIntensity, this.lightBloomTime);
            }

            CoroutineResult ShowCompletionParticles(Altar altar)
            {
                GameObject prefab = this.milestoneParticles.Find(particle => particle.GetTileColor() == altar.GetColor()).GetValue();
                CoroutineResult result = CoroutineResult.Single();
                this.StartCoroutine(this.CompletionParticlesCoroutine(prefab, altar.transform, result));
                return result;
            }

            CoroutineResult ShowAltarCompletionUI(int playerNumber, Altar altar)
            {
                CoroutineResult result = CoroutineResult.Single();
                this.StartCoroutine(this.ShowCompletionUICoroutine(playerNumber, altar.GetColor(), this.bannerHoldTime, result));
                return result;
            }

            CoroutineResult ShowNumberCompletionUI(int playerNumber, int ritualNumber)
            {
                CoroutineResult result = CoroutineResult.Single();
                this.StartCoroutine(this.ShowNumberCompletionUICoroutine(playerNumber, ritualNumber, this.bannerHoldTime, result));
                return result;
            }

            IEnumerator CompletionParticlesCoroutine(GameObject particlesPrefab, Transform target, CoroutineResult result)
            {
                result.Start();
                yield return new WaitForSeconds(this.particleStartDelay);
                Instantiate(particlesPrefab, target);
                result.Finish();
            }

            IEnumerator ShowCompletionUICoroutine(int playerNumber, TileColor color, float time, CoroutineResult result)
            {
                result.Start();
                MilestoneCompletedPanelUIController controller = System.Instance.GetUIController().GetMilestoneCompletedPanelUIController();
                this.completionSFX.Play();
                yield return controller.Show(color);
                yield return new WaitForSeconds(time);
                yield return controller.AnimateScore(playerNumber, this.scoreAnimateTime).WaitUntilCompleted();
                controller.Hide();
                result.Finish();
            }

            IEnumerator ShowNumberCompletionUICoroutine(int playerNumber, int ritualNumber, float time, CoroutineResult result)
            {
                result.Start();
                MilestoneCompletedPanelUIController controller = System.Instance.GetUIController().GetMilestoneCompletedPanelUIController();
                this.completionSFX.Play();
                yield return controller.Show(ritualNumber);
                yield return new WaitForSeconds(time);
                yield return controller.AnimateScore(playerNumber, this.scoreAnimateTime).WaitUntilCompleted();
                controller.Hide();
                result.Finish();
            }

            public bool IsShowingMilestoneCompletion()
            {
                return this.isShowingMilestoneCompletion;
            }
        }
    }
}

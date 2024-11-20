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
                if (System.Instance.GetPlayerController().GetCurrentPlayer().IsHuman())
                {
                    System.Instance.GetUIController().GetScoreTileSelectionUIController().ShowEndTurnPanel();
                }
            }

            IEnumerator PlayAltarMilestoneCompletionEvent(OnAltarMilestoneCompletedPayload payload)
            {
                this.isShowingMilestoneCompletion = true;
                this.HideScoringUI(payload.PlayerNumber);
                yield return CoroutineStatus.Multi(
                    this.MoveCameraToAltar(payload.CompletedMilestone),
                    this.FadeOutAltars(payload.PlayerNumber, payload.CompletedMilestone),
                    this.BloomAltarLight(payload.CompletedMilestone),
                    this.ShowCompletionParticles(payload.CompletedMilestone)
                ).WaitUntilCompleted();
                yield return this.ShowAltarCompletionUI(payload.PlayerNumber, payload.CompletedMilestone).WaitUntilCompleted();
                // restore everything to the way it was!
                yield return CoroutineStatus.Multi(
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

            CoroutineStatus MoveCameraToAltar(Altar completedAltar)
            {
                return System.Instance.GetCameraController().FocusMainCameraOnAltar(completedAltar, this.cameraRotationSeconds);
            }

            CoroutineStatus MoveCameraToPlayerBoard(int playerNumber)
            {
                return System.Instance.GetCameraController().AnimateFocusMainCameraOnPlayerBoard(playerNumber, this.cameraRotationSeconds);
            }

            CoroutineStatus FadeAltars(int playerNumber, Altar altarToKeep, float alpha)
            {
                PlayerBoard playerBoard = System.Instance.GetPlayerBoardController().GetPlayerBoard(playerNumber);
                List<Altar> altarsToFade = playerBoard.GetAltars().FindAll(altar => !ReferenceEquals(altar, altarToKeep));
                List<CoroutineStatus> results = new();
                foreach (Altar altar in altarsToFade)
                {
                    results.Add(altar.Fade(this.cameraRotationSeconds, alpha));
                }
                return CoroutineStatus.Multi(results);
            }

            CoroutineStatus FadeOutAltars(int playerNumber, Altar altarToKeep)
            {
                return this.FadeAltars(playerNumber, altarToKeep, 0.0f);
            }
            CoroutineStatus FadeInAltars(int playerNumber, Altar altarToKeep)
            {
                return this.FadeAltars(playerNumber, altarToKeep, 1.0f);
            }

            CoroutineStatus BloomAltarLight(Altar altar)
            {
                return altar.TurnOnMilestoneCompletionLight(System.Instance.GetTileMaterialProvider().GetColor(altar.GetColor()), this.lightBloomIntensity, this.lightBloomTime);
            }

            CoroutineStatus ShowCompletionParticles(Altar altar)
            {
                GameObject prefab = this.milestoneParticles.Find(particle => particle.GetTileColor() == altar.GetColor()).GetValue();
                CoroutineStatus status = CoroutineStatus.Single();
                this.StartCoroutine(this.CompletionParticlesCoroutine(prefab, altar.transform, status));
                return status;
            }

            CoroutineStatus ShowAltarCompletionUI(int playerNumber, Altar altar)
            {
                CoroutineStatus status = CoroutineStatus.Single();
                this.StartCoroutine(this.ShowCompletionUICoroutine(playerNumber, altar.GetColor(), this.bannerHoldTime, status));
                return status;
            }

            CoroutineStatus ShowNumberCompletionUI(int playerNumber, int ritualNumber)
            {
                CoroutineStatus status = CoroutineStatus.Single();
                this.StartCoroutine(this.ShowNumberCompletionUICoroutine(playerNumber, ritualNumber, this.bannerHoldTime, status));
                return status;
            }

            IEnumerator CompletionParticlesCoroutine(GameObject particlesPrefab, Transform target, CoroutineStatus status)
            {
                status.Start();
                yield return new WaitForSeconds(this.particleStartDelay);
                Instantiate(particlesPrefab, target);
                status.Finish();
            }

            IEnumerator ShowCompletionUICoroutine(int playerNumber, TileColor color, float time, CoroutineStatus status)
            {
                status.Start();
                MilestoneCompletedPanelUIController controller = System.Instance.GetUIController().GetMilestoneCompletedPanelUIController();
                int points = System.Instance.GetScoreBoardController().GetCompletionPoints(color);
                this.completionSFX.Play();
                yield return controller.Show(color);
                yield return new WaitForSeconds(time);
                yield return controller.AnimateScore(playerNumber, this.scoreAnimateTime).WaitUntilCompleted();
                System.Instance.GetScoreBoardController().AddPoints(playerNumber, points);
                controller.Hide();
                status.Finish();
            }

            IEnumerator ShowNumberCompletionUICoroutine(int playerNumber, int ritualNumber, float time, CoroutineStatus status)
            {
                status.Start();
                MilestoneCompletedPanelUIController controller = System.Instance.GetUIController().GetMilestoneCompletedPanelUIController();
                int points = System.Instance.GetScoreBoardController().GetCompletionPoints(ritualNumber);
                this.completionSFX.Play();
                yield return controller.Show(ritualNumber);
                yield return new WaitForSeconds(time);
                yield return controller.AnimateScore(playerNumber, this.scoreAnimateTime).WaitUntilCompleted();
                System.Instance.GetScoreBoardController().AddPoints(playerNumber, points);
                controller.Hide();
                status.Finish();
            }

            public bool IsShowingMilestoneCompletion()
            {
                return this.isShowingMilestoneCompletion;
            }
        }
    }
}

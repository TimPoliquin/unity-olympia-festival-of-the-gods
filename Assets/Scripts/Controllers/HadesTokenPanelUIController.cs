using System.Collections;
using System.Collections.Generic;
using Azul.Util;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class HadesTokenPanelUIController : MonoBehaviour
        {
            [SerializeField] private AudioSource hadesSFX;
            [SerializeField] private float displaySeconds = 3.5f;
            [SerializeField] private float scoreMoveTime = 1.0f;

            public CoroutineStatus Show(int playerNumber, int tileCount)
            {
                CoroutineStatus status = CoroutineStatus.Single();
                this.StartCoroutine(this.ShowCoroutine(playerNumber, tileCount, status));
                return status;
            }

            public IEnumerator ShowCoroutine(int playerNumber, int tileCount, CoroutineStatus status)
            {
                status.Start();
                this.hadesSFX.Play();
                HadesTokenPanelUI panel = System.Instance.GetPrefabFactory().CreateHadesTokenPanelUI();
                yield return panel.Show(tileCount).WaitUntilCompleted();
                if (System.Instance.GetPlayerController().GetPlayer(playerNumber).IsAI())
                {
                    yield return new WaitForSeconds(this.displaySeconds);
                    yield return this.OnDismissCoroutine(panel, playerNumber, tileCount, status);
                }
                else
                {
                    panel.AddOnDismissHandler(() => this.OnDismiss(panel, playerNumber, tileCount, status));
                }
            }

            private void OnDismiss(HadesTokenPanelUI panel, int playerNumber, int tileCount, CoroutineStatus status)
            {
                this.StartCoroutine(this.OnDismissCoroutine(panel, playerNumber, tileCount, status));
            }

            private IEnumerator OnDismissCoroutine(HadesTokenPanelUI panel, int playerNumber, int tileCount, CoroutineStatus status)
            {
                yield return panel.AnimateScoreToPoint(System.Instance.GetUIController().GetPlayerUIController().GetScoreScreenPosition(playerNumber), .5f, this.scoreMoveTime).WaitUntilCompleted();
                System.Instance.GetScoreBoardController().DeductPoints(playerNumber, tileCount);
                yield return panel.Hide().WaitUntilCompleted();
                Destroy(panel.gameObject);
                status.Finish();
            }
        }
    }
}

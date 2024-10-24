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

            public CoroutineResult Show(int playerNumber, int tileCount)
            {
                CoroutineResult result = CoroutineResult.Single();
                this.StartCoroutine(this.ShowCoroutine(playerNumber, tileCount, result));
                return result;
            }

            public IEnumerator ShowCoroutine(int playerNumber, int tileCount, CoroutineResult result)
            {
                result.Start();
                this.hadesSFX.Play();
                HadesTokenPanelUI panel = System.Instance.GetPrefabFactory().CreateHadesTokenPanelUI();
                panel.AddOnDismissHandler(() => this.OnDismiss(panel, playerNumber, tileCount, result));
                yield return panel.Show(tileCount).WaitUntilCompleted();
            }

            private void OnDismiss(HadesTokenPanelUI panel, int playerNumber, int tileCount, CoroutineResult result)
            {
                this.StartCoroutine(this.OnDismissCoroutine(panel, playerNumber, tileCount, result));
            }

            private IEnumerator OnDismissCoroutine(HadesTokenPanelUI panel, int playerNumber, int tileCount, CoroutineResult result)
            {
                yield return panel.AnimateScoreToPoint(System.Instance.GetUIController().GetPlayerUIController().GetScoreScreenPosition(playerNumber), .5f, this.scoreMoveTime).WaitUntilCompleted();
                System.Instance.GetScoreBoardController().DeductPoints(playerNumber, tileCount);
                yield return panel.Hide().WaitUntilCompleted();
                Destroy(panel.gameObject);
                result.Finish();
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Controller;
using Azul.Controller.TableUtilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class AcquireTilesPanelUI : MonoBehaviour
        {
            [SerializeField] private GameObject selectedTilesContainer;
            [SerializeField] private GameObject hadesTokenInfo;
            [SerializeField] private TextMeshProUGUI totalText;
            [SerializeField] private float delay = .1f;
            [SerializeField] private float fadeInTime = .5f;
            [SerializeField] private float fadeOutTime = .5f;
            [SerializeField] private CanvasGroup canvasGroup;
            [SerializeField] private string hadesTaxTemplate = "Hades' Tax: Lose {0} points";

            private bool hidden = true;


            public void Show(List<ColoredValue<int>> tileCounts)
            {
                PrefabFactory prefabFactory = System.Instance.GetPrefabFactory();
                int total = 0;
                this.hadesTokenInfo.SetActive(false);
                foreach (ColoredValue<int> tileCount in tileCounts)
                {
                    PlayerTileCountUI playerTileCountUI = prefabFactory.CreatePlayerTileCountUI(tileCount.GetTileColor(), this.selectedTilesContainer.transform);
                    playerTileCountUI.SetTileCount(tileCount.GetValue());
                    playerTileCountUI.SetSize(42);
                    total += tileCount.GetValue();
                    if (tileCount.GetTileColor() == TileColor.ONE)
                    {
                        this.hadesTokenInfo.SetActive(true);
                    }
                }
                this.totalText.text = string.Format(this.hadesTaxTemplate, total);
                this.hidden = false;
                this.StartCoroutine(this.FadeIn());
            }

            public void Hide()
            {
                if (this.hidden)
                {
                    return;
                }
                this.StartCoroutine(this.FadeOut());
            }

            private IEnumerator FadeIn()
            {
                this.canvasGroup.alpha = 0;
                yield return new WaitForSeconds(this.delay);
                while (!this.hidden && this.canvasGroup.alpha < 1)
                {
                    this.canvasGroup.alpha += Time.deltaTime / this.fadeInTime;
                    yield return null;
                }
            }

            private IEnumerator FadeOut()
            {
                this.hidden = true;
                while (this.hidden && this.canvasGroup.alpha > 0)
                {
                    this.canvasGroup.alpha -= Time.deltaTime / this.fadeInTime;
                    yield return null;
                }
                Destroy(this.gameObject);
            }
        }
    }
}

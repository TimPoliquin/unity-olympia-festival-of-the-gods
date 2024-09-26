using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Azul
{
    namespace Model
    {
        public class ScoringPanelUI : MonoBehaviour
        {
            [SerializeField] private GameObject godsScoringContainer;
            [SerializeField] private GameObject ritualScoringContainer;

            [SerializeField] private float transitionTime = .25f;

            private bool showing = false;

            void Awake()
            {
                RectTransform rectTransform = this.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width, rectTransform.anchoredPosition.y);
            }

            public void AddGodScoreUI(GodScoreUI godScoreUI)
            {
                godScoreUI.transform.SetParent(this.godsScoringContainer.transform);
            }

            public void AddRitualScoreUI(RitualScoreUI ritualScoreUI)
            {
                ritualScoreUI.transform.SetParent(this.ritualScoringContainer.transform);
            }

            public void Toggle()
            {
                if (this.showing)
                {
                    this.Hide();
                }
                else
                {
                    this.Show();
                }
            }

            public void Show()
            {
                if (!this.showing)
                {
                    this.gameObject.SetActive(true);
                    this.StartCoroutine(this.SlideIn());
                }
            }

            public void Hide()
            {
                if (showing)
                {
                    this.StartCoroutine(this.SlideOut(() => this.gameObject.SetActive(false)));
                }
            }

            private IEnumerator SlideIn()
            {
                this.showing = true;
                RectTransform rectTransform = this.GetComponent<RectTransform>();
                while (this.showing && rectTransform.anchoredPosition.x > 0)
                {
                    float x = Mathf.Max(0, rectTransform.anchoredPosition.x + -1 * rectTransform.rect.width * Time.deltaTime / this.transitionTime);
                    rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
                    yield return null;
                }
            }

            private IEnumerator SlideOut(Action callback = null)
            {
                this.showing = false;
                RectTransform rectTransform = this.GetComponent<RectTransform>();
                while (!this.showing && rectTransform.anchoredPosition.x < rectTransform.rect.width)
                {
                    float x = Mathf.Min(rectTransform.anchoredPosition.x + rectTransform.rect.width * Time.deltaTime / this.transitionTime, rectTransform.rect.width);
                    rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
                    yield return null;
                }
                if (callback != null && !this.showing)
                {
                    callback.Invoke();
                }
            }
        }
    }
}

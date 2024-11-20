using System.Collections;
using System.Collections.Generic;
using Azul.Util;
using UnityEngine;

namespace Azul
{
    namespace Animation
    {

    }
    public class SlideIn : MonoBehaviour
    {
        [SerializeField] private float transitionTime = .25f;

        private bool showing = false;

        void Awake()
        {
            RectTransform rectTransform = this.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(rectTransform.rect.width, rectTransform.anchoredPosition.y);
        }

        public CoroutineStatus Toggle()
        {
            if (this.showing)
            {
                return this.Hide();
            }
            else
            {
                return this.Show();
            }
        }

        public CoroutineStatus Show()
        {
            CoroutineStatus status = CoroutineStatus.Single();
            if (!this.showing)
            {
                this.gameObject.SetActive(true);
                this.StartCoroutine(this.SlideInCoroutine(status));
            }
            else
            {
                status.Finish();
            }
            return status;
        }

        public CoroutineStatus Hide()
        {
            CoroutineStatus status = CoroutineStatus.Single();
            if (showing)
            {
                this.StartCoroutine(this.SlideOut(status));
            }
            else
            {
                status.Finish();
            }
            return status;
        }

        public bool IsShowing()
        {
            return this.showing;
        }

        private IEnumerator SlideInCoroutine(CoroutineStatus status)
        {
            UnityEngine.Debug.Log("Showing help panel!");
            status.Start();
            this.showing = true;
            RectTransform rectTransform = this.GetComponent<RectTransform>();
            while (this.showing && rectTransform.anchoredPosition.x > 0)
            {
                float x = Mathf.Max(0, rectTransform.anchoredPosition.x + -1 * rectTransform.rect.width * Time.deltaTime / this.transitionTime);
                rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
                yield return null;
            }
            status.Finish();
        }

        private IEnumerator SlideOut(CoroutineStatus status)
        {
            status.Start();
            this.showing = false;
            RectTransform rectTransform = this.GetComponent<RectTransform>();
            while (!this.showing && rectTransform.anchoredPosition.x < rectTransform.rect.width)
            {
                float x = Mathf.Min(rectTransform.anchoredPosition.x + rectTransform.rect.width * Time.deltaTime / this.transitionTime, rectTransform.rect.width);
                rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
                yield return null;
            }
            status.Finish();
        }
    }
}

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

        public CoroutineResult Toggle()
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

        public CoroutineResult Show()
        {
            CoroutineResult result = CoroutineResult.Single();
            if (!this.showing)
            {
                this.gameObject.SetActive(true);
                this.StartCoroutine(this.SlideInCoroutine(result));
            }
            else
            {
                result.Finish();
            }
            return result;
        }

        public CoroutineResult Hide()
        {
            CoroutineResult result = CoroutineResult.Single();
            if (showing)
            {
                this.StartCoroutine(this.SlideOut(result));
            }
            else
            {
                result.Finish();
            }
            return result;
        }

        public bool IsShowing()
        {
            return this.showing;
        }

        private IEnumerator SlideInCoroutine(CoroutineResult result)
        {
            UnityEngine.Debug.Log("Showing help panel!");
            result.Start();
            this.showing = true;
            RectTransform rectTransform = this.GetComponent<RectTransform>();
            while (this.showing && rectTransform.anchoredPosition.x > 0)
            {
                float x = Mathf.Max(0, rectTransform.anchoredPosition.x + -1 * rectTransform.rect.width * Time.deltaTime / this.transitionTime);
                rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
                yield return null;
            }
            result.Finish();
        }

        private IEnumerator SlideOut(CoroutineResult result)
        {
            result.Start();
            this.showing = false;
            RectTransform rectTransform = this.GetComponent<RectTransform>();
            while (!this.showing && rectTransform.anchoredPosition.x < rectTransform.rect.width)
            {
                float x = Mathf.Min(rectTransform.anchoredPosition.x + rectTransform.rect.width * Time.deltaTime / this.transitionTime, rectTransform.rect.width);
                rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
                yield return null;
            }
            result.Finish();
        }
    }
}

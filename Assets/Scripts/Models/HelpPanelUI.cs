using System.Collections;
using System.Collections.Generic;
using Azul.Animation;
using Azul.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Azul
{
    namespace Model
    {
        [RequireComponent(typeof(Fade))]
        public class HelpPanelUI : MonoBehaviour
        {
            [SerializeField] TextMeshProUGUI slideTitleText;
            [SerializeField] Slider slider;
            [SerializeField] private ButtonUI previousButton;
            [SerializeField] private ButtonUI nextButton;
            [SerializeField] private ButtonUI closeButton;
            [SerializeField] private List<SlideUI> slides;
            private Fade fade;

            private UnityEvent onClose = new();

            void Awake()
            {
                this.previousButton.AddOnClickListener(this.OnPrevious);
                this.nextButton.AddOnClickListener(this.OnNext);
                this.closeButton.AddOnClickListener(() =>
                {
                    this.onClose.Invoke();
                    this.closeButton.SetInteractable(false);
                });
                this.slider.onValueChanged.AddListener((val) =>
                {
                    this.ShowSlide();
                });
                this.fade = this.GetComponent<Fade>();
                this.slider.minValue = 0;
                this.slider.maxValue = this.slides.Count - 1;
            }

            public void OnNext()
            {
                this.slider.value = this.slider.value + 1;
                this.ShowSlide();
            }

            public void OnPrevious()
            {
                this.slider.value = this.slider.value - 1;
                this.ShowSlide();
            }

            public CoroutineStatus Show()
            {
                this.slider.value = 0;
                this.ShowSlide();
                this.fade.StartHidden();
                return this.fade.Show();
            }

            public CoroutineStatus Hide()
            {
                return this.fade.Hide();
            }

            public void AddOnCloseClickListener(UnityAction listener)
            {
                this.onClose.AddListener(listener);
            }

            private void ShowSlide()
            {
                for (int idx = 0; idx < this.slides.Count; idx++)
                {
                    this.slides[idx].SetActive(idx == this.slider.value);
                }
                this.slideTitleText.text = this.slides[(int)this.slider.value].GetTitle();
                this.previousButton.SetInteractable(this.slider.value > 0);
                this.nextButton.SetInteractable(this.slider.value < this.slides.Count - 1);
            }
        }
    }
}

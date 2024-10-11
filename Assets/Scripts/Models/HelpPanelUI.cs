using System.Collections;
using System.Collections.Generic;
using Azul.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Azul
{
    namespace Model
    {
        [RequireComponent(typeof(SlideIn))]
        public class HelpPanelUI : MonoBehaviour
        {
            [SerializeField] private Button closeButton;

            private UnityEvent onClose = new();
            private SlideIn slideIn;

            void Awake()
            {
                this.slideIn = this.GetComponent<SlideIn>();
                this.closeButton.onClick.AddListener(() =>
                {
                    this.onClose.Invoke();
                    this.closeButton.interactable = false;
                });
            }

            public CoroutineResult Show()
            {
                return this.slideIn.Show();
            }

            public CoroutineResult Hide()
            {
                return this.slideIn.Hide();
            }

            public void AddOnCloseClickListener(UnityAction listener)
            {
                this.onClose.AddListener(listener);
            }
        }
    }
}
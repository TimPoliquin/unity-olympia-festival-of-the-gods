using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Azul
{
    namespace Model
    {
        public class PreviewingBannerUI : MonoBehaviour
        {
            [SerializeField] private string titleTemplate = "Previewing {0}'s Board";
            [SerializeField] private TextMeshProUGUI titleText;
            [SerializeField] private Button dismissButton;

            private UnityEvent onDismiss = new();

            void Awake()
            {
                this.dismissButton.onClick.AddListener(this.OnDismiss);
            }

            public void Show(string playerName)
            {
                this.titleText.text = string.Format(this.titleTemplate, playerName);
            }

            private void OnDismiss()
            {
                this.onDismiss.Invoke();
            }

            public void AddOnDismissListener(UnityAction listener)
            {
                this.onDismiss.AddListener(listener);
            }
        }
    }
}

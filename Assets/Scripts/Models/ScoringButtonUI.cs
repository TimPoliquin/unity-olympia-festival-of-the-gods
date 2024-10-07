using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Azul
{
    namespace Model
    {
        public class ScoringButtonUI : MonoBehaviour
        {
            [SerializeField] private Button button;
            [SerializeField] private Image focus;

            public void ShowFocus()
            {
                this.focus.gameObject.SetActive(true);
            }

            public void HideFocus()
            {
                this.focus.gameObject.SetActive(false);
            }

            public void AddOnClickListener(UnityAction listener)
            {
                this.button.onClick.AddListener(listener);
            }
        }

    }
}

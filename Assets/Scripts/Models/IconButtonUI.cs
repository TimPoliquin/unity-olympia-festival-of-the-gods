using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Azul
{
    namespace Model
    {
        public class IconButtonUI : IconUI
        {
            [SerializeField] private Button button;
            [SerializeField] private Image selectedOverlay;

            public void AddOnClickListener(UnityAction listener)
            {
                this.button.onClick.AddListener(listener);
            }

            public void SetSelected(bool selected)
            {
                this.selectedOverlay.gameObject.SetActive(selected);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace Model
    {
        public class FirstTimePlayerPanelUI : MonoBehaviour
        {
            [SerializeField] private ButtonUI noButton;
            [SerializeField] private ButtonUI yesButton;

            public void AddOnNoListener(UnityAction listener)
            {
                this.noButton.AddOnClickListener(listener);
            }
            public void AddOnYesListener(UnityAction listener)
            {
                this.yesButton.AddOnClickListener(listener);
            }
        }
    }
}

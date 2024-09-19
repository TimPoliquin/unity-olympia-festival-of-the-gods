using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Azul
{
    namespace Model
    {
        public class EndTurnPanelUI : MonoBehaviour
        {
            [SerializeField] PanelUI panel;
            [SerializeField] Button endTurnButton;

            private UnityEvent onEndTurn = new();

            private void Start()
            {
                this.endTurnButton.onClick.AddListener(this.OnEndTurn);
            }

            public void Show()
            {
                this.panel.Show();
            }

            public void Hide()
            {
                this.panel.Hide();
            }

            public void AddOnEndTurnListener(UnityAction listener)
            {
                this.onEndTurn.AddListener(listener);
            }

            private void OnEndTurn()
            {
                this.onEndTurn.Invoke();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.WildColorSelectionEvents;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Azul
{
    namespace WildColorSelectionEvents
    {
        public class OnWildColorSelectedPayload
        {
            public TileColor Color { get; init; }
        }
    }
    namespace Model
    {
        public class WildColorSelectionUI : MonoBehaviour
        {
            [SerializeField] private Button red;
            [SerializeField] private Button blue;
            [SerializeField] private Button yellow;
            [SerializeField] private Button green;
            [SerializeField] private Button purple;
            [SerializeField] private Button orange;
            [SerializeField] private Vector3 anchorOffset = new Vector3(0, 50, 0);

            private Dictionary<TileColor, Button> buttonsByColor = new();
            private UnityEvent<OnWildColorSelectedPayload> onColorSelected = new();

            void Awake()
            {
                this.buttonsByColor.Add(TileColor.RED, this.red);
                this.buttonsByColor.Add(TileColor.BLUE, this.blue);
                this.buttonsByColor.Add(TileColor.YELLOW, this.yellow);
                this.buttonsByColor.Add(TileColor.GREEN, this.green);
                this.buttonsByColor.Add(TileColor.PURPLE, this.purple);
                this.buttonsByColor.Add(TileColor.ORANGE, this.orange);
                foreach (KeyValuePair<TileColor, Button> buttonByColor in this.buttonsByColor)
                {
                    buttonByColor.Value.onClick.AddListener(() => this.OnButtonPress(buttonByColor.Key));
                }
                // should start the game disabled
                this.Deactivate();
            }

            public void OnButtonPress(TileColor color)
            {
                this.onColorSelected.Invoke(new OnWildColorSelectedPayload
                {
                    Color = color
                });
                this.Deactivate();
            }

            public void Deactivate()
            {
                this.DeactivateAllButtons();
                this.onColorSelected.RemoveAllListeners();
                this.gameObject.SetActive(false);
            }

            public void Activate(GameObject anchor, List<TileColor> tileColors)
            {
                this.DeactivateAllButtons();
                this.gameObject.SetActive(true);
                foreach (TileColor color in tileColors)
                {
                    this.buttonsByColor[color].gameObject.SetActive(true);
                }
                // TODO - do something to tie the overlay back to the original tile
            }

            public void AddOnColorSelectionListener(UnityAction<OnWildColorSelectedPayload> listener)
            {
                this.onColorSelected.AddListener(listener);
            }

            private void DeactivateAllButtons()
            {
                foreach (Button button in this.buttonsByColor.Values)
                {
                    button.gameObject.SetActive(false);
                }
            }
        }
    }

}

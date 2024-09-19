using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Azul.Model;
using Azul.WildColorSelectionEvents;
using TMPro;
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
            [SerializeField] private TextMeshProUGUI instructions;
            [SerializeField] private Button cancelButton;
            private bool deactivateOnSelection;

            private Dictionary<TileColor, Button> buttonsByColor = new();
            private UnityEvent<OnWildColorSelectedPayload> onColorSelected = new();

            void Awake()
            {
                this.Initialize();
                // should start the game disabled
                this.Deactivate();
            }

            void Initialize()
            {
                if (this.buttonsByColor.Count == 0)
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
                }
            }

            public void OnButtonPress(TileColor color)
            {
                this.onColorSelected.Invoke(new OnWildColorSelectedPayload
                {
                    Color = color
                });
                if (this.deactivateOnSelection)
                {
                    this.Deactivate();
                }
            }

            public void Deactivate()
            {
                this.DeactivateAllButtons();
                this.onColorSelected.RemoveAllListeners();
                this.gameObject.SetActive(false);
            }

            public void Hide()
            {
                Destroy(this.gameObject);
            }

            public void Activate(List<TileColor> tileColors, bool deactivateOnSelection, bool allowCancel)
            {
                this.Initialize();
                this.DeactivateAllButtons();
                this.gameObject.SetActive(true);
                this.deactivateOnSelection = deactivateOnSelection;
                this.cancelButton.gameObject.SetActive(allowCancel);
                foreach (TileColor color in tileColors)
                {
                    this.buttonsByColor[color].gameObject.SetActive(true);
                }
                // TODO - do something to tie the overlay back to the original tile
            }

            public void SetInstructions(string instructions)
            {
                this.instructions.text = instructions;
            }

            public void AddOnColorSelectionListener(UnityAction<OnWildColorSelectedPayload> listener)
            {
                this.onColorSelected.AddListener(listener);
            }

            public void AddOnCancel(UnityAction onCancel)
            {
                this.cancelButton.onClick.AddListener(onCancel);
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

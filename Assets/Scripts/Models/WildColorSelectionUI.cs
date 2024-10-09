using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Azul.Controller;
using Azul.Model;
using Azul.WildColorSelectionEvents;
using TMPro;
using Unity.VisualScripting;
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
            [SerializeField] private GameObject buttonContainer;
            [SerializeField] private TextMeshProUGUI instructions;
            [SerializeField] private Button cancelButton;
            [SerializeField] private float buttonSize = 50.0f;
            private bool deactivateOnSelection;

            private Dictionary<TileColor, IconButtonUI> buttonsByColor = new();
            private UnityEvent<OnWildColorSelectedPayload> onColorSelected = new();

            void Awake()
            {
                this.CreateButtons();
                // should start the game disabled
                this.Deactivate();
            }

            private void CreateButtons()
            {
                IconUIFactory iconUIFactory = System.Instance.GetUIController().GetIconUIFactory();
                foreach (TileColor color in TileColorUtils.GetTileColors())
                {
                    IconButtonUI iconUI = iconUIFactory.CreateIconButtonUI(color, this.buttonContainer.transform);
                    iconUI.GetComponent<RectTransform>().sizeDelta = Vector2.one * this.buttonSize;
                    iconUI.AddOnClickListener(() => this.OnButtonPress(color));
                    this.buttonsByColor.Add(color, iconUI);
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
                this.DeactivateAllButtons();
                this.gameObject.SetActive(true);
                this.deactivateOnSelection = deactivateOnSelection;
                this.cancelButton.gameObject.SetActive(allowCancel);
                foreach (TileColor color in tileColors)
                {
                    this.buttonsByColor[color].gameObject.SetActive(true);
                }
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
                foreach (IconButtonUI button in this.buttonsByColor.Values)
                {
                    button.gameObject.SetActive(false);
                }
            }
        }
    }

}

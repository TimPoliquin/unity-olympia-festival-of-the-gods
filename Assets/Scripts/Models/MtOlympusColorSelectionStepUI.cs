using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.MtOlympusSelectionEvents;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace MtOlympusSelectionEvents
    {
        public struct OnMtOlympusColorSelectedPayload
        {
            public TileColor Color { get; init; }
        }
    }
    namespace Model
    {
        public class MtOlympusColorSelectionStepUI : MonoBehaviour
        {
            [SerializeField] private List<ColoredValue<IconButtonUI>> selectionButtons;
            private UnityEvent<OnMtOlympusColorSelectedPayload> onSelection = new();

            private TileColor selection = TileColor.WILD;

            void Awake()
            {
                foreach (ColoredValue<IconButtonUI> entry in this.selectionButtons)
                {
                    TileColor color = entry.GetTileColor();
                    IconButtonUI buttonUI = entry.GetValue();
                    buttonUI.AddOnClickListener(() => this.OnSelection(color));
                    buttonUI.SetTileColor(color);
                    buttonUI.SetSelected(false);
                }
            }

            public void SetAvailableTokenColors(List<TileColor> colors)
            {
                foreach (ColoredValue<IconButtonUI> entry in this.selectionButtons)
                {
                    entry.GetValue().gameObject.SetActive(colors.Contains(entry.GetTileColor()));
                }
            }

            public bool HasSelection()
            {
                return this.selection != TileColor.WILD;
            }

            public TileColor GetSelection()
            {
                return this.selection;
            }

            public void Select(TileColor tileColor)
            {
                this.OnSelection(tileColor);
            }

            private void OnSelection(TileColor selectedColor)
            {
                this.selection = selectedColor;
                foreach (ColoredValue<IconButtonUI> entry in this.selectionButtons)
                {
                    IconButtonUI buttonUI = entry.GetValue();
                    buttonUI.SetSelected(entry.GetTileColor() == selectedColor);
                }
                this.onSelection.Invoke(new OnMtOlympusColorSelectedPayload
                {
                    Color = selectedColor
                });
            }

            public void AddOnSelectionListener(UnityAction<OnMtOlympusColorSelectedPayload> listener)
            {
                this.onSelection.AddListener(listener);
            }
        }
    }
}

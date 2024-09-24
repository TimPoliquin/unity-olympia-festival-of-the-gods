using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.ScoreTileSelectionUIEvent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Azul
{
    namespace ScoreTileSelectionUIEvent
    {
        public class OnSelectionCountChangePayload
        {
            public int Count { get; init; }
            public TileColor Color { get; init; }
        }
    }
    namespace Model
    {
        public interface ScoreTileSelectionUIContainer
        {
            public void AddScoreTileSelectionUI(ScoreTileSelectionUI ui);
        }
        public class ScoreTileSelectionUI : MonoBehaviour
        {
            [SerializeField] private Image iconContainer;
            [SerializeField] private Image icon;
            [SerializeField] private Slider counter;
            [SerializeField] private Button subtractButton;
            [SerializeField] private Button addButton;
            [SerializeField] private TextMeshProUGUI minText;
            [SerializeField] private TextMeshProUGUI maxText;
            [SerializeField] private TextMeshProUGUI currentText;

            private TileColor color;
            private UnityEvent<OnSelectionCountChangePayload> onValueChange = new();

            void Awake()
            {
                this.counter.onValueChanged.AddListener((value) => this.OnValueChange(true));
                this.addButton.onClick.AddListener(this.OnClickAdd);
                this.subtractButton.onClick.AddListener(this.OnClickSubtract);
            }

            public TileColor GetColor()
            {
                return this.color;
            }

            public void SetColor(TileColor color, Sprite icon, Color backgroundColor)
            {
                this.color = color;
                this.icon.sprite = icon;
                this.iconContainer.color = backgroundColor;
            }

            public void SetCounterRange(int min, int max, int total)
            {
                this.counter.minValue = min;
                this.counter.maxValue = max;
                this.minText.text = $"{min}";
                this.maxText.text = $"{total}";
            }

            public void SetDefaultValue(int value)
            {
                this.counter.value = value;
                this.OnValueChange(false);
            }

            public int GetSelectedCount()
            {
                return (int)this.counter.value;
            }

            public int GetMaxCount()
            {
                return (int)this.counter.maxValue;
            }

            public void DisableAddButton()
            {
                this.addButton.interactable = false;
            }

            public void EnableAddButton()
            {
                this.addButton.interactable = this.counter.value < this.counter.maxValue;
            }

            public void AddOnSelectionCountChangeListener(UnityAction<OnSelectionCountChangePayload> listener)
            {
                this.onValueChange.AddListener(listener);
            }

            private void OnClickAdd()
            {
                if (this.counter.value < this.counter.maxValue)
                {
                    this.counter.value++;
                }
            }

            private void OnClickSubtract()
            {
                if (this.counter.value > this.counter.minValue)
                {
                    this.counter.value--;
                }
            }

            private void OnValueChange(bool dispatch)
            {
                this.currentText.text = $"{this.counter.value}";
                this.subtractButton.interactable = this.counter.value > this.counter.minValue;
                this.addButton.interactable = this.counter.value < this.counter.maxValue;
                if (dispatch)
                {
                    this.onValueChange.Invoke(new OnSelectionCountChangePayload
                    {
                        Color = this.color,
                        Count = (int)this.counter.value
                    });
                }
            }
        }
    }
}

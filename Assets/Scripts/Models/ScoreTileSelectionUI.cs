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
        public class ScoreTileSelectionUI : MonoBehaviour
        {
            [SerializeField] private Slider counter;
            [SerializeField] private Button subtractButton;
            [SerializeField] private Button addButton;
            [SerializeField] private TextMeshProUGUI minText;
            [SerializeField] private TextMeshProUGUI maxText;
            [SerializeField] private TextMeshProUGUI currentText;

            private TileColor color;
            private UnityEvent<OnSelectionCountChangePayload> onValueChange = new();


            [SerializeField] private GameObject anchor;

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

            public void SetColor(TileColor color)
            {
                this.color = color;
            }

            public void SetCounterRange(int min = 0, int max = 6)
            {
                this.counter.minValue = min;
                this.counter.maxValue = max;
                this.minText.text = $"{min}";
                this.maxText.text = $"{max}";
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

            public void SetAnchor(GameObject anchor)
            {
                this.anchor = anchor;
                this.transform.position = Camera.main.WorldToScreenPoint(anchor.transform.position) + Vector3.right * 80;
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

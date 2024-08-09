using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Azul
{
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

            [SerializeField] private GameObject anchor;

            void Awake()
            {
                this.counter.onValueChanged.AddListener(value => currentText.text = $"{(int)value}");
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
                this.currentText.text = $"{value}";
            }

            public int GetSelectedCount()
            {
                return (int)this.counter.value;
            }

            public void SetAnchor(GameObject anchor)
            {
                this.anchor = anchor;
                this.transform.position = Camera.main.WorldToScreenPoint(anchor.transform.position);
            }
        }
    }
}

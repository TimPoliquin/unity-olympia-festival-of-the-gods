using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class IntegerStatBoxUI : MonoBehaviour
        {
            [SerializeField] private TextMeshProUGUI valueText;
            [SerializeField] private TextMeshProUGUI emptyText;

            public void SetValue(int value)
            {
                this.valueText.text = $"{value}";
                this.emptyText.gameObject.SetActive(false);
            }

            public void SetNoValue()
            {
                this.valueText.gameObject.SetActive(false);
                this.emptyText.gameObject.SetActive(true);
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class RitualNumberIconUI : MonoBehaviour
        {
            [SerializeField] private TextMeshProUGUI ritualNumberText;

            public void SetRitualNumber(int ritualNumber)
            {
                this.ritualNumberText.text = $"{ritualNumber}";
            }

            public void SetFontSize(int fontSize)
            {
                ritualNumberText.fontSize = fontSize;
            }

            public void SetSize(float width, float height)
            {
                RectTransform rectTransform = this.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(width, height);
            }
        }

    }
}

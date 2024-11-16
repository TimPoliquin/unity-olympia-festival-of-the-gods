using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class ScoringIconUI : MonoBehaviour
        {
            [SerializeField] private TextMeshProUGUI scoreText;

            public void SetScore(int score)
            {
                this.scoreText.text = $"{score}";
            }

            public void SetFontSize(int fontSize)
            {
                this.scoreText.fontSize = fontSize;
            }

            public void SetSize(float width, float height)
            {
                RectTransform rect = this.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(width, height);
            }
        }

    }
}

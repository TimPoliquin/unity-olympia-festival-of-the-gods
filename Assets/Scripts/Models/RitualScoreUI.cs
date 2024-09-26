using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class RitualScoreUI : MonoBehaviour
        {
            [SerializeField] private TextMeshProUGUI valueText;
            [SerializeField] private TextMeshProUGUI scoreText;

            public void Setup(int value, int score)
            {
                this.valueText.text = $"{value}";
                this.scoreText.text = $"{score}";
            }

        }
    }
}

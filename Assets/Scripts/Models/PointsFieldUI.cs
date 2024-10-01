using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class PointsFieldUI : MonoBehaviour
        {
            [SerializeField] private TextMeshProUGUI scoreValueText;

            public void SetScoreValue(int scoreValue)
            {
                this.scoreValueText.text = $"{scoreValue}";
            }
        }

    }
}

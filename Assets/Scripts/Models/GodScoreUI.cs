using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class GodScoreUI : MonoBehaviour
        {
            [SerializeField] private IconUI icon;
            [SerializeField] private TextMeshProUGUI scoreText;

            public void Setup(TileColor tileColor, int score)
            {
                System.Instance.GetUIController().GetIconUIFactory().SetIconValues(this.icon, tileColor);
                scoreText.text = $"{score}";
            }
        }

    }
}

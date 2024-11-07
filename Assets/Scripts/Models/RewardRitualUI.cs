using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Azul
{
    namespace Model
    {
        public class RewardRitualUI : MonoBehaviour
        {
            [SerializeField] private IconUI icon;
            [SerializeField] private TextMeshProUGUI valueText;
            [SerializeField] private Image completedIcon;

            private bool isCompleted;

            public void Setup(TileColor tileColor, int value, bool isCompleted)
            {
                this.icon.SetTileColor(tileColor);
                this.valueText.text = $"{value}";
                this.isCompleted = isCompleted;
                if (!isCompleted)
                {
                    this.completedIcon.color = new Color(0, 0, 0, 0);
                }
            }

            public bool IsCompleted()
            {
                return this.isCompleted;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class RewardRitualUI : MonoBehaviour
        {
            [SerializeField] private IconUI icon;
            [SerializeField] private TextMeshProUGUI valueText;

            public void Setup(TileColor tileColor, int value)
            {
                this.icon.SetTileColor(tileColor);
                this.valueText.text = $"{value}";
            }
        }
    }
}

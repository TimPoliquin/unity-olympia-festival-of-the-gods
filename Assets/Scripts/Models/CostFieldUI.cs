using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class CostFieldUI : MonoBehaviour
        {
            [SerializeField] private IconUI icon;
            [SerializeField] private TextMeshProUGUI valueText;

            public void SetTileColor(TileColor tileColor)
            {
                this.icon.SetTileColor(tileColor);
            }

            public void SetValue(int value)
            {
                this.valueText.text = $"{value}";
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class ColorMilestoneProgressFieldUI : MonoBehaviour
        {
            [SerializeField] private IconUI icon;
            [SerializeField] private TextMeshProUGUI valueText;

            public void SetTileColor(TileColor tileColor)
            {
                this.icon.SetTileColor(tileColor);
            }

            public void SetProgress(int completed, int total)
            {
                this.valueText.text = $"{completed}/{total}";
            }
        }

    }
}

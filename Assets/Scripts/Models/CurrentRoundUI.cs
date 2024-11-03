using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Azul
{
    namespace Model
    {
        public class CurrentRoundUI : MonoBehaviour
        {
            [SerializeField] private Color inactiveColor;
            [SerializeField] private Color activeColor;
            [SerializeField] private IconUI currentIcon;
            [SerializeField] private List<ColoredValue<HonoringIconUI>> honoringIcons;

            public void SetActiveColor(TileColor activeColor)
            {
                foreach (ColoredValue<HonoringIconUI> honoringIcon in this.honoringIcons)
                {
                    Color statusColor;
                    if (honoringIcon.GetTileColor() == activeColor)
                    {
                        statusColor = this.activeColor;
                    }
                    else
                    {
                        statusColor = this.inactiveColor;
                    }
                    honoringIcon
                        .GetValue()
                        .SetStatusColor(statusColor);
                }
                System.Instance.GetUIController().GetIconUIFactory().SetIconValues(this.currentIcon, activeColor);
                this.currentIcon.EnableFrame();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
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
            [SerializeField] private TextMeshProUGUI currentPlayerText;
            [SerializeField] private GameObject currentPlayerContainer;

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

            public void SetCurrentPlayerName(string playerName)
            {
                if (null == playerName)
                {
                    this.currentPlayerContainer.SetActive(false);
                }
                else
                {
                    this.currentPlayerContainer.SetActive(true);
                    this.currentPlayerText.text = playerName;
                }
            }
        }
    }
}

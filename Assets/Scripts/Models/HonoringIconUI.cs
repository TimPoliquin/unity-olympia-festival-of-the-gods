using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Azul
{
    namespace Model
    {
        public class HonoringIconUI : MonoBehaviour
        {
            [SerializeField] private Image statusIndicator;
            [SerializeField] private Image backgroundImage;
            [SerializeField] private Image iconImage;

            public void SetStatusColor(Color statusColor)
            {
                this.statusIndicator.color = statusColor;
            }

            public Sprite GetIcon()
            {
                return this.iconImage.sprite;
            }

            public Color GetBackgroundColor()
            {
                return this.backgroundImage.color;
            }
        }
    }
}

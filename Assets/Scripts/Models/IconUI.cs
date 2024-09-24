using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Azul
{
    namespace Model
    {
        public class IconUI : MonoBehaviour
        {

            [SerializeField] private Image backgroundImage;
            [SerializeField] private Image iconImage;

            public void SetBackgroundColor(Color color)
            {
                this.backgroundImage.color = color;
            }

            public void SetIcon(Sprite icon)
            {
                this.iconImage.sprite = icon;
            }

            public Sprite GetIcon()
            {
                return this.iconImage.sprite;
            }

            public Color GetBackgroundColor()
            {
                return this.backgroundImage.color;
            }

            public Image GetBackgroundImage()
            {
                return this.backgroundImage;
            }


        }
    }
}

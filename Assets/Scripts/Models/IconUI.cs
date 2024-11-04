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
            [SerializeField] private Image frame;

            public void SetBackgroundColor(Color color)
            {
                this.backgroundImage.color = color;
            }

            public void SetIcon(Sprite icon, Color iconColor)
            {
                this.iconImage.sprite = icon;
                this.iconImage.color = iconColor;
            }

            public void SetFrameColor(Color color)
            {
                if (this.frame != null)
                {
                    this.frame.color = color;
                }
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

            public void SetTileColor(TileColor tileColor)
            {
                System.Instance.GetUIController().GetIconUIFactory().SetIconValues(this, tileColor);
            }

            public void SetSize(float size)
            {
                this.GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
            }

            public void EnableFrame()
            {
                this.frame.gameObject.SetActive(true);
            }
        }
    }
}

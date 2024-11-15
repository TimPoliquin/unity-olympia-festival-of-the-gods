using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        [Serializable]
        public struct IconConfig
        {
            [SerializeField] private Color IconColor;
            [SerializeField] private Color Color;
            [SerializeField] private Color FrameColor;
            [SerializeField] private Sprite Icon;

            public Color GetIconColor()
            {
                return this.IconColor;
            }

            public Color GetColor()
            {
                return this.Color;
            }
            public Color GetFrameColor()
            {
                return this.FrameColor;
            }
            public Sprite GetIcon()
            {
                return this.Icon;
            }
        }
        public class IconUIFactory : MonoBehaviour
        {
            [SerializeField] private IconUI iconUIPrefab;
            [SerializeField] private IconButtonUI iconButtonUIPrefab;
            [SerializeField] private List<ColoredValue<IconConfig>> icons;

            private Dictionary<TileColor, Sprite> iconsByColor;
            private Dictionary<TileColor, Color> backgroundsByColor;
            private Dictionary<TileColor, Color> iconColorByColor;
            private Dictionary<TileColor, Color> framesByColor;

            void Start()
            {
                this.SetupIcons();
            }

            private void SetupIcons()
            {
                this.iconsByColor = new();
                this.backgroundsByColor = new();
                this.iconColorByColor = new();
                this.framesByColor = new();
                foreach (ColoredValue<IconConfig> honoringIcon in this.icons)
                {
                    this.iconsByColor[honoringIcon.GetTileColor()] = honoringIcon.GetValue().GetIcon();
                    this.iconColorByColor[honoringIcon.GetTileColor()] = honoringIcon.GetValue().GetIconColor();
                    this.backgroundsByColor[honoringIcon.GetTileColor()] = honoringIcon.GetValue().GetColor();
                    this.framesByColor[honoringIcon.GetTileColor()] = honoringIcon.GetValue().GetFrameColor();
                }
            }

            public Sprite GetIcon(TileColor color)
            {
                return this.iconsByColor[color];
            }

            public Color GetIconColor(TileColor color)
            {
                return this.iconColorByColor[color];
            }

            public Color GetBackgroundColor(TileColor color)
            {
                return this.backgroundsByColor[color];
            }

            public Color GetFrameColor(TileColor color)
            {
                return this.framesByColor[color];
            }

            public void SetIconValues(IconUI iconUI, TileColor color)
            {
                iconUI.SetBackgroundColor(this.GetBackgroundColor(color));
                iconUI.SetIcon(this.GetIcon(color), this.GetIconColor(color));
                iconUI.SetFrameColor(this.GetFrameColor(color));
            }

            public IconUI Create(TileColor color, Transform parent = null)
            {
                IconUI iconUI = Instantiate(this.iconUIPrefab, parent ? parent : null);
                this.SetIconValues(iconUI, color);
                return iconUI;
            }

            public IconUI CreateFramed(TileColor color, Transform parent)
            {
                IconUI iconUI = Instantiate(this.iconUIPrefab, parent);
                this.SetIconValues(iconUI, color);
                iconUI.EnableFrame();
                return iconUI;
            }

            public IconButtonUI CreateIconButtonUI(TileColor color, Transform parent)
            {
                IconButtonUI iconButtonUI = Instantiate(this.iconButtonUIPrefab, parent);
                this.SetIconValues(iconButtonUI, color);
                return iconButtonUI;
            }

        }

    }
}

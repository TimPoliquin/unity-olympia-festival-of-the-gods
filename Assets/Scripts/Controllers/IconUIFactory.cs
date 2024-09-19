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
            [SerializeField] private Color Color;
            [SerializeField] private Sprite Icon;

            public Color GetColor()
            {
                return this.Color;
            }
            public Sprite GetIcon()
            {
                return this.Icon;
            }
        }
        public class IconUIFactory : MonoBehaviour
        {
            [SerializeField] private List<ColoredValue<IconConfig>> icons;

            private Dictionary<TileColor, Sprite> iconsByColor;
            private Dictionary<TileColor, Color> backgroundsByColor;

            void Start()
            {
                this.SetupIcons();
            }

            private void SetupIcons()
            {
                this.iconsByColor = new();
                this.backgroundsByColor = new();
                foreach (ColoredValue<IconConfig> honoringIcon in this.icons)
                {
                    this.iconsByColor[honoringIcon.GetTileColor()] = honoringIcon.GetValue().GetIcon();
                    this.backgroundsByColor[honoringIcon.GetTileColor()] = honoringIcon.GetValue().GetColor();
                }
            }

            public Sprite GetIcon(TileColor color)
            {
                return this.iconsByColor[color];
            }

            public Color GetBackgroundColor(TileColor color)
            {
                return this.backgroundsByColor[color];
            }
        }

    }
}

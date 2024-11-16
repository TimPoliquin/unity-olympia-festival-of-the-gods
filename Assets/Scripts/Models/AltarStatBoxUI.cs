using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Azul
{
    namespace Model
    {
        public class AltarStatBoxUI : MonoBehaviour
        {
            [SerializeField] private List<ColoredValue<IconUI>> icons;
            [SerializeField] private TextMeshProUGUI emptyStatText;

            void Awake()
            {
                foreach (ColoredValue<IconUI> icon in this.icons)
                {
                    icon.GetValue().SetTileColor(icon.GetTileColor());
                    icon.GetValue().gameObject.SetActive(false);
                }
                this.emptyStatText.gameObject.SetActive(false);
            }

            public void ShowIcon(TileColor color)
            {
                this.icons
                    .Find(icon => icon.GetTileColor() == color)
                    .GetValue().gameObject.SetActive(true);
            }

            public void ShowEmptyStat()
            {
                this.emptyStatText.gameObject.SetActive(true);
            }
        }
    }
}

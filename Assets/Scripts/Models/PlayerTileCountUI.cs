using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class PlayerTileCountUI : MonoBehaviour
        {
            [SerializeField] private TextMeshProUGUI countText;
            [SerializeField] private IconUI iconUI;
            [SerializeField] private TileColor tileColor;
            [SerializeField] private float size = 30.0f;

            void Start()
            {
                this.SetTileColor(this.tileColor);
                this.SetSize(this.size);
            }

            public void SetTileCount(int count)
            {
                this.countText.text = $"{count}";
                this.gameObject.SetActive(count > 0);
            }

            public void SetTileColor(TileColor tileColor)
            {
                this.tileColor = tileColor;
                System.Instance.GetUIController().GetIconUIFactory().SetIconValues(this.iconUI, tileColor);
            }

            public void SetSize(float size)
            {
                this.size = size;
                this.iconUI.SetSize(size);
                this.GetComponent<RectTransform>().sizeDelta = new Vector2(size + 20.0f, size);
            }
        }
    }
}

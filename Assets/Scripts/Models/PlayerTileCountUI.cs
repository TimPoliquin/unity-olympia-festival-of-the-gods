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

            public void SetTileCount(int count)
            {
                this.countText.text = $"{count}";
                this.gameObject.SetActive(count > 0);
            }
        }
    }
}

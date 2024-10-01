using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Azul
{
    namespace Model
    {
        [Serializable]
        public struct RewardColor
        {
            [Range(1, 3)] public int Value;
            public Color Color;
            public Sprite Icon;
        }
        public class RewardProgressFieldUI : MonoBehaviour
        {
            [SerializeField] private Image icon;
            [SerializeField] private TextMeshProUGUI valueText;
            [SerializeField] private List<RewardColor> colors;

            public void SetRewardValue(int value)
            {
                this.icon.sprite = this.colors[value - 1].Icon;
                this.icon.color = this.colors[value - 1].Color;
            }

            public void SetProgress(int completed, int total)
            {
                this.valueText.text = $"{completed}/{total}";
            }
        }

    }
}

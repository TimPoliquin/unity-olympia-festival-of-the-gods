using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class ValueMilestoneProgressFieldUI : MonoBehaviour
        {
            [SerializeField] private TextMeshProUGUI valueText;
            [SerializeField] private TextMeshProUGUI progressText;

            public void SetValue(int value)
            {
                this.valueText.text = $"{value}";
            }

            public void SetProgress(int completed, int total)
            {
                this.progressText.text = $"{completed}/{total}";
            }
        }

    }
}

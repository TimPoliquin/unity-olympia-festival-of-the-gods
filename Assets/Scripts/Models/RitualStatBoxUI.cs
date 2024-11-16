using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class RitualStatBoxUI : MonoBehaviour
        {
            [Serializable]
            private class NumberedValue<T>
            {
                [SerializeField] private int Key;
                [SerializeField] private T Value;

                public int GetKey()
                {
                    return this.Key;
                }
                public T GetValue()
                {
                    return this.Value;
                }
            }
            [SerializeField] private List<NumberedValue<RitualNumberIconUI>> rituals;
            [SerializeField] private TextMeshProUGUI emptyStatText;

            void Awake()
            {
                foreach (NumberedValue<RitualNumberIconUI> entry in this.rituals)
                {
                    entry.GetValue().SetRitualNumber(entry.GetKey());
                    entry.GetValue().gameObject.SetActive(false);
                }
                this.emptyStatText.gameObject.SetActive(false);
            }

            public void ShowRitual(int ritualNumber)
            {
                this.rituals
                    .Find(icon => icon.GetKey() == ritualNumber)
                    .GetValue().gameObject.SetActive(true);
            }

            public void ShowEmptyStat()
            {
                this.emptyStatText.gameObject.SetActive(true);
            }
        }
    }
}

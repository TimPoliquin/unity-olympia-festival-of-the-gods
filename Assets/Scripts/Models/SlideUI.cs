using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class SlideUI : MonoBehaviour
        {
            [SerializeField] private string title;

            public void SetActive(bool active)
            {
                this.gameObject.SetActive(active);
            }

            public string GetTitle()
            {
                return this.title;
            }
        }
    }
}

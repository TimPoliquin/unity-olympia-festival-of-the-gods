using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class StarSpace : MonoBehaviour
        {
            [SerializeField] private GameObject tile;
            [SerializeField][Range(1, 6)] private int value;
            [SerializeField] private bool occupied;

            public void SetValue(int value)
            {
                this.value = value;
            }
        }

    }
}

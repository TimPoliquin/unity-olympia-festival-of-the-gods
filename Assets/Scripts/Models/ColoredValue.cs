using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        [Serializable]
        public struct ColoredValue<T>
        {
            [SerializeField] private TileColor tileColor;
            [SerializeField] private T value;

            public TileColor GetTileColor()
            {
                return this.tileColor;
            }

            public T GetValue()
            {
                return this.value;
            }
        }

    }
}

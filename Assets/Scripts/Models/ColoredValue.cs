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
        public class ColoredValue<T>
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

            public static ColoredValue<T> Create<T>(TileColor tileColor, T value)
            {
                ColoredValue<T> ret = new ColoredValue<T>
                {
                    tileColor = tileColor,
                    value = value
                };
                return ret;
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class Tile : MonoBehaviour
        {
            public Color Color;
            public bool IsPlaceholder;

            public static Tile Create(Color color, bool IsPlaceholder = false)
            {
                GameObject gameObject = new GameObject();
                Tile tile = gameObject.AddComponent<Tile>();
                tile.Color = color;
                tile.IsPlaceholder = IsPlaceholder;
                return tile;
            }
        }

    }
}

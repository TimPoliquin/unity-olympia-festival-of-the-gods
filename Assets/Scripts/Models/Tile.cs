using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class Tile : MonoBehaviour
        {
            public TileColor Color;
            public bool IsPlaceholder;

            public static Tile Create(GameObject tilePrefab, TileColor color, bool IsPlaceholder = false)
            {
                GameObject gameObject = Instantiate(tilePrefab);
                gameObject.name = $"Tile {color}";
                Tile tile = gameObject.GetComponent<Tile>();
                tile.Color = color;
                tile.IsPlaceholder = IsPlaceholder;
                return tile;
            }
        }

    }
}

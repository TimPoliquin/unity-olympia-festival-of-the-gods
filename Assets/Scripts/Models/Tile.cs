using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class Tile : MonoBehaviour
        {
            public TileColor Color { get; private set; }

            public static Tile Create(GameObject tilePrefab, TileColor color)
            {
                GameObject gameObject = Instantiate(tilePrefab);
                gameObject.name = $"Tile {color}";
                Tile tile = gameObject.GetComponent<Tile>();
                tile.Color = color;
                return tile;
            }
        }

    }
}

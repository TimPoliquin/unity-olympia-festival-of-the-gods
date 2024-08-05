using System.Collections;
using System.Collections.Generic;
using Azul.Controller;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Azul
{
    namespace Model
    {
        public class Tile : MonoBehaviour
        {
            [SerializeField] private TileColor color;

            public TileColor Color
            {
                get => this.color;
                private set { this.color = value; }
            }

            public TilePointerController GetTilePointerController()
            {
                TilePointerController tilePointerController = null;
                if (this.TryGetComponent<TilePointerController>(out tilePointerController))
                {
                    return tilePointerController;
                }
                else
                {
                    UnityEngine.Debug.Log("No tile pointer controller assigned!");
                    return null;
                }
            }

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

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
            private Outline outline;

            public TileColor Color
            {
                get => this.color;
                private set { this.color = value; }
            }

            public TilePointerEventController GetTilePointerController()
            {
                TilePointerEventController tilePointerController;
                if (this.TryGetComponent(out tilePointerController))
                {
                    return tilePointerController;
                }
                else
                {
                    UnityEngine.Debug.Log("No tile pointer controller assigned!");
                    return null;
                }
            }

            public Outline GetOutline()
            {
                if (null == this.outline)
                {
                    this.outline = this.GetComponentInChildren<Outline>();
                }
                return this.outline;
            }

            public bool IsHadesToken()
            {
                return this.color == TileColor.ONE;
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

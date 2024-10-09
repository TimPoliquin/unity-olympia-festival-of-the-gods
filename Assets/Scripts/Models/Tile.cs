using System.Collections;
using System.Collections.Generic;
using Azul.Controller;
using Azul.Cursor;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Azul
{
    namespace Model
    {
        public class Tile : MonoBehaviour
        {
            [SerializeField] private TileColor color;
            [SerializeField] private AudioSource tableCollisionSFX;

            private Outline outline;
            private CursorChange cursorChange;

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

            public CursorChange GetCursorChange()
            {
                if (null == this.cursorChange)
                {
                    this.cursorChange = this.GetComponentInChildren<CursorChange>();
                }
                return this.cursorChange;
            }

            public void MakeSelectable()
            {
                this.GetOutline().enabled = true;
                if (System.Instance.GetPlayerController().GetCurrentPlayer().IsHuman())
                {
                    this.GetCursorChange().enabled = true;
                    this.GetCursorChange().OnHoverEnter();
                }
            }

            public void MakeUnselectable()
            {
                this.GetOutline().enabled = false;
                if (System.Instance.GetPlayerController().GetCurrentPlayer().IsHuman())
                {
                    this.GetCursorChange().enabled = false;
                    this.GetCursorChange().OnHoverExit();
                }
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

            void OnCollisionEnter(Collision collision)
            {
                if (collision.gameObject.CompareTag("Table"))
                {
                    if (System.Instance.GetCameraController().IsInView(this.gameObject))
                    {
                        this.tableCollisionSFX.Play();
                    }
                }
            }

            public void DisableSound()
            {
                this.tableCollisionSFX.enabled = false;
            }

            public void EnableSound()
            {
                this.tableCollisionSFX.enabled = true;
            }
        }

    }
}

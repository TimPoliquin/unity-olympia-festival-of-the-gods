using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Cursor
    {
        public class CursorChange : MonoBehaviour
        {
            [SerializeField] private Texture2D hoverTexture;
            [SerializeField] private Vector2 offset = new Vector2(6, 6);

            private bool hovered = false;

            public void OnHoverEnter()
            {
                if (this.enabled)
                {
                    this.hovered = true;
                    UnityEngine.Cursor.SetCursor(this.hoverTexture, this.offset, CursorMode.Auto);
                }
            }

            public void OnHoverExit()
            {
                this.hovered = false;
                UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }

            void OnDisable()
            {
                if (this.hovered)
                {
                    this.OnHoverExit();
                }
            }
        }
    }
}

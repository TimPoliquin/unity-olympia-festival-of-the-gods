using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Controller;
using Azul.Layout;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class Star : MonoBehaviour
        {
            [SerializeField] private GameObject layoutGameObject;
            [SerializeField] private TileColor color;
            [SerializeField] private StarSpace[] spaces;

            private CircularLayout layout;

            void Awake()
            {
                this.layout = this.layoutGameObject.GetComponent<CircularLayout>();
            }

            public void SetColor(TileColor color)
            {
                this.color = color;
            }
            public void SetSpaces(StarSpace[] spaces)
            {
                this.spaces = spaces;
            }

            public void AddTilePlaceholders(List<TilePlaceholder> placeholders)
            {
                this.layout.AddChildren(placeholders.Select(placeholder => placeholder.gameObject).ToList());
            }

        }
    }
}

using System.Collections;
using System.Collections.Generic;
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
            [SerializeField] private TileColor color;
            [SerializeField] private StarSpace[] spaces;

            public void SetColor(TileColor color)
            {
                this.color = color;
            }
            public void SetSpaces(StarSpace[] spaces)
            {
                this.spaces = spaces;
            }

        }
    }
}

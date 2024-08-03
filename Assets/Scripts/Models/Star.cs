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
            [SerializeField] private GameObject tilePrefab;
            [SerializeField] private float radius = 3.0f;
            [SerializeField] private TileColor color;
            [SerializeField] private StarSpace[] spaces;

            public void SetSpaces(StarSpace[] spaces)
            {
                this.spaces = spaces;
            }

        }
    }
}

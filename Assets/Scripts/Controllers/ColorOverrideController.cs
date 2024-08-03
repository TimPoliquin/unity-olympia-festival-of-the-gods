using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class ColorOverrideController : MonoBehaviour
        {
            [SerializeField] private Model.TileColor color;

            public Model.TileColor GetColor()
            {
                return this.color;
            }
        }
    }
}

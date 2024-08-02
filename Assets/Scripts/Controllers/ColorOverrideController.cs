using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class ColorOverrideController : MonoBehaviour
        {
            [SerializeField] private Model.Color color;

            public Model.Color GetColor()
            {
                return this.color;
            }
        }
    }
}

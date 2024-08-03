using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class Star : MonoBehaviour
        {
            [SerializeField] private StarSpace[] spaces = new StarSpace[6];
        }
    }
}

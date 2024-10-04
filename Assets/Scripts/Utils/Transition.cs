using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Util
    {
        public struct Transition<T>
        {
            public T Original { get; init; }
            public T Target { get; init; }
        }
    }
}

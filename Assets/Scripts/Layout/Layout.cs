using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Layout
    {
        public interface Layout
        {
            public void AddChildren(List<GameObject> children);
        }
    }
}

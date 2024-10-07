using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Utils
    {
        public sealed class VectorUtils
        {

            public static Vector3 CreateRandomVector3(float radius, float height)
            {
                return new Vector3(Random.Range(-radius, radius), height, Random.Range(-radius, radius));
            }

            public static Vector3 CreateRandomVector3(float width, float height, float depth)
            {
                return new Vector3(Random.Range(-width, width), height, Random.Range(-depth, depth));
            }
        }
    }
}

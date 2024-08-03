using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using Azul.Debug;

namespace Azul
{
    namespace Layout
    {
        public class CircularLayout : MonoBehaviour
        {

            private float radius;
            public class CircularLayoutElementInput
            {
                public int Index { get; init; }
                public Vector3 Position { get; init; }
                public float Angle { get; init; }
            }

            public void CreateLayout(int numElements, float radius, Func<CircularLayoutElementInput, GameObject> CreateElement, bool rotate = true)
            {
                this.radius = radius;
                float arcSize = Mathf.PI * 2.0f / numElements;
                for (int idx = 0; idx < numElements; idx++)
                {
                    float x = radius * Mathf.Sin(idx * arcSize);
                    float z = radius * Mathf.Cos(idx * arcSize);
                    GameObject element = CreateElement(new CircularLayoutElementInput
                    {
                        Index = idx,
                    });
                    element.transform.SetParent(this.transform);
                    element.transform.localPosition = new Vector3(x, 0, z);
                    if (rotate)
                    {
                        element.transform.Rotate(0, Mathf.Rad2Deg * arcSize * idx, 0);
                    }
                }
            }

            void OnDrawGizmosSelected()
            {
                DebugUtils.DrawCircle(this.transform.position, this.radius, 50, Color.white);
            }
        }
    }
}

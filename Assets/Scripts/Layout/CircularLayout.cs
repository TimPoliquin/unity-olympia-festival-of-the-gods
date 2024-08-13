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
        public class CircularLayout : MonoBehaviour, Layout
        {
            [SerializeField] private float radius;
            [SerializeField] private bool rotate;
            [SerializeField] private float rotateAfterLayout;

            public void AddChildren(List<GameObject> children)
            {
                int numElements = children.Count;
                float arcSize = Mathf.PI * 2.0f / numElements;
                for (int idx = 0; idx < numElements; idx++)
                {
                    float x = radius * Mathf.Sin(idx * arcSize);
                    float z = radius * Mathf.Cos(idx * arcSize);
                    GameObject element = children[idx];
                    element.transform.SetParent(this.transform);
                    element.transform.localPosition = new Vector3(x, 0, z);
                    if (this.rotate)
                    {
                        element.transform.Rotate(0, Mathf.Rad2Deg * arcSize * idx, 0);
                    }
                }
                this.transform.Rotate(0, rotateAfterLayout, 0);
            }

            void OnDrawGizmosSelected()
            {
                DebugUtils.DrawCircle(this.transform.position, this.radius, 50, Color.white);
            }
        }
    }
}

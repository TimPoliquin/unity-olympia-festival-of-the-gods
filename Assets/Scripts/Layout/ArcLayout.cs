using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Layout
    {
        public class ArcLayout : MonoBehaviour, Layout
        {
            [SerializeField] private float radius;
            [SerializeField] private float arcSize = 180f;
            public void AddChildren(List<GameObject> children)
            {
                int numElements = children.Count;
                float segmentSize = this.arcSize * Mathf.Deg2Rad / numElements;
                float arcRotation = -1 * this.arcSize / 2f * Mathf.Deg2Rad + segmentSize / 2f;
                for (int idx = 0; idx < numElements; idx++)
                {
                    float x = radius * Mathf.Sin(idx * segmentSize + arcRotation);
                    float z = radius * Mathf.Cos(idx * segmentSize + arcRotation);
                    GameObject element = children[idx];
                    element.transform.SetParent(this.transform);
                    element.transform.localPosition = new Vector3(x, 0, z);
                }
            }
        }
    }
}

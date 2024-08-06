using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{

    namespace Model
    {
        [Serializable]
        public class CameraSettings
        {
            [SerializeField] private Vector3 offset;
            [SerializeField] private Quaternion rotation;
            [SerializeField] private float size;

            public Vector3 GetOffset()
            {
                return this.offset;
            }

            public Quaternion GetRotation()
            {
                return this.rotation;
            }

            public float GetSize()
            {
                return this.size;
            }
        }

    }
}

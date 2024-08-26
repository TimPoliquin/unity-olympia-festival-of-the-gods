using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    public class Spiral : MonoBehaviour
    {
        [SerializeField] private float angularSpeed = 2.0f;
        [SerializeField] private float bobSpeed = 2.0f;
        [SerializeField] private float radius = 2.0f;
        [SerializeField] private float height = 2.0f;
        [SerializeField] float angle = 0;
        [SerializeField] float verticalAngle = 0;

        void Start()
        {
            this.angle = UnityEngine.Random.Range(0, 360);
            this.verticalAngle = UnityEngine.Random.Range(0, 360);
        }

        void Update()
        {
            this.angle = (this.angle + Time.deltaTime * angularSpeed) % 360.0f; // update angle
            this.verticalAngle = (this.verticalAngle + Time.deltaTime * bobSpeed) % 360.0f;
            Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward; // calculate direction from center - rotate the up vector Angle degrees clockwise
            float verticalPosition = this.height * Mathf.Sin(Mathf.Deg2Rad * this.verticalAngle) + this.height;
            transform.localPosition = Vector3.zero + direction * radius + Vector3.up * verticalPosition; // update position based on center, the direction, and the radius (which is a constant)
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Layout
    {
        public class LinearLayout : MonoBehaviour, Layout
        {
            public enum Axis
            {
                X,
                Y,
                Z
            }
            public enum Direction
            {
                POSITIVE,
                NEGATIVE
            }
            [SerializeField] private Axis axis = Axis.X;
            [SerializeField] private Direction direction = Direction.POSITIVE;
            [SerializeField] private float spacing = 1.0f;
            [SerializeField] private bool normalizeRotation;

            public void Awake()
            {
                this.LayoutChildren(this.GetChildren());
            }

            public void AddChildren(List<GameObject> newChildren)
            {
                List<GameObject> children = this.GetChildren();
                children.AddRange(newChildren);
                this.LayoutChildren(children);
            }

            private List<GameObject> GetChildren()
            {
                List<GameObject> children = new();
                foreach (Transform transform in this.transform)
                {
                    children.Add(transform.gameObject);
                }
                return children;
            }

            private void LayoutChildren(List<GameObject> children)
            {
                Vector3 layoutDirection;
                switch (this.axis)
                {
                    case Axis.X:
                        layoutDirection = Vector3.right;
                        break;
                    case Axis.Y:
                        layoutDirection = Vector3.up;
                        break;
                    case Axis.Z:
                        layoutDirection = Vector3.forward;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(this.direction), $"Unexpected layout direction: {this.direction}");
                }
                if (direction == Direction.NEGATIVE)
                {
                    layoutDirection *= -1;
                }
                layoutDirection *= this.spacing;
                for (int idx = 0; idx < children.Count; idx++)
                {
                    GameObject child = children[idx];
                    child.transform.SetParent(this.transform);
                    child.transform.localPosition = layoutDirection * idx;
                    if (this.normalizeRotation)
                    {
                        child.transform.localEulerAngles = Vector3.zero;
                    }
                }
            }
        }
    }
}

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
                NEGATIVE,
                CENTER
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

            public void Refresh()
            {
                this.LayoutChildren(this.GetChildren());
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
                switch (this.direction)
                {
                    case Direction.NEGATIVE:
                        this.PlaceChildren(children, layoutDirection * this.spacing * -1, Vector3.zero);
                        break;
                    case Direction.POSITIVE:
                        this.PlaceChildren(children, layoutDirection * this.spacing, Vector3.zero);
                        break;
                    case Direction.CENTER:
                        this.PlaceChildren(children, layoutDirection * spacing, (layoutDirection * this.spacing * ((float)children.Count)) / 2.0f);
                        break;
                }
            }

            private void PlaceChildren(List<GameObject> children, Vector3 layoutDirection, Vector3 offset)
            {
                for (int idx = 0; idx < children.Count; idx++)
                {
                    GameObject child = children[idx];
                    this.PlaceChild(child, layoutDirection * idx, offset);
                }
            }

            private void PlaceChild(GameObject child, Vector3 position, Vector3 offset)
            {
                child.transform.SetParent(this.transform);
                child.transform.localPosition = position - offset;
                if (this.normalizeRotation)
                {
                    child.transform.localEulerAngles = Vector3.zero;
                }
            }
        }
    }
}

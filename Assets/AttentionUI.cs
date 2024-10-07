using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Azul
{
    namespace Animation
    {
        public class AttentionUI : MonoBehaviour
        {
            [SerializeField] float rotationDuration;
            [SerializeField] float pulseDuration;
            [SerializeField] Image image;
            private RectTransform rectTransform;
            private float rotationTime = 0f;
            private float pulseTime = 0f;
            private Color color;
            private Color transparent;
            private Color originalColor;
            private Color targetColor;
            void Awake()
            {
                this.rectTransform = this.GetComponent<RectTransform>();
                this.color = this.image.color;
                this.transparent = new Color(this.color.r, this.color.g, this.color.b, .5f);
                this.originalColor = this.color;
                this.targetColor = this.transparent;
            }
            // Update is called once per frame
            void Update()
            {
                this.Rotate();
                this.Pulse();
            }

            void Rotate()
            {
                this.rotationTime = (rotationTime + Time.deltaTime) % this.rotationDuration;
                this.rectTransform.rotation = Quaternion.Euler(0, 0, rotationTime / rotationDuration * -360f);
            }

            void Pulse()
            {
                this.pulseTime += Time.deltaTime;
                if (this.pulseTime > this.pulseDuration)
                {
                    if (this.targetColor == this.color)
                    {
                        this.originalColor = this.color;
                        this.targetColor = this.transparent;
                    }
                    else
                    {
                        this.originalColor = this.transparent;
                        this.targetColor = this.color;
                    }
                    this.pulseTime = this.pulseTime % this.pulseDuration;
                }
                this.image.color = Color.Lerp(this.originalColor, this.targetColor, this.pulseTime / this.pulseDuration);

            }
        }
    }
}

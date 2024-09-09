using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using Utils;

namespace Azul
{
    namespace Model
    {
        public class Fire : MonoBehaviour
        {
            [SerializeField] private VisualEffect fire;
            [SerializeField] private Light fireLight;
            [SerializeField] float maxInterval = 1f;
            [SerializeField] float minIntensity = 1f;
            [SerializeField] float maxIntensity = 5f;


            float targetIntensity;
            float lastIntensity;
            float interval;
            float timer;

            void Awake()
            {
                //this.Disable();
            }

            void Update()
            {
                if (this.fireLight.gameObject.activeInHierarchy)
                {
                    timer += Time.deltaTime;

                    if (timer > interval)
                    {
                        lastIntensity = this.fireLight.intensity;
                        targetIntensity = Random.Range(this.minIntensity, this.maxIntensity);
                        timer = 0;
                        interval = Random.Range(0, maxInterval);
                    }

                    this.fireLight.intensity = Mathf.Lerp(lastIntensity, targetIntensity, timer / interval);
                }
            }

            public void Enable()
            {
                this.fire.gameObject.SetActive(true);
                this.fireLight.gameObject.SetActive(true);
            }

            public void Disable()
            {
                this.fire.gameObject.SetActive(false);
                this.fireLight.gameObject.SetActive(false);
            }

            public void SetColor(TileColor tileColor)
            {
                Color color = System.Instance.GetTileMaterialProvider().GetColor(tileColor);
                this.fire.SetVector4("Color", color * 5);
                this.fireLight.color = color;
            }
        }
    }
}

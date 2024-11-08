using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class FlickeringSpotLight : MonoBehaviour
        {

            private static int BASE_COLOR = Shader.PropertyToID("_BaseColor");
            private static int EMISSION_COLOR = Shader.PropertyToID("_EmissionColor");

            [SerializeField] float maxInterval = 1f;
            [SerializeField] float minIntensity = 1f;
            [SerializeField] float maxIntensity = 5f;
            [SerializeField] List<ColoredValue<Color>> overrideColors;

            private Dictionary<TileColor, Color> baseColorByTileColor;

            private Color baseColor;
            private Material material;

            float targetIntensity;
            float lastIntensity;
            float interval;
            float timer;

            void Awake()
            {
                this.material = this.GetComponent<MeshRenderer>().material;
                this.baseColorByTileColor = new();
                foreach (ColoredValue<Color> @override in this.overrideColors)
                {
                    this.baseColorByTileColor[@override.GetTileColor()] = @override.GetValue();
                }
            }

            void Update()
            {
                timer += Time.deltaTime;
                if (timer > interval)
                {
                    this.lastIntensity = this.targetIntensity;
                    this.targetIntensity = Random.Range(this.minIntensity, this.maxIntensity);
                    this.timer = 0;
                    this.interval = Random.Range(0, maxInterval);
                }
                this.SetEmissionColor(this.baseColor * Mathf.Lerp(lastIntensity, targetIntensity, timer / interval));
            }

            public void SetColor(TileColor color)
            {
                this.baseColor = System.Instance.GetTileMaterialProvider().GetColor(color);
                if (this.baseColorByTileColor.ContainsKey(color))
                {
                    this.material.SetColor(BASE_COLOR, this.baseColorByTileColor[color]);
                }
                else
                {
                    this.material.SetColor(BASE_COLOR, new Color(this.baseColor.r, this.baseColor.g, this.baseColor.b, .2f));
                }
                this.SetEmissionColor(this.baseColor);
            }

            private void SetEmissionColor(Color color)
            {
                this.material.SetColor(EMISSION_COLOR, color);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Azul
{
    namespace Model
    {
        public class PanelManagerUI : MonoBehaviour
        {
            private Dictionary<string, GameObject> layers = new();


            public void ShowLayer(string name)
            {
                foreach (KeyValuePair<string, GameObject> entry in this.layers)
                {
                    entry.Value.GetComponent<CanvasGroup>().alpha = entry.Key == name ? 1 : 0;
                }
            }

            public GameObject AddLayer(string name)
            {
                GameObject layer = new GameObject(name);
                layer.transform.SetParent(this.gameObject.transform);
                RectTransform rectTransform = layer.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.pivot = new Vector2(.5f, .5f);
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.localScale = Vector3.one;
                CanvasGroup group = layer.AddComponent<CanvasGroup>();
                group.alpha = 0.0f;
                this.layers[name] = layer;
                return layer;
            }

            public GameObject GetLayer(string name)
            {
                if (this.layers.ContainsKey(name))
                {
                    return this.layers[name];
                }
                else
                {
                    return null;
                }
            }

        }
    }
}

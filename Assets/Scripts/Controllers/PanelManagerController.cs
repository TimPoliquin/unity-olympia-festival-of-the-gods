using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class PanelManagerController : MonoBehaviour
        {
            private static readonly string DEFAULT_KEY = "default";

            [SerializeField] private PanelManagerUI panelManagerUI;


            void Awake()
            {
                this.AddLayer(DEFAULT_KEY);
                this.panelManagerUI.ShowLayer(DEFAULT_KEY);
            }

            public void ShowDefaultLayer()
            {
                this.panelManagerUI.ShowLayer(DEFAULT_KEY);
            }

            public void AddToDefaultLayer(GameObject toAdd)
            {
                this.AddToLayer(DEFAULT_KEY, toAdd);
            }

            public void AddToLayer(string name, GameObject toAdd)
            {
                if (name == null)
                {
                    name = DEFAULT_KEY;
                }
                GameObject layer = this.panelManagerUI.GetLayer(name);
                if (null == layer)
                {
                    layer = this.AddLayer(name);
                }
                toAdd.transform.SetParent(layer.transform);
            }

            public GameObject AddLayer(string name)
            {
                return this.panelManagerUI.AddLayer(name);
            }

            public void ShowLayer(string name)
            {
                this.panelManagerUI.ShowLayer(name);
            }


            public void HideLayer(string name)
            {
                if (this.panelManagerUI.GetLayer(name).activeInHierarchy)
                {
                    this.ShowDefaultLayer();
                }
            }
        }
    }
}

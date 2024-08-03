using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.Provider;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public abstract class AbstractMaterialController : MonoBehaviour
        {
            [SerializeField] private GameObject mesh;
            void Start()
            {
                TileMaterialProvider materialProvider = System.Instance.GetTileMaterialProvider();
                this.SetMaterial(this.GetMaterial());
                this.enabled = false;
            }

            protected abstract Material GetMaterial();

            private void SetMaterial(Material material)
            {
                this.mesh.GetComponent<Renderer>().material = material;
            }
        }
    }
}

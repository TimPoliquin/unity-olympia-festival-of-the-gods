using System.Collections;
using System.Collections.Generic;
using Azul.Provider;
using UnityEngine;

namespace Azul
{
    public class System : MonoBehaviour
    {
        public static System Instance { get; private set; }

        private TileMaterialProvider tileMaterialProvider;

        void Awake()
        {
            if (null == Instance)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public TileMaterialProvider GetTileMaterialProvider()
        {
            if (null == this.tileMaterialProvider)
            {
                this.tileMaterialProvider = this.GetComponentInChildren<TileMaterialProvider>();
            }
            return this.tileMaterialProvider;
        }

    }
}

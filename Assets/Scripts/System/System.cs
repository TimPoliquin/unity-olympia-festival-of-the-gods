using System.Collections;
using System.Collections.Generic;
using Azul.Controller;
using Azul.Provider;
using UnityEngine;

namespace Azul
{
    public class System : MonoBehaviour
    {
        public static System Instance { get; private set; }

        private BagController bagController;
        private FactoryController factoryController;
        private PlayerController playerController;
        private TileController tileController;
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

        public BagController GetBagController()
        {
            if (null == this.bagController)
            {
                this.bagController = this.GetComponentInChildren<BagController>();
            }
            return this.bagController;
        }

        public FactoryController GetFactoryController()
        {
            if (null == this.factoryController)
            {
                this.factoryController = this.GetComponentInChildren<FactoryController>();
            }
            return this.factoryController;
        }

        public PlayerController GetPlayerController()
        {
            if (null == this.playerController)
            {
                this.playerController = this.GetComponentInChildren<PlayerController>();
            }
            return this.playerController;
        }

        public TileController GetTileController()
        {
            if (null == this.tileController)
            {
                this.tileController = this.GetComponentInChildren<TileController>();
            }
            return this.tileController;
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

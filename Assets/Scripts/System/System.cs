using System.Collections;
using System.Collections.Generic;
using Azul.Controller;
using Azul.Provider;
using UnityEngine;
using UnityEngine.Rendering;

namespace Azul
{
    public class System : MonoBehaviour
    {
        public static System Instance { get; private set; }

        private BagController bagController;
        private CameraController cameraController;
        private FactoryController factoryController;
        private GameController gameController;
        private PlayerBoardController playerBoardController;
        private PlayerController playerController;
        private RoundController roundController;
        private ScoreBoardController scoreBoardController;
        private StarController starController;
        private TableController tableController;
        private TileController tileController;
        private TileMaterialProvider tileMaterialProvider;
        private UIController uiController;

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

        public CameraController GetCameraController()
        {
            if (null == this.cameraController)
            {
                this.cameraController = this.GetComponentInChildren<CameraController>();
            }
            return this.cameraController;
        }

        public FactoryController GetFactoryController()
        {
            if (null == this.factoryController)
            {
                this.factoryController = this.GetComponentInChildren<FactoryController>();
            }
            return this.factoryController;
        }

        public GameController GetGameController()
        {
            if (null == this.gameController)
            {
                this.gameController = this.GetComponentInChildren<GameController>();
            }
            return this.gameController;
        }

        public PlayerController GetPlayerController()
        {
            if (null == this.playerController)
            {
                this.playerController = this.GetComponentInChildren<PlayerController>();
            }
            return this.playerController;
        }

        public PlayerBoardController GetPlayerBoardController()
        {
            if (null == this.playerBoardController)
            {
                this.playerBoardController = this.GetComponentInChildren<PlayerBoardController>();
            }
            return this.playerBoardController;
        }

        public RoundController GetRoundController()
        {
            if (null == this.roundController)
            {
                this.roundController = this.GetComponentInChildren<RoundController>();
            }
            return this.roundController;
        }

        public ScoreBoardController GetScoreBoardController()
        {
            if (null == this.scoreBoardController)
            {
                this.scoreBoardController = this.GetComponentInChildren<ScoreBoardController>();
            }
            return this.scoreBoardController;
        }

        public StarController GetStarController()
        {
            if (null == this.starController)
            {
                this.starController = this.GetComponentInChildren<StarController>();
            }
            return this.starController;
        }

        public TableController GetTableController()
        {
            if (null == this.tableController)
            {
                this.tableController = this.GetComponentInChildren<TableController>();
            }
            return this.tableController;
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

        public UIController GetUIController()
        {
            if (null == this.uiController)
            {
                this.uiController = this.GetComponentInChildren<UIController>();
            }
            return this.uiController;
        }
    }
}

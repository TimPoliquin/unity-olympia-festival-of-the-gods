using System.Collections;
using System.Collections.Generic;
using Azul.Controller;
using Azul.Model;
using Azul.Provider;
using Steamworks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Azul
{
    public class System : MonoBehaviour
    {
        public static System Instance { get; private set; }

        private AIController aiController;
        private AltarFactory starController;
        private AudioController audioController;
        private BagController bagController;
        private CameraController cameraController;
        private FactoryController factoryController;
        private GameController gameController;
        private GraphicsSettingsController graphicsSettingsController;
        private MilestoneCompletionController milestoneCompletionController;
        private PlayerBoardController playerBoardController;
        private PlayerController playerController;
        private PrefabFactory prefabFactory;
        private RoundController roundController;
        private ScoreBoardController scoreBoardController;
        private ScreenController screenController;
        private TableController tableController;
        private TileController tileController;
        private TileColorMappingController tileColorMappingController;
        private TileAnimationController tileAnimationController;
        private TileMaterialProvider tileMaterialProvider;
        private UIController uiController;

        void Awake()
        {

            if (null != Instance)
            {
                Destroy(Instance);
            }
            Instance = this;
        }

        public AIController GetAIController()
        {
            if (null == this.aiController)
            {
                this.aiController = this.GetComponentInChildren<AIController>();
            }
            return this.aiController;
        }

        public AudioController GetAudioController()
        {
            if (null == this.audioController)
            {
                this.audioController = this.GetComponentInChildren<AudioController>();
            }
            return this.audioController;
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

        public GraphicsSettingsController GetGraphicsSettingsController()
        {
            if (null == this.graphicsSettingsController)
            {
                this.graphicsSettingsController = this.GetComponentInChildren<GraphicsSettingsController>();
            }
            return this.graphicsSettingsController;
        }

        public MilestoneCompletionController GetMilestoneCompletionController()
        {
            if (null == this.milestoneCompletionController)
            {
                this.milestoneCompletionController = this.GetComponentInChildren<MilestoneCompletionController>();
            }
            return this.milestoneCompletionController;
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

        public PrefabFactory GetPrefabFactory()
        {
            if (null == this.prefabFactory)
            {
                this.prefabFactory = this.GetComponentInChildren<PrefabFactory>();
            }
            return this.prefabFactory;
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

        public ScreenController GetScreenController()
        {
            if (null == this.screenController)
            {
                this.screenController = this.GetComponentInChildren<ScreenController>();
            }
            return this.screenController;
        }

        public AltarFactory GetStarController()
        {
            if (null == this.starController)
            {
                this.starController = this.GetComponentInChildren<AltarFactory>();
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

        public TileAnimationController GetTileAnimationController()
        {
            if (null == this.tileAnimationController)
            {
                this.tileAnimationController = this.GetComponentInChildren<TileAnimationController>();
            }
            return this.tileAnimationController;
        }

        public TileColorMappingController GetTileColorMappingController()
        {
            if (null == this.tileColorMappingController)
            {
                this.tileColorMappingController = this.GetComponentInChildren<TileColorMappingController>();
            }
            return this.tileColorMappingController;
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

        public void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        public void Pause()
        {
            Time.timeScale = 0;
        }

        public void Resume()
        {
            Time.timeScale = 1;
        }

        public void PlayAgain()
        {
            SceneManager.LoadScene(0);
        }

        public string GetUsername()
        {
# if !DISABLESTEAMWORKS
            if (SteamManager.Initialized)
            {
                return SteamFriends.GetPersonaName();
            }
# endif
            return "Player 1";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.Prefab;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class PrefabFactory : MonoBehaviour
        {
            [SerializeField] private Canvas canvas;
            [SerializeField] private List<TokenPrefab> tokenPrefabs;
            [SerializeField] private AcquireTilesPanelUI acquireTilesPanelUIPrefab;
            [SerializeField] private ExplosionEffect explosionEffectPrefab;
            [SerializeField] private GodScoreUI godScoreUI;
            [SerializeField] private GrantRewardTilesUI grantRewardTilesUIPrefab;
            [SerializeField] private HadesTokenPanelUI hadesTokenPanelUIPrefab;
            [SerializeField] private HelpPanelUI helpPanelUIPrefab;
            [SerializeField] private MilestoneCompletedPanelUI milestoneCompletedPanelUIPrefab;
            [SerializeField] private OptionsPanelUI optionsPanelUIPrefab;
            [SerializeField] private PlayerUI playerUIPrefab;
            [SerializeField] private Camera playerBoardPreviewCamera;
            [SerializeField] private PlayerBoardPreviewUI playerBoardPreviewUIPrefab;
            [SerializeField] private PlayerConfigUI playerConfigUIPrefab;
            [SerializeField] private PlayerTurnBannerUI playerTurnBannerUIPrefab;
            [SerializeField] private PreviewingBannerUI previewBannerUIPrefab;
            [SerializeField] private RewardPreviewPanelUI rewardPreviewPanelUIPrefab;
            [SerializeField] private RewardProgressFieldUI rewardProgressFieldUIPrefab;
            [SerializeField] private RewardRitualUI rewardRitualUIPrefab;
            [SerializeField] private RoundStartUI roundStartUIPrefab;
            [SerializeField] private RitualScoreUI ritualScoreUIPrefab;
            [SerializeField] private ScoreTileSelectionPanelUI scoreTileSelectionPanelUIPrefab;
            [SerializeField] private ScoreTileSelectionUI scoreTileSelectionUIPrefab;
            [SerializeField] private ScoreTilesPreviewPanelUI scoreTilesPreviewPanelUIPrefab;
            [SerializeField] private PlayerTileCountUI tileCountUIPrefab;
            [SerializeField] private UnavailableTokensPanelUI unavailableTokensPanelUIPrefab;
            [SerializeField] private WildColorSelectionUI wildColorSelectionUIPrefab;

            private Dictionary<TileColor, Tile> tilePrefabsByColor;
            private AltarFactory altarFactory;
            private PanelManagerController panelManagerController;

            void Awake()
            {
                this.tilePrefabsByColor = new Dictionary<TileColor, Tile>();
                foreach (TokenPrefab tokenPrefab in this.tokenPrefabs)
                {
                    this.tilePrefabsByColor[tokenPrefab.GetTileColor()] = tokenPrefab.GetPrefab();
                }
            }

            public AltarFactory GetAltarFactory()
            {
                if (null == this.altarFactory)
                {
                    this.altarFactory = this.GetComponentInChildren<AltarFactory>();
                }
                return this.altarFactory;
            }

            public Tile CreateTile(TileColor tileColor)
            {
                return Instantiate(this.tilePrefabsByColor[tileColor]);
            }

            public Altar CreateAltar(TileColor tileColor, float rotation)
            {
                return this.altarFactory.CreateAltar(tileColor, rotation);
            }

            public AcquireTilesPanelUI CreateAcquireTilesPanelUI()
            {
                AcquireTilesPanelUI panel = Instantiate(this.acquireTilesPanelUIPrefab, this.canvas.transform);
                this.GetPanelManagerController().AddToDefaultLayer(panel.gameObject);
                return panel;

            }

            public ExplosionEffect CreateExplosionEffect(Transform parent = null)
            {
                return Instantiate(this.explosionEffectPrefab, parent);
            }

            public GodScoreUI CreateGodScoreUI()
            {
                return Instantiate(this.godScoreUI, this.canvas.transform);
            }

            public GrantRewardTilesUI CreateGrantRewardTilesUI()
            {
                GrantRewardTilesUI panel = Instantiate(this.grantRewardTilesUIPrefab, this.canvas.transform);
                this.GetPanelManagerController().AddToDefaultLayer(panel.gameObject);
                return panel;
            }

            public HadesTokenPanelUI CreateHadesTokenPanelUI()
            {
                HadesTokenPanelUI panel = Instantiate(this.hadesTokenPanelUIPrefab, this.canvas.transform);
                this.GetPanelManagerController().AddToDefaultLayer(panel.gameObject);
                return panel;
            }

            public HelpPanelUI CreateHelpPanelUI()
            {
                HelpPanelUI panel = Instantiate(this.helpPanelUIPrefab, this.canvas.transform);
                this.GetPanelManagerController().AddToDefaultLayer(panel.gameObject);
                return panel;
            }

            public MilestoneCompletedPanelUI CreateMilestoneCompletedPanelUI()
            {
                return Instantiate(this.milestoneCompletedPanelUIPrefab, this.canvas.transform);
            }

            public OptionsPanelUI CreateOptionsPanelUI()
            {
                return Instantiate(this.optionsPanelUIPrefab, this.canvas.transform);
            }

            public PlayerUI CreatePlayerUI()
            {
                return Instantiate(this.playerUIPrefab);
            }

            public Camera CreatePlayerBoardPreviewCamera()
            {
                return Instantiate(this.playerBoardPreviewCamera);
            }

            public PlayerBoardPreviewUI CreatePlayerBoardPreviewUI(Transform parent)
            {
                return Instantiate(this.playerBoardPreviewUIPrefab, parent ? parent : this.canvas.transform);
            }

            public PlayerConfigUI CreatePlayerConfigUI(Transform parent)
            {
                return Instantiate(this.playerConfigUIPrefab, parent);
            }

            public PlayerTurnBannerUI CreatePlayerTurnBannerUI(string layer = null)
            {
                PlayerTurnBannerUI panel = Instantiate(this.playerTurnBannerUIPrefab, this.canvas.transform);
                this.GetPanelManagerController().AddToLayer(layer, panel.gameObject);
                return panel;
            }

            public PreviewingBannerUI CreatePreviewBannerUI(string layer = null)
            {
                PreviewingBannerUI panel = Instantiate(this.previewBannerUIPrefab, this.canvas.transform);
                this.GetPanelManagerController().AddToLayer(layer, panel.gameObject);
                return panel;
            }

            public RewardPreviewPanelUI CreateRewardPreviewPanelUI()
            {
                return Instantiate(this.rewardPreviewPanelUIPrefab, this.canvas.transform);
            }

            public RewardProgressFieldUI CreateRewardProgressFieldUI()
            {
                return Instantiate(this.rewardProgressFieldUIPrefab, this.canvas.transform);
            }
            public RewardRitualUI CreateRewardRitualUI()
            {
                return Instantiate(this.rewardRitualUIPrefab, this.canvas.transform);
            }

            public RoundStartUI CreateRoundStartUI(string layer = null)
            {
                RoundStartUI roundStartUI = Instantiate(this.roundStartUIPrefab, this.canvas.transform);
                this.GetPanelManagerController().AddToLayer(layer, roundStartUI.gameObject);
                return roundStartUI;
            }

            public RitualScoreUI CreateRitualScoreUI()
            {
                return Instantiate(this.ritualScoreUIPrefab, this.canvas.transform);
            }

            public ScoreTilesPreviewPanelUI CreateScoreTilesPreviewPanelUI()
            {
                return Instantiate(this.scoreTilesPreviewPanelUIPrefab, this.canvas.transform);
            }

            public ScoreTileSelectionPanelUI CreateScoreTileSelectionPanelUI()
            {
                return Instantiate(this.scoreTileSelectionPanelUIPrefab, this.canvas.transform);
            }

            public ScoreTileSelectionUI CreateScoreTileSelectionUI(ScoreTileSelectionUIContainer panel)
            {
                ScoreTileSelectionUI scoreTileSelectionUI = Instantiate(this.scoreTileSelectionUIPrefab, this.canvas.transform);
                panel.AddScoreTileSelectionUI(scoreTileSelectionUI);
                return scoreTileSelectionUI;
            }

            public PlayerTileCountUI CreatePlayerTileCountUI(TileColor tileColor, Transform parent)
            {
                PlayerTileCountUI playerTileCountUI = Instantiate(this.tileCountUIPrefab, parent ? parent : this.canvas.transform);
                playerTileCountUI.SetTileColor(tileColor);
                return playerTileCountUI;
            }

            public UnavailableTokensPanelUI CreateUnavailableTokensPanelUI(string layer = null)
            {
                PanelManagerController panelManagerController = this.GetPanelManagerController();
                UnavailableTokensPanelUI panelUI = Instantiate(this.unavailableTokensPanelUIPrefab, this.canvas.transform);
                panelManagerController.AddToLayer(layer, panelUI.gameObject);
                return panelUI;
            }

            public WildColorSelectionUI CreateWildColorSelectionUI(Transform parent = null)
            {
                return Instantiate(this.wildColorSelectionUIPrefab, parent ? parent : this.canvas.transform);
            }

            public PanelManagerController GetPanelManagerController()
            {
                if (null == this.panelManagerController)
                {
                    this.panelManagerController = System.Instance.GetUIController().GetPanelManagerController();
                }
                return this.panelManagerController;
            }
        }
    }
}

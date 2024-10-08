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
            [SerializeField] private MilestoneCompletedPanelUI milestoneCompletedPanelUIPrefab;
            [SerializeField] private PlayerUI playerUIPrefab;
            [SerializeField] private PlayerTurnBannerUI playerTurnBannerUIPrefab;
            [SerializeField] private RewardProgressFieldUI rewardProgressFieldUIPrefab;
            [SerializeField] private RoundStartUI roundStartUIPrefab;
            [SerializeField] private RitualScoreUI ritualScoreUIPrefab;
            [SerializeField] private ScoreTileSelectionPanelUI scoreTileSelectionPanelUIPrefab;
            [SerializeField] private ScoreTileSelectionUI scoreTileSelectionUIPrefab;
            [SerializeField] private ScoreTilesPreviewPanelUI scoreTilesPreviewPanelUIPrefab;
            [SerializeField] private PlayerTileCountUI tileCountUIPrefab;
            [SerializeField] private WildColorSelectionUI wildColorSelectionUIPrefab;

            private Dictionary<TileColor, Tile> tilePrefabsByColor;
            private AltarFactory altarFactory;

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
                return Instantiate(this.acquireTilesPanelUIPrefab, this.canvas.transform);
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
                return Instantiate(this.grantRewardTilesUIPrefab, this.canvas.transform);
            }

            public HadesTokenPanelUI CreateHadesTokenPanelUI()
            {
                return Instantiate(this.hadesTokenPanelUIPrefab, this.canvas.transform);
            }

            public MilestoneCompletedPanelUI CreateMilestoneCompletedPanelUI()
            {
                return Instantiate(this.milestoneCompletedPanelUIPrefab, this.canvas.transform);
            }

            public PlayerUI CreatePlayerUI()
            {
                return Instantiate(this.playerUIPrefab);
            }

            public PlayerTurnBannerUI CreatePlayerTurnBannerUI()
            {
                return Instantiate(this.playerTurnBannerUIPrefab, this.canvas.transform);
            }

            public RewardProgressFieldUI CreateRewardProgressFieldUI()
            {
                return Instantiate(this.rewardProgressFieldUIPrefab, this.canvas.transform);
            }

            public RoundStartUI CreateRoundStartUI()
            {
                return Instantiate(this.roundStartUIPrefab, this.canvas.transform);
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

            public WildColorSelectionUI CreateWildColorSelectionUI(Transform parent = null)
            {
                return Instantiate(this.wildColorSelectionUIPrefab, parent ? parent : this.canvas.transform);
            }
        }
    }
}

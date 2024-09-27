using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using Azul.Prefab;
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
            [SerializeField] private PlayerUI playerUIPrefab;
            [SerializeField] private RitualScoreUI ritualScoreUIPrefab;
            [SerializeField] private ScoreTileSelectionPanelUI scoreTileSelectionPanelUIPrefab;
            [SerializeField] private ScoreTileSelectionUI scoreTileSelectionUIPrefab;
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

            public PlayerUI CreatePlayerUI()
            {
                return Instantiate(this.playerUIPrefab);
            }

            public RitualScoreUI CreateRitualScoreUI()
            {
                return Instantiate(this.ritualScoreUIPrefab, this.canvas.transform);
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

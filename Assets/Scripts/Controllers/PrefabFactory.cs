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
            [SerializeField] private PlayerUI playerUIPrefab;
            [SerializeField] private ScoreTileSelectionPanelUI scoreTileSelectionPanelUIPrefab;
            [SerializeField] private ScoreTileSelectionUI scoreTileSelectionUIPrefab;
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

            public PlayerUI CreatePlayerUI()
            {
                return Instantiate(this.playerUIPrefab);
            }

            public ScoreTileSelectionPanelUI CreateScoreTileSelectionPanelUI()
            {
                return Instantiate(this.scoreTileSelectionPanelUIPrefab, this.canvas.transform);
            }

            public ScoreTileSelectionUI CreateScoreTileSelectionUI(ScoreTileSelectionPanelUI panel)
            {
                ScoreTileSelectionUI scoreTileSelectionUI = Instantiate(this.scoreTileSelectionUIPrefab, this.canvas.transform);
                panel.AddScoreTileSelectionUI(scoreTileSelectionUI);
                return scoreTileSelectionUI;

            }

            public WildColorSelectionUI CreateWildColorSelectionUI(Transform parent = null)
            {
                return Instantiate(this.wildColorSelectionUIPrefab, parent ? parent : this.canvas.transform);
            }

        }
    }
}

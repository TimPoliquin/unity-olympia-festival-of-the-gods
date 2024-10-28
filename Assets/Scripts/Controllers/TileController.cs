using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Azul.Model;
using Azul.GameEvents;
using Azul.RoundEvents;
using Azul.TableEvents;
using Azul.Controller.TableEvents;
using Azul.Event;

namespace Azul
{
    namespace Controller
    {
        public class TileController : MonoBehaviour
        {
            private static int NUM_TILES = 132;

            /// <summary>
            /// A special tile. This tile indicates which player goest first in the next steps of the current round,
            /// and which player goes first in the following round.
            /// </summary>
            private Tile oneTile;
            private List<Tile> tiles;

            private int var_ShaderWildEnabled = Shader.PropertyToID("_Enabled");
            private int var_ShaderOffset = Shader.PropertyToID("_Offset");

            public void SetupGame()
            {
                this.CreateStandardTiles();
                this.CreateOneTile();
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnGameSetupComplete);
            }

            private void CreateStandardTiles()
            {
                this.tiles = new();
                TileColor[] tileColors = TileColorUtils.GetTileColors();
                for (int idx = 0; idx < NUM_TILES; idx++)
                {
                    TileColor tileColor = tileColors[idx % tileColors.Length];
                    this.CreateTile(tileColor);
                }
                tiles.Shuffle();
            }

            private void CreateOneTile()
            {
                PrefabFactory prefabFactory = System.Instance.GetPrefabFactory();
                this.oneTile = prefabFactory.CreateTile(TileColor.ONE);
            }


            public List<Tile> GetTiles()
            {
                return this.tiles;
            }

            public Tile GetOneTile()
            {
                return this.oneTile;
            }

            private void OnGameSetupComplete(OnGameSetupCompletePayload payload)
            {
                TableController tableController = System.Instance.GetTableController();
                tableController.MoveOneTileToCenter(this.oneTile);
                System.Instance.GetRoundController().AddOnBeforeRoundStartListener(this.OnRoundPrepare);
                System.Instance.GetPlayerController().AddOnPlayerTurnStartListener(this.OnPlayerTurnStart);
                System.Instance.GetTableController().AddOnTilesDrawnListener(this.OnTableTilesDrawn);
                System.Instance.GetFactoryController().AddOnFactoryTilesDrawnListener(this.OnFactoryTilesDrawn);
            }

            public Tile CreateTile(TileColor tileColor)
            {
                Tile tile = System.Instance.GetPrefabFactory().CreateTile(tileColor);
                tile.GetTokenRenderer().material.SetVector(this.var_ShaderOffset, new Vector2(this.tiles.Count, this.tiles.Count));
                tile.GetTilePointerController().SetInteractable(false);
                this.tiles.Add(tile);
                return tile;
            }

            private void OnRoundPrepare(OnBeforeRoundStartPayload payload)
            {
                foreach (Tile tile in this.tiles)
                {
                    tile.GetTokenRenderer().material.SetInt(this.var_ShaderWildEnabled, tile.Color == payload.WildColor ? 1 : 0);
                }
            }

            public void OnPlayerTurnStart(OnPlayerTurnStartPayload payload)
            {
                bool enablePointer = payload.Player.IsHuman() && payload.Phase == Phase.ACQUIRE;
                foreach (Tile tile in this.tiles)
                {
                    if (tile.gameObject.activeInHierarchy)
                    {
                        tile.GetTilePointerController().SetInteractable(enablePointer);
                    }
                }
                this.oneTile.GetTilePointerController().SetInteractable(enablePointer);
            }

            public void OnTableTilesDrawn(EventTracker<OnTableTilesDrawnPayload> eventTracker)
            {
                // Disable all tiles
                this.DisableAllTilePointerActions();
                eventTracker.Done();
            }

            public void OnFactoryTilesDrawn(EventTracker<OnFactoryTilesDrawnPayload> eventTracker)
            {
                this.DisableAllTilePointerActions();
                eventTracker.Done();
            }

            private void DisableAllTilePointerActions()
            {
                foreach (Tile tile in this.tiles)
                {
                    tile.GetTilePointerController().SetInteractable(false);
                }
                this.oneTile.GetTilePointerController().SetInteractable(false);
            }
        }
    }
}

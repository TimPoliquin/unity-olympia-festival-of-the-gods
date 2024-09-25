using System.Collections;
using System.Collections.Generic;
using Azul.GameEvents;
using Azul.Model;
using Azul.TileHolderEvents;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class AcquireTilesPanelUIController : MonoBehaviour
        {
            [SerializeField] private Vector3 offset = Vector3.right * 10.0f;
            private AcquireTilesPanelUI acquireTilesPanel;

            void Start()
            {
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.InitializeListeners);
            }

            private void InitializeListeners(OnGameSetupCompletePayload payload)
            {
                TableController tableController = System.Instance.GetTableController();
                tableController.AddOnTilesHoverEnterListener(this.OnHoverTiles);
                tableController.AddOnTilesHoverExitListener(this.OnLeaveTiles);
                tableController.AddOnTileSelectListener(this.OnSelectTiles);
            }

            private void OnHoverTiles(OnTileHoverEnterPayload payload)
            {
                this.Hide();
                if (System.Instance.GetPlayerController().GetCurrentPlayer().IsHuman())
                {
                    this.acquireTilesPanel = System.Instance.GetPrefabFactory().CreateAcquireTilesPanelUI();
                    Camera camera = System.Instance.GetCameraController().GetMainCamera();
                    this.acquireTilesPanel.transform.position = camera.WorldToScreenPoint(payload.Tile.transform.position) + this.offset;
                    this.acquireTilesPanel.Show(payload.TilesHovered);
                }
            }

            private void OnSelectTiles(OnTileSelectPayload payload)
            {
                this.Hide();
            }

            private void OnLeaveTiles(OnTileHoverExitPayload payload)
            {
                this.Hide();
            }

            private void Hide()
            {
                if (this.acquireTilesPanel != null)
                {
                    this.acquireTilesPanel.Hide();
                    this.acquireTilesPanel = null;
                }
            }
        }
    }
}

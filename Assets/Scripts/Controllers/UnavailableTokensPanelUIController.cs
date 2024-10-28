using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Cursor;
using Azul.GameEvents;
using Azul.Model;
using Azul.TileHolderEvents;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Azul
{
    namespace Controller
    {
        [RequireComponent(typeof(CursorChange))]
        public class UnavailableTokensPanelUIController : MonoBehaviour
        {
            [SerializeField] private Vector3 offset;
            private UnavailableTokensPanelUI panel;
            private CursorChange cursorChange;
            void Start()
            {
                System.Instance.GetGameController().AddOnGameSetupCompleteListener(this.OnGameSetupComplete);
                this.cursorChange = this.GetComponent<CursorChange>();
            }

            private void OnGameSetupComplete(OnGameSetupCompletePayload payload)
            {
                System.Instance.GetTableController().AddOnUnavailableTokenHoverEnterListener(this.OnUnavailableTokenHoverEnter);
                System.Instance.GetTableController().AddOnTilesHoverExitListener(this.OnTokenHoverExit);
                System.Instance.GetFactoryController().AddOnUnavailableTokenHoverEnterListener(this.OnUnavailableTokenHoverEnter);
                System.Instance.GetFactoryController().AddOnTokenHoverExitListener(this.OnTokenHoverExit);
            }

            private void OnUnavailableTokenHoverEnter(OnUnavailableTokenHoverEnter payload)
            {
                if (null != this.panel)
                {
                    this.StartCoroutine(this.Hide(this.panel));
                    this.panel = null;
                }
                if (payload.Tile.IsHadesToken())
                {
                    this.StartCoroutine(this.ShowHadesTokenUnavailable(payload.Tile));
                    this.cursorChange.OnHoverEnter();
                }
                else if (payload.Tile.IsWild())
                {
                    this.StartCoroutine(this.ShowWildTokenUnavailable(payload.Tile));
                    this.cursorChange.OnHoverEnter();
                }
            }
            private void OnTokenHoverExit(OnTileHoverExitPayload payload)
            {
                if (null != this.panel)
                {
                    this.StartCoroutine(this.Hide(this.panel));
                    this.panel = null;
                    this.cursorChange.OnHoverExit();
                }
            }

            private IEnumerator ShowHadesTokenUnavailable(Tile tile)
            {
                yield return this.CreatePanel(tile).ShowHadesUnavailable().WaitUntilCompleted();
            }

            private IEnumerator ShowWildTokenUnavailable(Tile tile)
            {
                yield return this.CreatePanel(tile).ShowWildUnavailable(System.Instance.GetTileColorMappingController().GetGodName(tile.Color)).WaitUntilCompleted();
            }

            private IEnumerator Hide(UnavailableTokensPanelUI panel)
            {
                yield return panel.Hide().WaitUntilCompleted();
                Destroy(panel.gameObject);
            }

            private UnavailableTokensPanelUI CreatePanel(Tile tile)
            {
                this.panel = System.Instance.GetPrefabFactory().CreateUnavailableTokensPanelUI();
                this.panel.transform.position = System.Instance.GetCameraController().GetMainCamera().WorldToScreenPoint(tile.transform.position) + this.offset;
                return this.panel;
            }

        }
    }
}

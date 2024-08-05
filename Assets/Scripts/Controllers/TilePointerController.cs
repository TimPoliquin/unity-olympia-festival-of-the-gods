using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Azul.Model;
using UnityEngine.Events;

namespace Azul
{
    namespace Controller
    {

        public class OnTileHoverEnterPayload
        {
            public Tile Tile { get; init; }
        }

        public class OnTileHoverExitPayload
        {
            public Tile Tile { get; init; }
        }

        public class OnTileSelectPayload
        {
            public Tile Tile { get; init; }
        }

        [RequireComponent(typeof(Tile))]
        public class TilePointerController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
        {
            private UnityEvent<OnTileHoverEnterPayload> onTileHoverEnter = new();
            private UnityEvent<OnTileHoverExitPayload> onTileHoverExit = new();
            private UnityEvent<OnTileSelectPayload> onTileSelect = new();

            private Tile tile;

            void Awake()
            {
                this.tile = this.GetComponent<Tile>();
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                this.onTileHoverEnter.Invoke(new OnTileHoverEnterPayload { Tile = this.tile });
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                this.onTileHoverExit.Invoke(new OnTileHoverExitPayload { Tile = this.tile });
            }
            public void OnPointerDown(PointerEventData eventData)
            {
                this.onTileSelect.Invoke(new OnTileSelectPayload { Tile = this.tile });
            }

            public void AddOnTileHoverEnterListener(UnityAction<OnTileHoverEnterPayload> listener)
            {
                this.onTileHoverEnter.AddListener(listener);
            }
            public void AddOnTileHoverExitListener(UnityAction<OnTileHoverExitPayload> listener)
            {
                this.onTileHoverExit.AddListener(listener);
            }
            public void AddOnTileSelectListener(UnityAction<OnTileSelectPayload> listener)
            {
                this.onTileSelect.AddListener(listener);
            }

            public void RemoveOnTileHoverEnterListener(UnityAction<OnTileHoverEnterPayload> listener)
            {
                this.onTileHoverEnter.RemoveListener(listener);
            }

            public void RemoveOnTileHoverExitListener(UnityAction<OnTileHoverExitPayload> listener)
            {
                this.onTileHoverExit.RemoveListener(listener);
            }

            public void RemoveOnTileSelectListener(UnityAction<OnTileSelectPayload> listener)
            {
                this.onTileSelect.RemoveListener(listener);
            }
        }
    }
}

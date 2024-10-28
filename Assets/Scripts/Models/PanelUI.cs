using Azul.Controller;
using Azul.Cursor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Azul
{
    namespace Model
    {
        public class PanelUI : MonoBehaviour
        {
            private struct DragEvent
            {
                public Vector3 Position { get; init; }
            }
            private class DragManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
            {
                private RectTransform rectTransform;
                public UnityEvent<DragEvent> onDrag = new();
                public UnityEvent onHover = new();
                public UnityEvent onExit = new();
                private bool dragging = false;
                private Vector2 mouseStartPosition;
                private Vector3 panelStartPosition;

                void Awake()
                {
                    this.rectTransform = this.GetComponent<RectTransform>();
                }

                public void OnPointerDown(PointerEventData eventData)
                {
                    this.dragging = true;
                    this.mouseStartPosition = eventData.position;
                    this.panelStartPosition = this.rectTransform.position;
                }

                public void OnPointerUp(PointerEventData eventData)
                {
                    this.dragging = false;
                }
                public void OnPointerMove(PointerEventData eventData)
                {
                    if (this.dragging)
                    {
                        Vector3 delta = this.mouseStartPosition - eventData.position;
                        this.onDrag.Invoke(new DragEvent
                        {
                            Position = this.panelStartPosition - delta,
                        });
                    }
                }

                public void OnPointerEnter(PointerEventData eventData)
                {
                    this.onHover.Invoke();
                }

                public void OnPointerExit(PointerEventData eventData)
                {
                    this.onExit.Invoke();
                }
            }
            [SerializeField] private GameObject dragHandle;
            [SerializeField] private GameObject dragRoot;
            private RectTransform rootTransform;

            void Start()
            {
                if (null != this.dragRoot && null != this.dragHandle)
                {
                    CursorChange cursorChange = this.GetComponent<CursorChange>();
                    DragManager dragManager = this.dragHandle.AddComponent<DragManager>();
                    dragManager.onDrag.AddListener(this.OnDrag);
                    dragManager.onHover.AddListener(cursorChange.OnHoverEnter);
                    dragManager.onExit.AddListener(cursorChange.OnHoverExit);
                    this.rootTransform = dragRoot.GetComponent<RectTransform>();
                }
            }

            public void Show()
            {
                this.gameObject.SetActive(true);
            }

            public void Hide()
            {
                this.gameObject.SetActive(false);
            }

            void OnDrag(DragEvent payload)
            {
                RectTransform canvasRect = this.rootTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
                Vector3 canvasScale = canvasRect.localScale;
                Vector3 newPosition = this.dragRoot.transform.position;
                if (payload.Position.y - canvasScale.y * this.rootTransform.rect.height < 0)
                {
                    // lower bound
                }
                else if (Screen.height - payload.Position.y < canvasScale.y * 106)
                {
                    // upper bound
                }
                else
                {
                    newPosition.y = payload.Position.y;
                }
                if (payload.Position.x - canvasScale.x * (this.rootTransform.rect.width / 2f) < 0)
                {
                    // left bound
                }
                else if (Screen.width - payload.Position.x < canvasScale.x * (this.rootTransform.rect.width / 2f))
                {
                    // right bound
                }
                else
                {
                    newPosition.x = payload.Position.x;
                }
                this.dragRoot.transform.position = newPosition;
            }
        }
    }
}

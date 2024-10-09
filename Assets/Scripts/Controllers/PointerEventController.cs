using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Azul.Model;
using Azul.PointerEvents;
using Azul.Cursor;
using UnityEngine.Events;
using UnityEditor;

namespace Azul
{
    namespace PointerEvents
    {
        public class OnPointerEnterPayload<T>
        {
            public T Target { get; init; }
        }

        public class OnPointerExitPayload<T>
        {
            public T Target { get; init; }
        }

        public class OnPointerSelectPayload<T>
        {
            public T Target { get; init; }
        }
    }
    namespace Controller
    {
        [RequireComponent(typeof(CursorChange))]
        public abstract class PointerEventController<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
        {
            private UnityEvent<OnPointerEnterPayload<T>> onPointerEnter = new();
            private UnityEvent<OnPointerExitPayload<T>> onPointerExit = new();
            private UnityEvent<OnPointerSelectPayload<T>> onPointerSelect = new();

            private T target;

            private bool allowEvents = false;

            void Awake()
            {
                this.target = this.GetComponent<T>();
                PlayerController playerController = System.Instance.GetPlayerController();
                playerController.AddOnPlayerTurnStartListener((payload) =>
                {
                    this.allowEvents = payload.Player.IsHuman();
                });
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                if (this.allowEvents)
                {
                    this.onPointerEnter.Invoke(new OnPointerEnterPayload<T> { Target = this.target });
                }
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                if (this.allowEvents)
                {
                    this.onPointerExit.Invoke(new OnPointerExitPayload<T> { Target = this.target });
                }
            }
            public void OnPointerDown(PointerEventData eventData)
            {
                if (this.allowEvents)
                {
                    this.onPointerSelect.Invoke(new OnPointerSelectPayload<T> { Target = this.target });
                }
            }

            public void AddOnPointerEnterListener(UnityAction<OnPointerEnterPayload<T>> listener)
            {
                this.onPointerEnter.AddListener(listener);
            }
            public void AddOnPointerExitListener(UnityAction<OnPointerExitPayload<T>> listener)
            {
                this.onPointerExit.AddListener(listener);
            }
            public void AddOnPointerSelectListener(UnityAction<OnPointerSelectPayload<T>> listener)
            {
                this.onPointerSelect.AddListener(listener);
            }

            public void RemoveOnPointerEnterListener(UnityAction<OnPointerEnterPayload<T>> listener)
            {
                this.onPointerEnter.RemoveListener(listener);
            }

            public void RemoveOnPointerExitListener(UnityAction<OnPointerExitPayload<T>> listener)
            {
                this.onPointerExit.RemoveListener(listener);
            }

            public void RemoveOnPointerSelectListener(UnityAction<OnPointerSelectPayload<T>> listener)
            {
                this.onPointerSelect.RemoveListener(listener);
            }

            public void ClearOnPointerSelectListeners()
            {
                this.onPointerSelect.RemoveAllListeners();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.PreviewEvents;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Azul
{
    namespace PreviewEvents
    {
        public struct OnZoomPayload
        {
            public int PlayerNumber { get; init; }
        }
        public struct OnPreviewSelectionChangePayload
        {
            public int PlayerNumber { get; init; }
        }
    }
    namespace Model
    {
        public class PlayerBoardPreviewUI : MonoBehaviour
        {
            [SerializeField] private RawImage image;
            [SerializeField] private Button previewButton;

            private int playerNumber;

            private UnityEvent<OnZoomPayload> onZoomIn = new();

            void Awake()
            {
                this.previewButton.onClick.AddListener(this.OnPreview);
            }

            public void SetPlayerNumber(int playerNumber)
            {
                this.playerNumber = playerNumber;
            }

            public void SetTexture(RenderTexture texture)
            {
                this.image.texture = texture;
            }

            private void OnPreview()
            {
                this.onZoomIn.Invoke(new OnZoomPayload
                {
                    PlayerNumber = this.playerNumber
                });
            }

            public void AddOnZoomInListener(UnityAction<OnZoomPayload> listener)
            {
                this.onZoomIn.AddListener(listener);
            }
        }
    }
}

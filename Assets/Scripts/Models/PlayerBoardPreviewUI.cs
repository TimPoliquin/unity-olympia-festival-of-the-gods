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
            [SerializeField] private Toggle previewButton;

            [SerializeField] private TMP_Dropdown players;
            private List<string> playerNames;
            private UnityEvent<OnZoomPayload> onZoomIn = new();
            private UnityEvent<OnZoomPayload> onZoomOut = new();
            private UnityEvent<OnPreviewSelectionChangePayload> onSelectionChange = new();

            void Awake()
            {
                this.previewButton.onValueChanged.AddListener(this.OnClick);
                this.players.onValueChanged.AddListener(this.OnSelectionChange);
            }


            public void SetPlayerNames(List<Player> playerNames)
            {
                this.playerNames = playerNames.Select(player => player.GetPlayerName()).ToList();
                this.players.AddOptions(playerNames.Select(player => new TMP_Dropdown.OptionData(player.GetPlayerName())).ToList());
            }

            public void SetActivePlayer(int playerNumber)
            {
                this.players.options[this.players.value].text = this.playerNames[this.players.value];
                this.players.options[playerNumber].text = $"{this.playerNames[playerNumber]} (Current)";
                this.players.value = playerNumber;
            }

            private void OnClick(bool value)
            {
                this.previewButton.GetComponentInChildren<Text>().text = $"Zoom {(value ? "-" : " + ")}";
                if (value)
                {
                    this.onZoomIn.Invoke(new OnZoomPayload
                    {
                        PlayerNumber = this.players.value
                    });
                }
                else
                {
                    this.onZoomOut.Invoke(new OnZoomPayload
                    {
                        PlayerNumber = this.players.value
                    });
                }
            }

            private void OnSelectionChange(int selection)
            {
                this.onSelectionChange.Invoke(new OnPreviewSelectionChangePayload
                {
                    PlayerNumber = selection
                });
                this.previewButton.isOn = false;
            }

            public void AddOnZoomInListener(UnityAction<OnZoomPayload> listener)
            {
                this.onZoomIn.AddListener(listener);
            }
            public void AddOnZoomOutListener(UnityAction<OnZoomPayload> listener)
            {
                this.onZoomOut.AddListener(listener);
            }

            public void AddOnPreviewSelectionChangeListener(UnityAction<OnPreviewSelectionChangePayload> listener)
            {
                this.onSelectionChange.AddListener(listener);
            }
        }
    }
}

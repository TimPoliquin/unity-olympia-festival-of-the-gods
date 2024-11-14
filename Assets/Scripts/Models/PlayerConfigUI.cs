using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Model;
using Azul.PlayerConfigEvents;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Azul
{
    namespace PlayerConfigEvents
    {
        public class PlayerConfigOnChangePayload
        {
            public int PlayerNumber { get; init; }
            public string PlayerName { get; init; }
            public PlayerType PlayerType { get; init; }
            public bool IsValid { get; init; }
        }
    }

    namespace Model
    {
        public struct PlayerConfig
        {
            public int PlayerNumber;
            public string Name;
            public PlayerType PlayerType;
        }
        public class PlayerConfigUI : MonoBehaviour
        {
            [SerializeField] private TMP_InputField playerNameInput;
            [SerializeField] private TMP_Dropdown playerType;
            [SerializeField] private int playerNumber;

            private UnityEvent<PlayerConfigOnChangePayload> onChange = new();

            void Awake()
            {
                (this.playerNameInput.placeholder as TextMeshProUGUI).text = $"Player {this.playerNumber + 1} Name";
                this.playerType.options = PlayerTypeUtils.GetPlayerTypes().Select(playerType => new TMP_Dropdown.OptionData(PlayerTypeUtils.GetPlayerTypeName(playerType))).ToList();
                this.InitializeListeners();
            }

            void InitializeListeners()
            {
                this.playerNameInput.onValueChanged.AddListener(this.OnChangePlayerName);
                this.playerType.onValueChanged.AddListener(this.OnChangePlayerType);
            }

            public PlayerConfig GetPlayerConfig()
            {
                return new PlayerConfig
                {
                    PlayerNumber = this.playerNumber,
                    Name = this.playerNameInput.text,
                    PlayerType = (PlayerType)this.playerType.value
                };
            }

            public void SetPlayerNumber(int playerNumber)
            {
                this.playerNumber = playerNumber;
            }

            public void SetPlayerType(PlayerType playerType)
            {
                this.playerType.value = (int)playerType;
            }

            public void SetPlayerTypeInteractable(bool interactable)
            {
                this.playerType.interactable = interactable;
            }

            public void AddOnPlayerConfigChangeListener(UnityAction<PlayerConfigOnChangePayload> listener)
            {
                this.onChange.AddListener(listener);
            }

            public bool IsValid()
            {
                return this.playerNameInput.text.Trim().Length > 0;
            }

            public bool IsActive()
            {
                return this.gameObject.activeInHierarchy;
            }

            public void SetPlayerName(string name)
            {
                this.playerNameInput.text = name;
            }

            private void OnChangePlayerName(string playerName)
            {
                this.InvokeChangeEvent();
            }

            private void OnChangePlayerType(int playerType)
            {
                this.InvokeChangeEvent();
            }

            private void InvokeChangeEvent()
            {
                this.onChange.Invoke(new PlayerConfigOnChangePayload
                {
                    PlayerNumber = this.playerNumber,
                    PlayerName = this.playerNameInput.text,
                    PlayerType = (PlayerType)this.playerType.value,
                    IsValid = this.IsValid()
                });
            }

            public TMP_InputField GetInput()
            {
                return this.playerNameInput;
            }


        }
    }
}

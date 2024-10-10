using System.Collections;
using System.Collections.Generic;
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
            public bool IsAI { get; init; }
            public bool IsValid { get; init; }
        }
    }

    namespace Model
    {
        public struct PlayerConfig
        {
            public int PlayerNumber;
            public string Name;
            public bool isAI;
        }
        public class PlayerConfigUI : MonoBehaviour
        {
            [SerializeField] private TMP_InputField playerNameInput;
            [SerializeField] private Toggle isAIToggle;
            [SerializeField] private int playerNumber;

            private UnityEvent<PlayerConfigOnChangePayload> onChange = new();

            void Awake()
            {
                (this.playerNameInput.placeholder as TextMeshProUGUI).text = $"Player {this.playerNumber + 1} Name";
                this.isAIToggle.gameObject.SetActive(this.playerNumber > 0);
                this.isAIToggle.isOn = this.playerNumber > 0;
                this.InitializeListeners();
            }

            void InitializeListeners()
            {
                this.playerNameInput.onValueChanged.AddListener(this.OnChangePlayerName);
                this.isAIToggle.onValueChanged.AddListener(this.OnChangeIsAI);
            }

            public PlayerConfig GetPlayerConfig()
            {
                return new PlayerConfig
                {
                    PlayerNumber = this.playerNumber,
                    Name = this.playerNameInput.text,
                    isAI = this.isAIToggle.isOn
                };
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

            private void OnChangeIsAI(bool isAI)
            {
                this.InvokeChangeEvent();
            }

            private void InvokeChangeEvent()
            {
                this.onChange.Invoke(new PlayerConfigOnChangePayload
                {
                    PlayerNumber = this.playerNumber,
                    PlayerName = this.playerNameInput.text,
                    IsAI = this.isAIToggle.isOn,
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

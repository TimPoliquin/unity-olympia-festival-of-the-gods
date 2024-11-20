using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.GameStartEvents;
using Azul.Model;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;

namespace Azul
{
    namespace GameStartEvents
    {
        public class OnGameStartPayload
        {
            public List<PlayerConfig> PlayerConfigs { get; init; }
        }
    }
    namespace Model
    {
        public class GameStartUISetupStep : MonoBehaviour
        {
            [SerializeField] private List<string> defaultPlayerNames = new() { "Helen", "Alexander", "Phoebe", "Metron", "Philomenes", "Eurymachos", "Demonous", "Aetios", "Halie", "Alexis", "Calliope", "Cassandra", "Daphne", "Sophia", "Zoe", "Basil", "Castor", "Georgios", "Jason", "Neo", "Odysseus", "Theodore", "Troy" };
            [SerializeField] private List<Button> playerCountButtons;
            [SerializeField] private GameObject playerConfigContainer;
            [SerializeField] private ButtonUI startButton;
            private readonly List<PlayerConfigUI> playerConfigUIs = new();


            private UnityEvent<OnGameStartPayload> onGameStart = new();

            void Start()
            {
                this.InitializePlayerConfigUIs();
                this.InitializePlayerCountButtons();
                this.startButton.AddOnClickListener(this.OnClickStart);
                this.startButton.gameObject.SetActive(false);
            }

            public void Show()
            {
                this.playerConfigContainer.SetActive(false);
                this.startButton.gameObject.SetActive(false);
            }

            public void Hide()
            {

            }

            private void VerifyIfStartIsReady()
            {
                bool ready = true;
                foreach (PlayerConfigUI input in this.playerConfigUIs)
                {
                    if (input.IsActive())
                    {
                        ready &= input.IsValid();
                    }
                }
                this.startButton.SetInteractable(ready);
            }

            private void OnClickPlayerCount(int playerCount)
            {
                this.ShowPlayerConfigs(playerCount);
            }

            public void ShowPlayerConfigs(int playerCount)
            {
                this.playerConfigContainer.gameObject.SetActive(true);
                for (int idx = 0; idx < this.playerConfigUIs.Count; idx++)
                {
                    this.playerConfigUIs[idx].gameObject.SetActive(idx < playerCount);
                }
                this.startButton.gameObject.SetActive(true);
                this.VerifyIfStartIsReady();
            }

            public Button GetFirstButton()
            {
                return this.playerCountButtons[0];
            }

            public TMP_InputField GetFirstInput()
            {
                return this.playerConfigUIs[0].GetInput();
            }

            private void OnClickStart()
            {
                this.onGameStart.Invoke(new OnGameStartPayload
                {
                    PlayerConfigs = this.playerConfigUIs
                        .FindAll(input => input.gameObject.activeInHierarchy)
                        .Select(input => input.GetPlayerConfig())
                        .ToList()
                });
            }

            public void AddOnGameStartListener(UnityAction<OnGameStartPayload> listener)
            {
                this.onGameStart.AddListener(listener);
            }

            private void InitializePlayerConfigUIs()
            {
                List<string> names = this.CreatePlayerNames(4);
                for (int idx = 0; idx < names.Count; idx++)
                {
                    this.playerConfigUIs.Add(this.CreatePlayerConfigUI(names[idx], idx, idx == 0));
                }
            }

            private void InitializePlayerCountButtons()
            {
                for (int idx = 0; idx < this.playerCountButtons.Count; idx++)
                {
                    int playerCount = idx + 2;
                    this.playerCountButtons[idx].onClick.AddListener(() =>
                    {
                        this.OnClickPlayerCount(playerCount);
                    });
                }
            }

            private PlayerConfigUI CreatePlayerConfigUI(string name, int playerNumber, bool isDefaultPlayer)
            {
                PlayerConfigUI playerConfigUI = System.Instance.GetPrefabFactory().CreatePlayerConfigUI(this.playerConfigContainer.transform);
                playerConfigUI.SetPlayerName(name);
                playerConfigUI.SetPlayerNumber(playerNumber);
                if (isDefaultPlayer)
                {
                    playerConfigUI.SetPlayerType(PlayerType.HUMAN);
                    playerConfigUI.SetPlayerTypeInteractable(false);
                    playerConfigUI.SetPlayerUserName(name);
                }
                else
                {
                    playerConfigUI.SetPlayerType(PlayerType.AI_EASY);
                    playerConfigUI.SetPlayerTypeInteractable(true);
                }
                playerConfigUI.AddOnPlayerConfigChangeListener((payload) =>
                    {
                        this.VerifyIfStartIsReady();
                    });
                return playerConfigUI;
            }

            private List<string> CreatePlayerNames(int playerCount)
            {
                List<string> names = new() { System.Instance.GetUsername() };
                while (names.Count < playerCount)
                {
                    string name = ListUtils.GetRandomElement(this.defaultPlayerNames);
                    if (!names.Contains(name))
                    {
                        names.Add(name);
                    }
                }
                return names;
            }

        }
    }
}

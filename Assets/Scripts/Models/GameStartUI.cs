using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Azul.GameStartEvents;
using Azul.Model;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Azul
{
    namespace GameStartEvents
    {
        public class OnPlayerCountSelectionPayload
        {
            public int PlayerCount { get; init; }
        }
        public class OnGameStartPayload
        {
            public List<PlayerConfig> PlayerConfigs { get; init; }
        }
    }
    namespace Model
    {
        public class GameStartUI : MonoBehaviour
        {
            [SerializeField] private List<Button> playerCountButtons;
            [SerializeField] private GameObject playerUIContainer;
            [SerializeField] private List<PlayerConfigUI> playerConfigUIs;
            [SerializeField] private Button startButton;
            [SerializeField] private List<ColoredValue<IconUI>> icons;

            private UnityEvent<OnPlayerCountSelectionPayload> onPlayerCountSelection = new();
            private UnityEvent<OnGameStartPayload> onGameStart = new();


            // Start is called before the first frame update
            void Awake()
            {
                this.playerConfigUIs.ForEach(input =>
                {
                    input.gameObject.SetActive(false);
                    input.AddOnPlayerConfigChangeListener((payload) =>
                    {
                        this.VerifyIfStartIsReady();
                    });
                });
                for (int idx = 0; idx < this.playerCountButtons.Count; idx++)
                {
                    int playerCount = idx + 2;
                    this.playerCountButtons[idx].onClick.AddListener(() =>
                    {
                        this.OnClickPlayerCount(playerCount);
                    });
                }
                this.startButton.onClick.AddListener(this.OnClickStart);
                this.startButton.gameObject.SetActive(false);
                this.InitializePlayerNames();
            }

            void Start()
            {
                this.icons.ForEach(icon => icon.GetValue().SetTileColor(icon.GetTileColor()));
            }


            private void OnClickPlayerCount(int playerCount)
            {
                this.onPlayerCountSelection.Invoke(new OnPlayerCountSelectionPayload { PlayerCount = playerCount });
            }

            public void ShowPlayerConfigs(int playerCount)
            {
                this.playerUIContainer.gameObject.SetActive(true);
                for (int idx = 0; idx < this.playerConfigUIs.Count; idx++)
                {
                    this.playerConfigUIs[idx].gameObject.SetActive(idx < playerCount);
                }
                this.startButton.gameObject.SetActive(true);
                this.VerifyIfStartIsReady();
            }

            public void AddOnPlayerCountSelectionListener(UnityAction<OnPlayerCountSelectionPayload> listener)
            {
                this.onPlayerCountSelection.AddListener(listener);
            }

            public void AddOnGameStartListener(UnityAction<OnGameStartPayload> listener)
            {
                this.onGameStart.AddListener(listener);
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
                this.startButton.interactable = ready;
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

            public Button GetFirstButton()
            {
                return this.playerCountButtons[0];
            }

            public TMP_InputField GetFirstInput()
            {
                return this.playerConfigUIs[0].GetInput();
            }

            private void InitializePlayerNames()
            {
                this.playerConfigUIs[0].SetPlayerName("Player 1");
                this.playerConfigUIs[1].SetPlayerName("Helen");
                this.playerConfigUIs[2].SetPlayerName("Alexander");
                this.playerConfigUIs[3].SetPlayerName("Phoebe");
            }
        }
    }
}

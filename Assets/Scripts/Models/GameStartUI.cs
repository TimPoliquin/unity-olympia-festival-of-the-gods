using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Azul.GameStartEvents;
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
            public int playerCount { get; init; }
        }
        public class OnGameStartPayload
        {
            public List<string> playerNames { get; init; }
        }
    }
    namespace Model
    {
        public class GameStartUI : MonoBehaviour
        {
            [SerializeField] private List<Button> playerCountButtons;
            [SerializeField] private List<TMP_InputField> playerNameInputs;
            [SerializeField] private Button startButton;

            private UnityEvent<OnPlayerCountSelectionPayload> onPlayerCountSelection = new();
            private UnityEvent<OnGameStartPayload> onGameStart = new();


            // Start is called before the first frame update
            void Awake()
            {
                this.playerNameInputs.ForEach(input =>
                {
                    input.gameObject.SetActive(false);
                    input.onValueChanged.AddListener((playerName) =>
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


            private void OnClickPlayerCount(int playerCount)
            {
                this.onPlayerCountSelection.Invoke(new OnPlayerCountSelectionPayload { playerCount = playerCount });
            }

            public void ShowPlayerNameInputs(int count)
            {
                for (int idx = 0; idx < this.playerNameInputs.Count; idx++)
                {
                    this.playerNameInputs[idx].gameObject.SetActive(idx < count);
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
                foreach (TMP_InputField input in this.playerNameInputs)
                {
                    if (input.gameObject.activeInHierarchy)
                    {
                        ready &= input.text.Length > 0;
                    }
                }
                this.startButton.interactable = ready;
            }

            private void OnClickStart()
            {
                this.onGameStart.Invoke(new OnGameStartPayload
                {
                    playerNames = this.playerNameInputs
                        .FindAll(input => input.gameObject.activeInHierarchy)
                        .Select(input => input.text)
                        .ToList()
                });
            }

            private void InitializePlayerNames()
            {
#if UNITY_EDITOR
                this.playerNameInputs[0].text = "Tim";
                this.playerNameInputs[1].text = "Alex";
                this.playerNameInputs[2].text = "Foobag";
                this.playerNameInputs[3].text = "Oaty";
#endif
            }
        }
    }
}

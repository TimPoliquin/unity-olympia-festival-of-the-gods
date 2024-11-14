using System.Collections;
using System.Collections.Generic;
using Azul.GameStartEvents;
using Azul.Model;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Azul
{
    namespace Controller
    {
        public class GameStartUIController : MonoBehaviour
        {
            [SerializeField] private GameStartUI gameStartUI;

            void Awake()
            {
                this.gameStartUI.GetIntroStep().AddOnPlayListener(this.OnPlay);
                this.gameStartUI.GetSetupStep().AddOnGameStartListener(this.OnGameStart);
                this.gameStartUI.gameObject.SetActive(true);
            }

            void Start()
            {
                this.StartCoroutine(this.MonitorTabInput());
            }

            public void InitializeListeners()
            {
                // nothing to do here... yet
            }

            private void OnPlay()
            {
                this.gameStartUI.ShowSetupStep();
            }

            public void OnGameStart(OnGameStartPayload payload)
            {
                System.Instance.GetPlayerController().SetPlayers(payload.PlayerConfigs);
                // TODO - add a real mechanism to decide who goes first.
                System.Instance.GetPlayerController().SetStartingPlayer(Random.Range(0, payload.PlayerConfigs.Count));
                System.Instance.GetGameController().StartGame();
                this.gameStartUI.gameObject.SetActive(false);
            }

            IEnumerator MonitorTabInput()
            {
                while (this.gameStartUI.gameObject.activeInHierarchy)
                {
                    if (Input.GetKeyDown(KeyCode.Tab))
                    {
                        Selectable current = EventSystem.current.currentSelectedGameObject?.GetComponentInChildren<Selectable>();
                        Selectable next;
                        if (null == current)
                        {
                            next = this.gameStartUI.GetSetupStep().GetFirstButton();
                        }
                        else if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                        {
                            next = current.FindSelectableOnUp();
                        }
                        else
                        {
                            next = current.FindSelectableOnDown();
                        }
                        if (null != next)
                        {
                            next.Select();
                            TMP_InputField input;
                            next.TryGetComponent(out input);
                            if (null != input)
                            {
                                input.OnPointerClick(new PointerEventData(EventSystem.current));
                            }
                        }
                    }

                    yield return null;
                }
            }
        }
    }
}

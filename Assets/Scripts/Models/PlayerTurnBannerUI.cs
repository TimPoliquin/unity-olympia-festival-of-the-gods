using System.Collections;
using System.Collections.Generic;
using Azul.Animation;
using Azul.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Azul
{
    namespace Model
    {
        [RequireComponent(typeof(Fade))]
        public class PlayerTurnBannerUI : MonoBehaviour
        {
            [SerializeField] private string titleTemplate = "{0}'s Turn!";
            [SerializeField] private TextMeshProUGUI titleText;
            [SerializeField] private IconUI hadesIcon;
            [SerializeField] private GameObject acquiringInstructions;
            [SerializeField] private GameObject scoringInstructions;
            [SerializeField] private Button closeButton;

            private UnityEvent onClose = new();

            private bool hidden = false;

            void Start()
            {
                this.hadesIcon.SetTileColor(TileColor.ONE);
                this.closeButton.onClick.AddListener(() =>
                {
                    if (!this.hidden)
                    {
                        this.onClose.Invoke();
                    }
                });
            }

            public CoroutineResult Show(string playerName, Phase phase, bool showInstructions = true)
            {
                bool show;
                this.titleText.text = string.Format(titleTemplate, playerName);
                switch (phase)
                {
                    case Phase.ACQUIRE:
                        {
                            this.acquiringInstructions.SetActive(showInstructions);
                            this.scoringInstructions.SetActive(false);
                            show = true;
                            break;
                        }
                    case Phase.SCORE:
                        {
                            this.scoringInstructions.SetActive(showInstructions);
                            this.acquiringInstructions.SetActive(false);
                            show = true;
                            break;
                        }
                    default:
                        {
                            show = false;
                            break;
                        }
                }
                if (show)
                {
                    if (!showInstructions)
                    {
                        this.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 120);
                        this.closeButton.gameObject.SetActive(false);
                    }
                    Fade transition = this.GetComponent<Fade>();
                    transition.StartHidden();
                    return transition.Show();
                }
                else
                {
                    return null;
                }
            }

            public CoroutineResult Hide()
            {
                if (!this.hidden)
                {
                    this.hidden = true;
                    return this.GetComponent<Fade>().Hide();
                }
                return null;
            }

            public void AddOnCloseListener(UnityAction listener)
            {
                this.onClose.AddListener(listener);
            }

            public bool IsHidden()
            {
                return this.hidden;
            }
        }
    }
}

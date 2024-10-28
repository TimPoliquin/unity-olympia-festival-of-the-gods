using System.Collections;
using System.Collections.Generic;
using Azul.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Azul
{
    namespace Controller
    {
        public class HelpPanelUIController : MonoBehaviour
        {
            [SerializeField] private Button helpButton;

            private HelpPanelUI helpPanelUI;

            void Start()
            {
                this.helpButton.onClick.AddListener(this.OnHelp);
            }

            private void OnHelp()
            {
                if (null == this.helpPanelUI)
                {
                    this.helpPanelUI = System.Instance.GetPrefabFactory().CreateHelpPanelUI();
                    this.helpPanelUI.AddOnCloseClickListener(() => this.OnClose(helpPanelUI));
                }
                this.StartCoroutine(this.OnHelpCoroutine());
            }

            private IEnumerator OnHelpCoroutine()
            {
                yield return this.helpPanelUI.Toggle().WaitUntilCompleted();
            }

            private void OnClose(HelpPanelUI helpPanelUI)
            {
                this.StartCoroutine(this.OnCloseCoroutine(helpPanelUI));
            }

            private IEnumerator OnCloseCoroutine(HelpPanelUI helpPanelUI)
            {
                yield return helpPanelUI.Hide().WaitUntilCompleted();
            }
        }
    }
}

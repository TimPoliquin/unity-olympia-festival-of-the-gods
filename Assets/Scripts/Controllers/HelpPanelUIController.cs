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

            void Start()
            {
                this.helpButton.onClick.AddListener(this.OnHelp);
            }

            private void OnHelp()
            {
                this.StartCoroutine(this.OnHelpCoroutine());
            }

            private IEnumerator OnHelpCoroutine()
            {
                this.helpButton.interactable = false;
                HelpPanelUI helpPanelUI = System.Instance.GetPrefabFactory().CreateHelpPanelUI();
                helpPanelUI.AddOnCloseClickListener(() => this.OnClose(helpPanelUI));
                yield return helpPanelUI.Show().WaitUntilCompleted();
            }

            private void OnClose(HelpPanelUI helpPanelUI)
            {
                this.StartCoroutine(this.OnCloseCoroutine(helpPanelUI));
            }

            private IEnumerator OnCloseCoroutine(HelpPanelUI helpPanelUI)
            {
                yield return helpPanelUI.Hide().WaitUntilCompleted();
                Destroy(helpPanelUI.gameObject);
                this.helpButton.interactable = true;
            }
        }
    }
}

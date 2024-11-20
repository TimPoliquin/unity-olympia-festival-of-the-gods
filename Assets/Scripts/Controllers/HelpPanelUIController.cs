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
            public static readonly string HELP_LAYER = "help";
            [SerializeField] private Button helpButton;

            private HelpPanelUI helpPanelUI;

            void Start()
            {
                this.helpButton.onClick.AddListener(this.OnHelp);
            }

            public void ShowHelp()
            {
                this.OnHelp();
            }

            private void OnHelp()
            {
                if (null == this.helpPanelUI)
                {
                    this.helpPanelUI = System.Instance.GetPrefabFactory().CreateHelpPanelUI(HELP_LAYER);
                    this.helpPanelUI.AddOnCloseClickListener(() => this.OnClose(helpPanelUI));
                }
                this.StartCoroutine(this.OnHelpCoroutine());
            }

            private IEnumerator OnHelpCoroutine()
            {
                System.Instance.GetUIController().GetPanelManagerController().ShowLayer(HELP_LAYER);
                yield return this.helpPanelUI.Show().WaitUntilCompleted();
            }

            private void OnClose(HelpPanelUI helpPanelUI)
            {
                this.StartCoroutine(this.OnCloseCoroutine(helpPanelUI));
            }

            private IEnumerator OnCloseCoroutine(HelpPanelUI helpPanelUI)
            {
                yield return helpPanelUI.Hide().WaitUntilCompleted();
                Destroy(this.helpPanelUI.gameObject);
                System.Instance.GetUIController().GetPanelManagerController().ShowDefaultLayer();
            }
        }
    }
}

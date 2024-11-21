using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Animation;
using Azul.Util;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        [Serializable]
        public struct UnavailableTokensText
        {
            public string TitleTemplate;
            public string SubTitleTemplate;
        }
        [RequireComponent(typeof(Fade))]
        public class UnavailableTokensPanelUI : MonoBehaviour
        {
            [SerializeField] private UnavailableTokensText hadesTokenText;
            [SerializeField] private UnavailableTokensText wildTokenText;
            [SerializeField] private TextMeshProUGUI titleText;
            [SerializeField] private TextMeshProUGUI subtitleText;

            private Fade fade;

            private void Awake()
            {
                this.fade = this.GetComponent<Fade>();
                this.fade.StartHidden();
            }

            public CoroutineStatus ShowHadesUnavailable()
            {
                this.titleText.text = this.hadesTokenText.TitleTemplate;
                this.subtitleText.text = this.hadesTokenText.SubTitleTemplate;
                return this.fade.Show();
            }

            public CoroutineStatus ShowWildUnavailable(string godName)
            {
                this.titleText.text = string.Format(this.wildTokenText.TitleTemplate, godName);
                this.subtitleText.text = this.wildTokenText.SubTitleTemplate;
                return this.fade.Show();
            }

            public CoroutineStatus Hide()
            {
                return this.fade.Hide();
            }
        }
    }
}

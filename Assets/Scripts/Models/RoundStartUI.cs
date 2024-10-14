using System;
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
        public class RoundStartUI : MonoBehaviour
        {
            [SerializeField] private IconUI icon;
            [SerializeField] private string titleTemplate = "The Season of {0} Has Begun!";
            [SerializeField] private string subtitleTemplate = "{0}'s tokens can be used to complete all rituals.";
            [SerializeField] private TextMeshProUGUI titleText;
            [SerializeField] private TextMeshProUGUI subtitleText;
            [SerializeField] private Button dismissButton;
            private UnityEvent onDismiss = new();

            private bool isDismissed = false;

            private Fade transition;

            void Awake()
            {
                this.dismissButton.onClick.AddListener(this.OnDismiss);
            }

            public CoroutineResult Show(string god, TileColor tileColor)
            {
                this.transition = this.GetComponent<Fade>();
                this.transition.StartHidden();
                this.icon.SetTileColor(tileColor);
                this.titleText.text = string.Format(this.titleTemplate, god);
                this.subtitleText.text = string.Format(this.subtitleTemplate, god);
                return this.transition.Show();
            }

            public CoroutineResult Hide()
            {
                if (!this.isDismissed)
                {
                    this.isDismissed = true;
                    return this.transition.Hide();
                }
                else
                {
                    CoroutineResult result = CoroutineResult.Single();
                    result.Finish();
                    return result;
                }
            }

            private void OnDismiss()
            {
                this.onDismiss.Invoke();
            }

            public bool IsDismissed()
            {
                return this.isDismissed;
            }

            public void AddOnDismissListener(UnityAction listener)
            {
                this.onDismiss.AddListener(listener);
            }
        }
    }
}

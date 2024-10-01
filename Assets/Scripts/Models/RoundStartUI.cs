using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Animation;
using TMPro;
using UnityEngine;

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

            private Fade transition;

            public void Show(string god, TileColor tileColor)
            {
                this.transition = this.GetComponent<Fade>();
                this.transition.StartHidden();
                this.icon.SetTileColor(tileColor);
                this.titleText.text = string.Format(this.titleTemplate, god);
                this.subtitleText.text = string.Format(this.subtitleTemplate, god);
                this.transition.Show();
            }

            public void Hide(Action callback)
            {
                this.transition.Hide(() =>
                {
                    callback.Invoke();
                    Destroy(this.gameObject);
                });
            }
        }
    }
}

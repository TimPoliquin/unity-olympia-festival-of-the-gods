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
        [RequireComponent(typeof(MoveAndScale))]
        public class HadesTokenPanelUI : MonoBehaviour
        {
            [SerializeField] private IconUI hadesIcon;
            [SerializeField] private GameObject scoreContainer;
            [SerializeField] private TextMeshProUGUI scoreText;
            [SerializeField] private Button dismissButton;
            private Fade fade;
            private MoveAndScale moveAndScale;
            private UnityEvent onDismiss = new();

            void Awake()
            {
                this.fade = this.GetComponent<Fade>();
                this.moveAndScale = this.GetComponent<MoveAndScale>();
                this.dismissButton.gameObject.SetActive(false);
                this.dismissButton.onClick.AddListener(this.OnDismiss);
            }

            public CoroutineResult Show(int tileCount)
            {
                this.hadesIcon.SetTileColor(TileColor.ONE);
                this.scoreText.text = $"-{tileCount}";
                this.fade.StartHidden();
                return this.fade.Show();
            }

            public CoroutineResult AnimateScoreToPoint(Vector3 target, float scale, float time)
            {
                return this.moveAndScale.Animate(this.scoreContainer, target, scale, time);
            }

            public CoroutineResult Hide()
            {
                return this.fade.Hide();
            }

            private void OnDismiss()
            {
                this.onDismiss.Invoke();
            }

            public void AddOnDismissHandler(UnityAction listener)
            {
                this.dismissButton.gameObject.SetActive(true);
                this.onDismiss.AddListener(listener);
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using Azul.Controller;
using Azul.Model;
using Azul.Animation;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class StarSpaceUI : MonoBehaviour
        {
            private AltarSpace starSpace;
            [SerializeField] private TextMeshProUGUI valueText;
            [SerializeField] private float tableScale = .25f;
            [SerializeField] private float boardScale = 1.0f;
            [SerializeField] private AttentionUI attentionUI;

            void Awake()
            {
                this.DeactivateHighlight();
            }

            void LateUpdate()
            {
                if (null != this.starSpace)
                {
                    this.UpdatePosition();
                    if (!starSpace.IsEmpty() && this.valueText.color.a < 1)
                    {
                        this.valueText.color = new Color(this.valueText.color.r, this.valueText.color.g, this.valueText.color.b, 1f);
                    }
                }
            }

            public void SetStarSpace(AltarSpace starSpace)
            {
                this.starSpace = starSpace;
                this.valueText.text = $"{starSpace.GetValue()}";
                this.UpdatePosition();
            }

            private void UpdatePosition()
            {
                if (System.Instance.GetCameraController().IsInView(this.starSpace.gameObject))
                {
                    this.valueText.gameObject.SetActive(true);
                    this.transform.position = Camera.main.WorldToScreenPoint(this.starSpace.transform.position);
                }
                else
                {
                    this.valueText.gameObject.SetActive(false);
                    this.DeactivateHighlight();
                }
            }

            public void ScaleToTableView()
            {
                this.transform.localScale = Vector3.one * this.tableScale;
            }

            public void ScaleToPlayerBoardView()
            {
                this.transform.localScale = Vector3.one * this.boardScale;
            }

            public void ActivateHighlight()
            {
                this.attentionUI.gameObject.SetActive(true);
            }
            public void DeactivateHighlight()
            {
                this.attentionUI.gameObject.SetActive(false);
            }
        }
    }
}

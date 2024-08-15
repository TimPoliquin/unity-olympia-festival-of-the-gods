using System.Collections;
using System.Collections.Generic;
using Azul.Controller;
using Azul.Model;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class StarSpaceUI : MonoBehaviour
        {
            private StarSpace starSpace;
            [SerializeField] private TextMeshProUGUI valueText;
            [SerializeField] private float tableScale = .25f;
            [SerializeField] private float boardScale = 1.0f;

            void LateUpdate()
            {
                if (null != this.starSpace)
                {
                    this.UpdatePosition();
                }
            }

            public void SetStarSpace(StarSpace starSpace)
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
        }
    }
}
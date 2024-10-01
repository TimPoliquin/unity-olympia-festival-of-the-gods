using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class ScoreTilesPreviewPanelUI : MonoBehaviour
        {
            [SerializeField] private CostFieldUI costField;
            [SerializeField] private PointsFieldUI pointsField;
            [SerializeField] private ColorMilestoneProgressFieldUI colorMilestoneProgressField;
            [SerializeField] private ValueMilestoneProgressFieldUI valueMilestoneProgressField;
            [SerializeField] private GameObject rewardContainer;
            [SerializeField] private float transitionTime = 0.25f;
            [SerializeField] private float delay = 0.5f;
            [SerializeField] private Vector3 offset = Vector3.right * 40f;

            private bool hidden = false;
            private CanvasGroup canvasGroup;

            void Awake()
            {
                this.canvasGroup = this.GetComponent<CanvasGroup>();
                this.hidden = true;
                this.canvasGroup.alpha = 0;
                this.rewardContainer.SetActive(false);
                this.valueMilestoneProgressField.gameObject.SetActive(false);
            }

            public void SetSpaceValue(TileColor color, int value)
            {
                this.costField.SetTileColor(color);
                this.costField.SetValue(value);
            }

            public void SetColorProgress(TileColor color, int progress, int total)
            {
                this.colorMilestoneProgressField.SetTileColor(color);
                this.colorMilestoneProgressField.SetProgress(progress, total);
            }

            public void SetValueProgress(int value, int progress, int total)
            {
                this.valueMilestoneProgressField.gameObject.SetActive(true);
                this.valueMilestoneProgressField.SetValue(value);
                this.valueMilestoneProgressField.SetProgress(progress, total);
            }

            public void SetScoreValue(int scoreValue)
            {
                this.pointsField.SetScoreValue(scoreValue);
            }

            public void AddRewardProgress(RewardProgressFieldUI rewardProgressFieldUI)
            {
                this.rewardContainer.SetActive(true);
                rewardProgressFieldUI.transform.SetParent(this.rewardContainer.transform);
            }

            public void Show(Vector3 targetPosition)
            {
                bool startedHidden = this.canvasGroup.alpha == 0;
                this.gameObject.SetActive(true);
                this.hidden = false;
                this.transform.position = System.Instance.GetCameraController().GetMainCamera().WorldToScreenPoint(targetPosition) + this.offset;
                this.StartCoroutine(this.Transition(startedHidden ? this.delay : 0, this.transitionTime, 1, null));
            }

            public void Hide(Action<ScoreTilesPreviewPanelUI> callback)
            {
                this.hidden = true;
                this.StartCoroutine(this.Transition(0, this.transitionTime, 0, () =>
                {
                    this.gameObject.SetActive(false);
                    if (callback != null)
                    {
                        callback.Invoke(this);
                    }
                }));
            }

            private IEnumerator Transition(float delay, float time, float alpha, Action callback)
            {
                bool hidden = this.hidden;
                float alphaDirection = alpha < this.canvasGroup.alpha ? -1 : 1;
                yield return new WaitForSeconds(delay);
                while (hidden == this.hidden && this.canvasGroup.alpha != alpha)
                {
                    this.canvasGroup.alpha = Mathf.Clamp(this.canvasGroup.alpha + Time.deltaTime / time * alphaDirection, 0, 1);
                    yield return null;
                }
                if (hidden == this.hidden && callback != null)
                {
                    callback.Invoke();
                }
            }
        }
    }
}

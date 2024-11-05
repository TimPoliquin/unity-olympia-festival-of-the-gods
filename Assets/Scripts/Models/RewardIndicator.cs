using System;
using System.Collections;
using System.Collections.Generic;
using Azul.Controller;
using Azul.Model;
using Azul.PointerEvents;
using UnityEngine;
using UnityEngine.UIElements;

namespace Azul
{
    namespace Model
    {
        public class RewardIndicator : MonoBehaviour
        {
            [SerializeField] private GameObject lid;
            [SerializeField] private GameObject treasure;
            [SerializeField] private Light pointLight;
            [SerializeField] private float force = 12.0f;
            [SerializeField] private float openTime = 2.0f;


            private RewardBehavior rewardBehavior;
            private RewardIndicatorPointerEventController pointerEventController;
            private bool isOpened = false;

            void Awake()
            {
                this.pointerEventController = this.gameObject.AddComponent<RewardIndicatorPointerEventController>();
            }

            public RewardIndicatorPointerEventController GetPointerEventController()
            {
                return this.pointerEventController;
            }

            public void SetRewardBehavior(RewardBehavior rewardBehavior)
            {
                this.rewardBehavior = rewardBehavior;
                this.rewardBehavior.AddOnCompleteListener(this.OnRewardEarn);
            }

            public RewardBehavior GetRewardBehavior()
            {
                return this.rewardBehavior;
            }

            public void OnRewardEarn()
            {
                if (!this.isOpened)
                {
                    this.isOpened = true;
                    this.StartCoroutine(this.OpenCoroutine());
                }
            }

            public void OnRewardClaim()
            {
                this.pointLight.enabled = false;
                this.treasure.SetActive(false);
            }

            private IEnumerator OpenCoroutine()
            {
                float openAmount = 0;
                while (openAmount < 1)
                {
                    openAmount += Time.deltaTime / this.openTime;
                    this.lid.GetComponent<Rigidbody>().AddForce(this.lid.transform.TransformDirection(Vector3.right * Time.deltaTime * this.force), ForceMode.Impulse);
                    yield return null;
                }

            }
        }
    }
    namespace Controller
    {
        public class RewardIndicatorPointerEventController : PointerEventController<RewardIndicator>
        {
            public RewardIndicatorPointerEventController() : base()
            {
                this.SetInteractable(true);
            }
        }
    }
}

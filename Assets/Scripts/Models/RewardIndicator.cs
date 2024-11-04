using System.Collections;
using System.Collections.Generic;
using Azul.Controller;
using Azul.Model;
using Azul.PointerEvents;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class RewardIndicator : MonoBehaviour
        {
            [SerializeField] private GameObject lid;
            [SerializeField] private GameObject treasure;
            [SerializeField] private Light pointLight;

            private RewardBehavior rewardBehavior;
            private RewardIndicatorPointerEventController pointerEventController;

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
                // TODO
            }

            public void OnRewardClaim()
            {
                this.pointLight.enabled = false;
                this.treasure.SetActive(false);
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Layout;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    public class PlayerBoard : MonoBehaviour
    {
        [SerializeField] private GameObject outerRing;
        [SerializeField] private GameObject center;
        [SerializeField] private Light activePlayerLight;
        private CircularLayout layout;

        void Awake()
        {
            this.layout = this.outerRing.GetComponent<CircularLayout>();
        }


        public void AddStars(List<Star> stars)
        {
            this.layout.AddChildren(stars.Select(star => star.gameObject).ToList());
        }

        public void AddCenterStar(Star star)
        {
            star.transform.SetParent(this.center.transform);
            star.transform.localPosition = Vector3.zero;
        }

        public void ActivateLight()
        {
            this.activePlayerLight.gameObject.SetActive(true);
        }

        public void DeactivateLight()
        {
            this.activePlayerLight.gameObject.SetActive(false);
        }
    }
}

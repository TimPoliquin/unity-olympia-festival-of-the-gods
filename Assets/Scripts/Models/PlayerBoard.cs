using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Layout;
using Azul.Model;
using Azul.Utils;
using UnityEngine;

namespace Azul
{
    public class PlayerBoard : MonoBehaviour
    {
        [SerializeField] private GameObject outerRing;
        [SerializeField] private GameObject center;
        [SerializeField] private GameObject drawnTilesContainer;
        [SerializeField] private Light activePlayerLight;
        private CircularLayout layout;
        private List<Tile> drawnTiles = new();

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

        public void AddDrawnTiles(List<Tile> tiles)
        {
            this.drawnTiles.AddRange(tiles);
            foreach (Tile tile in tiles)
            {
                tile.transform.SetParent(this.drawnTilesContainer.transform);
                tile.transform.localPosition = VectorUtils.CreateRandomVector3(-5, 5);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Layout;
using Azul.Model;
using Azul.PlayerBoardEvents;
using Azul.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Azul
{
    namespace PlayerBoardEvents
    {
        public class OnPlayerAcquireOneTilePayload
        {
            public int PlayerNumber { get; init; }
            public List<Tile> AcquiredTiles { get; init; }
        }
    }
    namespace Model
    {
        public class PlayerBoard : MonoBehaviour
        {
            [SerializeField] private GameObject outerRing;
            [SerializeField] private GameObject center;
            [SerializeField] private DrawnTilesContainer drawnTilesContainer;
            [SerializeField] private Light activePlayerLight;
            private int playerNumber;
            private CircularLayout layout;
            private List<Tile> drawnTiles = new();
            private bool hasOneTile;

            private UnityEvent<OnPlayerAcquireOneTilePayload> onAcquireOneTile = new();

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
                this.drawnTilesContainer.AddTiles(tiles);
                Tile oneTile = tiles.Find(tile => tile.IsOneTile());
                if (null != oneTile)
                {
                    this.hasOneTile = true;
                    this.onAcquireOneTile.Invoke(new OnPlayerAcquireOneTilePayload { PlayerNumber = this.playerNumber, AcquiredTiles = tiles });

                }
            }

            public int GetPlayerNumber()
            {
                return this.playerNumber;
            }

            public void SetPlayerNumber(int playerNumber)
            {
                this.playerNumber = playerNumber;
            }

            public bool HasOneTile()
            {
                return this.hasOneTile;
            }

            public void AddOnAcquireOneTileListener(UnityAction<OnPlayerAcquireOneTilePayload> listener)
            {
                this.onAcquireOneTile.AddListener(listener);
            }

        }
    }
}

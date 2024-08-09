using System.Collections;
using System.Collections.Generic;
using Azul.Controller;
using Azul.Model;
using UnityEngine;

namespace Azul
{
    public class TilePlaceholder : MonoBehaviour
    {
        [SerializeField] private GameObject placeholder;
        [SerializeField] private TileColor color;

        private Tile real;

        public void SetPlaceholder(GameObject placeholder)
        {
            this.placeholder = placeholder;
        }

        public void PlaceTile(Tile real)
        {
            this.real = real;
            this.real.transform.SetParent(this.transform);
            this.real.transform.localPosition = Vector3.zero;
            this.real.transform.localRotation = Quaternion.Euler(Vector3.zero);
            this.real.gameObject.SetActive(true);
            this.placeholder.SetActive(false);
        }

        public Tile DrawTile()
        {
            Tile tile = this.real;
            this.real = null;
            tile.transform.SetParent(null);
            tile.transform.localPosition = Vector3.zero;
            tile.transform.localRotation = Quaternion.identity;
            this.placeholder.SetActive(true);
            return tile;
        }

        public bool IsEmpty()
        {
            return this.real == null;
        }

        public TileColor GetColor()
        {
            return this.color;
        }

        public TileColor GetEffectiveColor()
        {
            if (null != this.real)
            {
                return this.real.Color;
            }
            else
            {
                return this.color;
            }
        }

        public TilePlaceholderPointerEventController GetPointerEventController()
        {
            return this.GetComponent<TilePlaceholderPointerEventController>();
        }

        public static TilePlaceholder Create(GameObject placeholderPrefab, TileColor color)
        {
            GameObject gameObject = Instantiate(placeholderPrefab);
            TilePlaceholder tilePlaceholder = gameObject.GetComponent<TilePlaceholder>();
            tilePlaceholder.color = color;
            return tilePlaceholder;
        }
    }
}

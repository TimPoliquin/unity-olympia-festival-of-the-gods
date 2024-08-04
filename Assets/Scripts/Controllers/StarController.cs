using System.Collections;
using System.Collections.Generic;
using Azul.Layout;
using Azul.Model;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class StarController : MonoBehaviour
        {
            [SerializeField] private int numSpaces = 6;
            [SerializeField] private GameObject starPrefab;
            [SerializeField] private GameObject tilePrefab;
            public Star CreateStar(TileColor color)
            {
                StarSpace[] spaces = new StarSpace[6];
                Star star = Instantiate(this.starPrefab).GetComponent<Star>();
                List<TilePlaceholder> placeholders = new();
                for (int idx = 0; idx < this.numSpaces; idx++)
                {
                    TilePlaceholder tile = TilePlaceholder.Create(this.tilePrefab, color);
                    tile.gameObject.name = $"Tile {idx + 1}";
                    placeholders.Add(tile);
                    StarSpace space = tile.AddComponent<StarSpace>();
                    space.SetValue(idx + 1);
                    spaces[idx] = space;
                }
                star.AddTilePlaceholders(placeholders);
                star.SetColor(color);
                star.SetSpaces(spaces);
                return star;
            }
        }
    }
}

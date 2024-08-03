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
            [SerializeField] private float starRadius = 3.0f;
            [SerializeField] private GameObject tilePrefab;
            public Star CreateStar(TileColor color)
            {
                StarSpace[] spaces = new StarSpace[6];
                GameObject gameObject = new GameObject($"Star: {color}");
                Star star = gameObject.AddComponent<Star>();
                CircularLayout layout = gameObject.AddComponent<CircularLayout>();
                layout.CreateLayout(6, this.starRadius, (input) =>
                {
                    TilePlaceholder tile = TilePlaceholder.Create(this.tilePrefab, color);
                    tile.gameObject.name = $"Tile {input.Index + 1}";
                    StarSpace space = tile.AddComponent<StarSpace>();
                    space.SetValue(input.Index + 1);
                    spaces[input.Index] = space;
                    return tile.gameObject;
                });
                star.SetColor(color);
                star.SetSpaces(spaces);
                return star;
            }
        }
    }
}

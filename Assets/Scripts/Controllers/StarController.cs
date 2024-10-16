using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azul.Layout;
using Azul.Model;
using Azul.Prefab;
using Unity.VisualScripting;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class AltarFactory : MonoBehaviour
        {
            [SerializeField] private Altar altarPrefab;
            [SerializeField] private List<AltarPrefab> altarPrefabs;
            [SerializeField] private int numSpaces = 6;
            [SerializeField] private AltarSpace altarSpacePrefab;
            public Altar CreateAltar(TileColor color, float rotation)
            {
                AltarSpace[] spaces = new AltarSpace[6];
                Altar star = Instantiate(this.altarPrefab).GetComponent<Altar>();
                star.SetAltarModel(Instantiate(this.altarPrefabs.Find(altarPrefab => altarPrefab.GetTileColor() == color).GetPrefab()));
                for (int idx = 0; idx < this.numSpaces; idx++)
                {
                    AltarSpace space = Instantiate(this.altarSpacePrefab);
                    space.SetValue(idx + 1);
                    space.SetOriginalColor(color);
                    spaces[idx] = space;
                }
                star.SetColor(color);
                star.AddAltarSpaces(spaces.ToList(), rotation);
                return star;
            }
        }
    }
}

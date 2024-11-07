using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using Utils;

namespace Azul
{
    namespace Model
    {
        public class Fire : MonoBehaviour
        {
            [SerializeField] private VisualEffect fire;
            [SerializeField] private FlickeringSpotLight fireLight;
            [SerializeField] AudioSource fireSFX;

            public void Enable(bool playSound = true)
            {
                this.fire.gameObject.SetActive(true);
                this.fireLight.gameObject.SetActive(true);
                if (playSound)
                {
                    this.fireSFX.Play();
                }
            }

            public void Disable()
            {
                this.fire.gameObject.SetActive(false);
                this.fireLight.gameObject.SetActive(false);
            }

            public void SetColor(TileColor tileColor)
            {
                Color color = System.Instance.GetTileMaterialProvider().GetColor(tileColor);
                this.fire.SetVector4("Color", color * 5);
                this.fireLight.SetColor(tileColor);
            }
        }
    }
}

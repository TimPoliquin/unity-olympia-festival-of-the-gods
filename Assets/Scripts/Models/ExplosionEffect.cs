using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Azul
{
    namespace Model
    {
        public class ExplosionEffect : MonoBehaviour
        {
            [SerializeField] private ParticleSystem explosion;

            public void Play(TileColor tileColor)
            {
                ParticleSystem.MainModule main = this.explosion.main;
                main.startColor = System.Instance.GetTileMaterialProvider().GetColor(tileColor);
                main.stopAction = ParticleSystemStopAction.Callback;
                this.explosion.Clear();
                this.explosion.Play();
            }

            void OnParticleSystemStopped()
            {
                Destroy(this.gameObject);
            }
        }
    }
}

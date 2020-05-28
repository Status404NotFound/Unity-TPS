using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR
{
    public class WeaponHook : MonoBehaviour
    {
        public Transform leftHandIK;

        ParticleSystem[] particles;


        private void OnEnable()
        {
            particles = transform.GetComponentsInChildren<ParticleSystem>();
        }

        public void Shoot()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Play();
            }
        }
    }
}
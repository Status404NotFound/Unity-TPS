using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR
{
    [CreateAssetMenu(menuName = "Weapons/Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        public string id;

        public IKPositions m_h_IK;
        public GameObject modelPrefab;

        public float fireRate;
        public int magazineAmmo;
        public int maxAmmo;
        public bool onIdleDisableOh;
        public int weaponType;

        public AnimationCurve recoilY;
        public AnimationCurve recoilZ;
    }
}
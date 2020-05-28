using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR
{
    [System.Serializable]
    public class WeaponManager
    {
        public string mw_id;
        public string sw_id;

        public RuntimeWeapon m_weapon;
        public RuntimeWeapon s_weapon;
    }
}
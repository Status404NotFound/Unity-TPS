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

        RuntimeWeapon curWeapon;
        public RuntimeWeapon GetCurrent()
        {
            return curWeapon;
        }

        public void SetCurrent(RuntimeWeapon rw)
        {
            curWeapon = rw;
        }

        public RuntimeWeapon m_weapon;
        public RuntimeWeapon s_weapon;
    }
}
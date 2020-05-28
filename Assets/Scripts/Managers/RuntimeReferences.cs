using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR
{
    [CreateAssetMenu(menuName = "Single Instances/Runtime References")]
    public class RuntimeReferences : ScriptableObject
    {
        public List<RuntimeWeapon> runtime_weapons = new List<RuntimeWeapon>();

        public void Init()
        {
            runtime_weapons.Clear();
        }

        public RuntimeWeapon WeaponToRuntimeWeapon(Weapon w)
        {
            RuntimeWeapon rw = new RuntimeWeapon();
            rw.w_actual = w;
            rw.curAmmo = w.magazineAmmo;
            rw.curCarrying = w.maxAmmo;
            runtime_weapons.Add(rw);
            return rw;
        }

        public void RemoveRuntimeWeapon(RuntimeWeapon rw)
        {
            if (rw.m_instance)
                Destroy(rw.m_instance);

            if (runtime_weapons.Contains(rw))
                runtime_weapons.Remove(rw);
        }
    }

    public class RuntimeWeapon
    {
        public int curAmmo;
        public int curCarrying;
        public float lastFired;
        public GameObject m_instance;
        public WeaponHook w_hook;
        public Weapon w_actual;

        public void ShootWeapon()
        {
            w_hook.Shoot();
            curAmmo--;
        }
    }
}
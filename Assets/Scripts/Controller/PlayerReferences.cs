using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR
{
    [CreateAssetMenu(menuName = "Single Instances/Player Refs")]
    public class PlayerReferences : ScriptableObject
    {
        public IntVariable curAmmo;
        public IntVariable curCarrying;
        public IntVariable health;
        public FloatVariable targetSpread;
        public GameEvent e_UpdateUI;
    }
}
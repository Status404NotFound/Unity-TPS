using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR
{
    [CreateAssetMenu(menuName = "Controller/Stats")]
    public class ControllerStats : ScriptableObject
    {
        public float walkSpeed = 4;
        public float runSpeed = 6;
        public float crounchSpeed = 2;
        public float aimSpeed = 2;
        public float rotateSpeed = 8;
    }
}
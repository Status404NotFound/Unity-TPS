using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR
{
    [CreateAssetMenu(menuName = "Controller/Camera Values")]
    public class CameraValues : ScriptableObject
    {
        public float turnSmooth;
        public float moveSpeed;
        public float aimSpeed;
        public float y_rotete_speed;
        public float x_rotete_speed;
        public float minAngle;
        public float maxAngle;
        public float normalZ;
        public float normalX;
        public float aimZ;
        public float aimX;
        public float normalY;
        public float crouchY;
        public float adaptSpeed;
    }
}
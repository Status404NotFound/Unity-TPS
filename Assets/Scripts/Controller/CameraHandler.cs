using FR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform camTrans;
        public Transform target;
        public Transform pivot;
        public Transform mTransform;
        public bool leftPivot;
        private float delta;
        private float mouseX;
        private float mouseY;
        private float smoothX;
        private float smoothY;
        private float smoothXvelocity;
        private float smoothYvelocity;
        private float lookAngle;
        private float tiltAngle;

        public CameraValues values;
        private StatesManager states;

        public void Init(InputHandler inp)
        {
            mTransform = this.transform;
            states = inp.states;
            target = states.mTransform;
        }

        public void FixedTick(float d)
        {
            delta = d;

            if (target == null)
                return;

            HandlePositions();
            HandleRotation();

            float speed = values.moveSpeed;
            if (states.states.isAiming)
                speed = values.aimSpeed;

            Vector3 targetPosition = Vector3.Lerp(mTransform.position, target.position, delta * speed);
            mTransform.position = targetPosition;
        }

        private void HandlePositions()
        {
            float targetX = values.normalX;
            float targetZ = values.normalZ;
            float targetY = values.normalY;

            if (states.states.isCrouching)
                targetY = values.crouchY;

            if (states.states.isAiming)
            {
                targetX = values.aimX;
                targetZ = values.aimZ;
            }

            if (leftPivot)
                targetX = -targetX;

            Vector3 newPivotPosition = pivot.localPosition;
            newPivotPosition.x = targetX;
            newPivotPosition.y = targetY;

            Vector3 newCamPosition = camTrans.localPosition;
            newCamPosition.z = targetZ;

            float t = delta * values.adaptSpeed;
            pivot.localPosition = Vector3.Lerp(pivot.localPosition, newPivotPosition, t);
            camTrans.localPosition = Vector3.Lerp(camTrans.localPosition, newCamPosition, t);
        }

        private void HandleRotation()
        {
            mouseX = Input.GetAxis(StaticStrings.MouseX);
            mouseY = Input.GetAxis(StaticStrings.MouseY);

            if (values.turnSmooth > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, mouseX, ref smoothXvelocity, values.turnSmooth);
                smoothY = Mathf.SmoothDamp(smoothY, mouseY, ref smoothYvelocity, values.turnSmooth);
            }
            else
            {
                smoothX = mouseX;
                smoothY = mouseY;
            }

            lookAngle += smoothX * values.y_rotete_speed;
            Quaternion targetRot = Quaternion.Euler(0, lookAngle, 0);
            mTransform.rotation = targetRot;

            tiltAngle -= smoothY * values.x_rotete_speed;
            tiltAngle = Mathf.Clamp(tiltAngle, values.minAngle, values.maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);
        }
    }
}
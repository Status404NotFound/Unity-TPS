﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR
{
    public class AnimatorHook : MonoBehaviour
    {
        private Animator anim;
        private StatesManager states;
        private float m_h_weight;
        private float o_h_weight;
        private float l_weight;
        private float b_weight;

        public Transform rh_target;
        public Transform lh_target;
        private Transform shoulder;
        private Transform aimPivot;
        private Vector3 lookDir;

        public bool onIdleDisableOh;
        public bool disable_o_h;
        public bool disable_m_h;

        public void Init(StatesManager st)
        {
            states = st;
            anim = states.anim;

            shoulder = anim.GetBoneTransform(HumanBodyBones.RightShoulder).transform;
            aimPivot = new GameObject().transform;
            aimPivot.name = "aim pivot";
            aimPivot.transform.parent = states.transform;
            rh_target = new GameObject().transform;
            rh_target.name = "right hand target";
            rh_target.parent = aimPivot;
            states.inp.aimPosition = states.transform.position + transform.forward * 15;
            states.inp.aimPosition.y += 1.4f;
        }

        public void EquipWeapon(RuntimeWeapon rw)
        {
            Weapon w = rw.w_actual;
            lh_target = rw.w_hook.leftHandIK;

            rh_target.localPosition = w.m_h_IK.pos;
            rh_target.localEulerAngles = w.m_h_IK.rot;
            basePosition = w.m_h_IK.pos;
            baseRotation = w.m_h_IK.rot;
            onIdleDisableOh = rw.w_actual.onIdleDisableOh;
        }

        private void OnAnimatorMove()
        {
            lookDir = states.inp.aimPosition - aimPivot.position;
            HandleShoulder();
        }

        private void HandleShoulder()
        {
            HandleShoulderPosition();
            HandleShoulderRotation();
        }

        private void HandleShoulderPosition()
        {
            aimPivot.position = shoulder.position;
        }

        private void HandleShoulderRotation()
        {
            Vector3 targetDir = lookDir;
            if (targetDir == Vector3.zero)
                targetDir = aimPivot.forward;
            Quaternion tr = Quaternion.LookRotation(targetDir);
            aimPivot.rotation = Quaternion.Slerp(aimPivot.rotation, tr, states.delta * 15);
        }

        private void HandleWeights()
        {
            if (states.states.isInteracting)
            {
                m_h_weight = 0;
                o_h_weight = 0;
                l_weight = 0;
                return;
            }

            float t_l_weight = 0;
            float t_m_weight = 0;

            if (states.states.isAiming)
            {
                t_m_weight = 1;
                b_weight = 0.4f;
            }
            else
            {
                b_weight = 0.3f;
            }

            if (disable_m_h)
                t_m_weight = 0;

            if (lh_target != null)
                o_h_weight = 1;
            else
                o_h_weight = 0;

            if (disable_o_h)
                o_h_weight = 0;

            Vector3 ld = states.inp.aimPosition - states.mTransform.position;
            float angle = Vector3.Angle(states.mTransform.forward, ld);

            if (angle < 76)
            {
                t_l_weight = 1;
            }
            else
            {
                t_l_weight = 0;
            }

            if (angle > 60)
                t_m_weight = 0;

            if (!states.states.isAiming)
            {
                if (onIdleDisableOh)
                    o_h_weight = 0;
            }
            l_weight = Mathf.Lerp(l_weight, t_l_weight, states.delta * 1);
            m_h_weight = Mathf.Lerp(m_h_weight, t_m_weight, states.delta * 9);
        }

        private void OnAnimatorIK()
        {
            HandleWeights();

            anim.SetLookAtWeight(l_weight, b_weight, 1, 1, 1);
            anim.SetLookAtPosition(states.inp.aimPosition);

            if (lh_target != null)
            {
                UpdateIK(AvatarIKGoal.LeftHand, lh_target, o_h_weight);
            }

            UpdateIK(AvatarIKGoal.RightHand, rh_target, m_h_weight);
        }

        private void UpdateIK(AvatarIKGoal goal, Transform t, float w)
        {
            anim.SetIKPositionWeight(goal, w);
            anim.SetIKRotationWeight(goal, w);
            anim.SetIKPosition(goal, t.position);
            anim.SetIKRotation(goal, t.rotation);
        }

        public void Tick()
        {
            RecoilActual();
        }

        #region Recoil
        private float recoilT;
        private Vector3 offsetPosition;
        private Vector3 offsetRotation;
        private Vector3 basePosition;
        private Vector3 baseRotation;
        private bool recoilIsInit;

        public void RecoilAnim()
        {
            if (!recoilIsInit)
            {
                recoilIsInit = true;
                recoilT = 0;
                offsetPosition = Vector3.zero;
            }
        }

        public void RecoilActual()
        {
            if (recoilIsInit)
            {
                recoilT += states.delta * 3;
                if (recoilT > 1)
                {
                    recoilT = 1;
                    recoilIsInit = false;
                }
                offsetPosition = Vector3.forward * states.w_manager.GetCurrent().w_actual.recoilZ.Evaluate(recoilT);
                offsetRotation = Vector3.right * 90 * -states.w_manager.GetCurrent().w_actual.recoilY.Evaluate(recoilT);

                rh_target.localPosition = basePosition + offsetPosition;
                rh_target.localEulerAngles = baseRotation + offsetRotation;
            }
        }
        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR
{
    public class StatesManager : MonoBehaviour
    {
        public ResourcesManager r_manager;
        public ControllerStats stats;
        public ControllerStates states;
        public InputVariables inp;
        public WeaponManager w_manager;
        public Character character;

        [System.Serializable]
        public class InputVariables
        {
            public float horizontal;
            public float vertical;
            public float moveAmount;
            public Vector3 moveDirection;
            public Vector3 aimPosition;
            public Vector3 rotateDirection;
        }

        [System.Serializable]
        public class ControllerStates
        {
            public bool onGround;
            public bool isAiming;
            public bool isCrouching;
            public bool isRunning;
            public bool isInteracting;
        }

        #region References
        public Animator anim;
        public GameObject activeModel;
        [HideInInspector]
        public AnimatorHook a_hook;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public Collider controllerCollider;

        private List<Collider> ragdollColliders = new List<Collider>();
        private List<Rigidbody> ragdollRigids = new List<Rigidbody>();
        [HideInInspector]
        public LayerMask ignoreLayers;
        [HideInInspector]
        public LayerMask ignoreForGround;

        //[HideInInspector]
        //public Transform referencesParent;
        [HideInInspector]
        public Transform mTransform;

        public CharState curState;
        public float delta;
        #endregion

        #region Init
        public void Init()
        {
            r_manager.Init();
            mTransform = this.transform;
            SetupAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.isKinematic = false;
            rigid.drag = 4;
            rigid.angularDrag = 999;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            controllerCollider = GetComponent<Collider>();

            SetupRagdols();

            gameObject.layer = 9;
            ignoreLayers = ~(1 << 9);
            ignoreForGround = ~(1 << 9 | 1 << 10);

            a_hook = activeModel.AddComponent<AnimatorHook>();
            a_hook.Init(this);

            Init_WeaponManager();

            character = GetComponent<Character>();
            character.Init(this);
        }

        private void SetupAnimator()
        {
            if(activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                activeModel = anim.gameObject;
            }

            if (anim == null)
                anim = activeModel.GetComponent<Animator>();

            anim.applyRootMotion = false;
        }

        private void SetupRagdols()
        {
            Rigidbody[] rigids = activeModel.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody r in rigids)
            {
                if (r == rigid)
                    continue;

                Collider c = r.gameObject.GetComponent<Collider>();
                c.isTrigger = true;
                ragdollRigids.Add(r);
                ragdollColliders.Add(c);
                r.isKinematic = true;
                r.gameObject.layer = 10;
            }
        }
        #endregion

        #region Fixed Update
        public void FixedTick(float d)
        {
            delta = d;
            switch (curState)
            {
                case CharState.normal:
                    states.onGround = OnGround();

                    if (states.isAiming)
                        MovementAiming();
                    else
                        MovementNormal();

                    RotationNormal();
                    break;
                case CharState.onAir:
                    rigid.drag = 0;
                    states.onGround = OnGround();
                    break;
                case CharState.cover:
                    break;
                case CharState.vaulting:
                    break;
                default:
                    break;
            }
        }

        private void MovementNormal()
        {
            if (inp.moveAmount > 0.05f)
                rigid.drag = 0;
            else
                rigid.drag = 4;

            float speed = stats.walkSpeed;
            if (states.isRunning)
                speed = stats.runSpeed;
            if (states.isCrouching)
                speed = stats.crounchSpeed;

            Vector3 dir = Vector3.zero;
            dir = mTransform.forward * (speed * inp.moveAmount);
            rigid.velocity = dir;
        }

        private void RotationNormal()
        {
            if(!states.isAiming)
                inp.rotateDirection = inp.moveDirection;

            Vector3 targetDir = inp.rotateDirection;
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = mTransform.forward;

            Quaternion lookDir = Quaternion.LookRotation(targetDir);
            Quaternion targetRot = Quaternion.Slerp(mTransform.rotation, lookDir, stats.rotateSpeed * delta);
            mTransform.rotation = targetRot;
        }

        private void MovementAiming()
        {
            if (inp.moveAmount > 0.05f)
                rigid.drag = 0;
            else
                rigid.drag = 4;

            float speed = stats.aimSpeed;
            Vector3 v = inp.moveDirection * speed;
            rigid.velocity = v;

        }
        #endregion

        #region Update
        public void Tick(float d)
        {
            delta = d;
            switch (curState)
            {
                case CharState.normal:
                    states.onGround = OnGround();
                    HandleAnimationAll();
                    break;
                case CharState.onAir:
                    states.onGround = OnGround();
                    break;
                case CharState.cover:
                    break;
                case CharState.vaulting:
                    break;
                default:
                    break;
            }
        }

        private void HandleAnimationAll()
        {
            anim.SetBool(StaticStrings.sprint, states.isRunning);
            anim.SetBool(StaticStrings.aiming, states.isAiming);
            anim.SetBool(StaticStrings.crouch, states.isCrouching);

            if (states.isAiming)
            {
                HandleAnimationsAiming();
            }
            else
            {
                HandleAnimationsNormal();
            }
        }

        private void HandleAnimationsNormal()
        {
            float anim_v = inp.moveAmount;
            anim.SetFloat(StaticStrings.vertical, anim_v, 0.15f, delta);
        }

        private void HandleAnimationsAiming()
        {
            float v = inp.vertical;
            float h = inp.horizontal;

            anim.SetFloat(StaticStrings.horizontal, h, 0.2f, delta);
            anim.SetFloat(StaticStrings.vertical, v, 0.2f, delta);
        }
        #endregion

        #region Manager Functions
        public void Init_WeaponManager()
        {
            CreateRuntimeWeapon(w_manager.mw_id, ref w_manager.m_weapon);
            EquipRuntimeWeapon(w_manager.m_weapon);
        }
        public void CreateRuntimeWeapon(string id, ref RuntimeWeapon r_w_m)
        {
            Weapon w = r_manager.GetWeapon(id);
            RuntimeWeapon rw = r_manager.runtime.WeaponToRuntimeWeapon(w);

            GameObject go = Instantiate(w.modelPrefab);
            rw.m_instance = go;
            rw.w_actual = w;
            rw.w_hook = go.GetComponent<WeaponHook>();
            go.SetActive(false);

            Transform p = anim.GetBoneTransform(HumanBodyBones.RightHand);
            go.transform.parent = p;
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localScale = Vector3.one;

            r_w_m = rw;
        }

        public void EquipRuntimeWeapon(RuntimeWeapon rw)
        {
            rw.m_instance.SetActive(true);
            a_hook.EquipWeapon(rw);

            anim.SetFloat(StaticStrings.weaponType, rw.w_actual.weaponType);
        }

        #endregion

        private bool OnGround()
        {
            Vector3 origin = mTransform.position;
            origin.y += 0.6f;
            Vector3 dir = -Vector3.up;
            float dis = 0.7f;
            RaycastHit hit;
            if(Physics.Raycast(origin,dir,out hit, dis, ignoreForGround))
            {
                Vector3 tp = hit.point;
                mTransform.position = tp;
                return true;
            }
            return false;
        }
    }

    public enum CharState
    {
        normal,onAir,cover,vaulting
    }
}
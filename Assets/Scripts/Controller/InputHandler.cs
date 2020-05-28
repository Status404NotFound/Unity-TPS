using SA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR
{
    public class InputHandler : MonoBehaviour
    {
        private float horizontal;
        private float vertical;

        private bool aimInput;
        private bool sprintInput;
        private bool shootInput;
        private bool crounchInput;
        private bool reloadInput;
        private bool switchInput;
        private bool pivotInput;

        private bool isInit;
        private bool updateUI;

        private float delta;

        public StatesManager states;
        public CameraHandler camHandler;
        public PlayerReferences p_references;
        
        private void Start()
        {
            InitInGame();
        }

        public void InitInGame()
        {
            states.Init();
            camHandler.Init(this);

            UpdatePlayerReferencesForWeapon(states.w_manager.GetCurrent());

            updateUI = true;
            isInit = true;
        }

        #region Fixed Update
        private void FixedUpdate()
        {
            if (!isInit)
                return;

            delta = Time.fixedDeltaTime;
            GetInput_FixedUpdate();
            InGame_UpdateStates_FixedUpdate();
            states.FixedTick(delta);

            camHandler.FixedTick(delta);

            if (states.rigid.velocity.sqrMagnitude > 0)
                p_references.targetSpread.value = 120;
            else
                p_references.targetSpread.value = 30;
        }

        private void GetInput_FixedUpdate()
        {
            vertical = Input.GetAxis(StaticStrings.Vertical);
            horizontal = Input.GetAxis(StaticStrings.Horizontal);
        }

        private void InGame_UpdateStates_FixedUpdate()
        {
            states.inp.horizontal = horizontal;
            states.inp.vertical = vertical;
            states.inp.moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            Vector3 moveDir = camHandler.mTransform.forward * vertical;
            moveDir += camHandler.mTransform.right * horizontal;
            moveDir.Normalize();
            states.inp.moveDirection = moveDir;

            states.inp.rotateDirection = camHandler.mTransform.forward;
        }
        #endregion

        public bool debugAim;

        #region Update
        private void Update()
        {
            if (!isInit)
                return;

            delta = Time.deltaTime;
            GetInput_Update();
            AimPosition();
            InGame_UpdateStates_Update();

            if (debugAim)
                states.states.isAiming = true;

            states.Tick(delta);

            if (updateUI)
            {
                updateUI = false;
                UpdatePlayerReferencesForWeapon(states.w_manager.GetCurrent());
                p_references.e_UpdateUI.Raise();
            }
        }

        private void GetInput_Update()
        {
            aimInput = Input.GetMouseButton(1);
            shootInput = Input.GetMouseButton(0);
        }

        private void InGame_UpdateStates_Update()
        {
            states.states.isAiming = aimInput;

            if (shootInput)
            {
                states.states.isAiming = true;
                bool shootActual = states.ShootWeapon(Time.realtimeSinceStartup);
                if (shootActual)
                {
                    updateUI = true;
                }
            }
        }

        private void AimPosition()
        {
            Ray ray = new Ray(camHandler.camTrans.position, camHandler.camTrans.forward);
            states.inp.aimPosition = ray.GetPoint(30);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, states.ignoreLayers))
            {
                states.inp.aimPosition = hit.point;
            }
        }
        #endregion

        #region Manager Functions
        public void UpdatePlayerReferencesForWeapon(RuntimeWeapon r)
        {
            p_references.curAmmo.value = r.curAmmo;
            p_references.curCarrying.value = r.curCarrying;
        }
        #endregion
    }

    public enum GamePhase
    {
        inGame, inMenu
    }
}
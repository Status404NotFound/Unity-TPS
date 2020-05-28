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
        private float delta;

        public StatesManager states;
        public CameraHandler camHandler;

        private void Start()
        {
            InitInGame();
        }

        public void InitInGame()
        {
            states.Init();
            camHandler.Init(this);
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
        }

        void GetInput_Update()
        {
            aimInput = Input.GetMouseButton(1);
        }

        void InGame_UpdateStates_Update()
        {
            states.states.isAiming = aimInput;
        }

        void AimPosition()
        {
            Ray ray = new Ray(camHandler.camTrans.position, camHandler.camTrans.forward);
            states.inp.aimPosition = ray.GetPoint(30);

            RaycastHit hit;
            if(Physics.Raycast(ray,out hit, 100, states.ignoreLayers))
            {
                states.inp.aimPosition = hit.point;
            }
        }
        #endregion
    }

    public enum GamePhase
    {
        inGame, inMenu
    }
}
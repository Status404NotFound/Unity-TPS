using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FR
{
    public class Character : MonoBehaviour
    {
        public Animator anim;
        public string outfitId;

        public CharObject hairObj;
        public CharObject eyebrowsObj;
        public Mask maskObj;
        private GameObject hair;
        private GameObject eyebrows;
        private GameObject mask;

        public bool isFemale;
        public SkinnedMeshRenderer bodyRenderer;

        public Transform eyebrowsBone;
        private List<GameObject> instancedObjs = new List<GameObject>();
        private ResourcesManager r_manager;

        public void Init(StatesManager st)
        {
            r_manager = st.r_manager;
            anim = st.anim;

            LoadCharacter();
            hair = LoadCharObject(hairObj);
            eyebrows = LoadCharObject(eyebrowsObj);
            mask = LoadMask(maskObj);
        }

        public void LoadCharacter()
        {
            MeshContainer m = r_manager.GetMesh(outfitId);
            LoadMeshContainer(m);
        }

        public void LoadMeshContainer(MeshContainer m)
        {
            bodyRenderer.sharedMesh = (isFemale) ? m.f_mesh : m.m_mesh;
            bodyRenderer.material = m.material;
        }

        public GameObject LoadMask(Mask m)
        {
            hair.SetActive(m.enableHair);
            eyebrows.SetActive(m.enableEyebrows);

            return LoadCharObject(m.obj);
        }

        public GameObject LoadCharObject(CharObject o)
        {
            Transform b = GetBone(o.parentBone);
            GameObject prefab = o.f_prefab;
            if (prefab == null || !isFemale)
                prefab = o.m_prefab;
            GameObject go = Instantiate(prefab);
            go.transform.parent = b;
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
            instancedObjs.Add(go);
            return go;
        }

        public Transform GetBone(MyBones b)
        {
            switch (b)
            {
                case MyBones.head:
                    return anim.GetBoneTransform(HumanBodyBones.Head);
                case MyBones.chest:
                    return anim.GetBoneTransform(HumanBodyBones.Chest);
                case MyBones.eyebrows:
                    return eyebrowsBone;
                case MyBones.rightHand:
                    return anim.GetBoneTransform(HumanBodyBones.RightHand);
                case MyBones.leftHand:
                    return anim.GetBoneTransform(HumanBodyBones.LeftHand);
                case MyBones.rightUpperLeg:
                    return anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
                case MyBones.hips:
                    return anim.GetBoneTransform(HumanBodyBones.Hips);
                default:
                    return null;
            }
        }
    }
}
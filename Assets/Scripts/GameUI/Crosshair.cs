﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FR.UI
{
    public class Crosshair : UIElement
    {
        public FloatVariable targetSpread;
        public float maxSpread = 80;
        public float spreadSpeed = 5;
        public Parts[] parts;
        private float t;
        private float curSpread;

        public override void Tick(float delta)
        {
            t = delta * spreadSpeed;
            curSpread = Mathf.Lerp(curSpread, targetSpread.value, t);
            for (int i = 0; i < parts.Length; i++)
            {
                Parts p = parts[i];
                p.trans.anchoredPosition = p.pos * curSpread;
            }
        }


        [System.Serializable]
        public class Parts
        {
            public RectTransform trans;
            public Vector2 pos;
        }
    }
}
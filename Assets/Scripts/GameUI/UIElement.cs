using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FR.UI
{
    public class UIElement : MonoBehaviour
    {
        private UIUpdater updater;

        private void Start()
        {
            updater = UIUpdater.singleton;

            if (updater != null)
                updater.elements.Add(this);

            Init();
        }

        public virtual void Init()
        {

        }

        public virtual void Tick(float delta)
        {

        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FR
{
    public class GameEventListener : MonoBehaviour
    {
        public GameEvent gameEvent;
        public UnityEvent response;

        private void OnEnable()
        {
            gameEvent.Register(this);
        }

        private void OnDisable()
        {
            gameEvent.UnRegister(this);
        }

        public void Response()
        {
            response.Invoke();
        }
    }
}
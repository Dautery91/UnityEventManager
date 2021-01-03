using UnityEngine;
using System.Collections;

namespace HitTheStuff.Events
{
    [EventListenerAtt]
    public class EventListenerMonoBase : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            if (EventManager.Current == null)
            {
                Debug.LogWarning("Event Manager Not Found!  Attempting delayed registration");
                StartCoroutine(TryRegister());
                return;
            }
            EventManager.Current.RegisterMyListeners(this);
        }

        protected virtual void OnDisable()
        {
            if (EventManager.Current == null) return;
            EventManager.Current.UnRegisterMyListeners(this);
        }

        private IEnumerator TryRegister()
        {
            while (EventManager.Current == null)
            {
                yield return null;
            }
            EventManager.Current.RegisterMyListeners(this);
        }
    }
}

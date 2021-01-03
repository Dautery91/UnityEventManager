using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HitTheStuff.Utility;


namespace HitTheStuff.Events
{
    /// <summary>
    /// This class makes it easier to tie together componenets that need to reference each other 
    /// ON THE SAME GAME OBJECT.  This is not a typical event manager / singleton pattern, though the code is
    /// very similar.  The key is that outside classes should not access it
    /// </summary>
    public class GOLocalEventManager : MonoBehaviour
    {
        delegate void LocalEventListener(EventInfoBase eventInfo);
        Dictionary<System.Type, List<LocalEventListener>> eventListeners;
        Dictionary<object, Dictionary<System.Type, List<LocalEventListener>>> instanceEventListeners;

        Dictionary<object, Dictionary<System.Type, List<UnityEventEventInfo>>> unityEventListeners;

        private void Awake()
        {
            if (instanceEventListeners == null)
            {
                instanceEventListeners = new Dictionary<object, Dictionary<System.Type, List<LocalEventListener>>>();
            }
        }

        public void RegisterLocalListener<T>(System.Action<T> listener, object callingObject) where T : EventInfoBase
        {
            System.Type eventType = typeof(T);

            if (instanceEventListeners == null)
            {
                instanceEventListeners = new Dictionary<object, Dictionary<System.Type, List<LocalEventListener>>>();
            }

            if (!instanceEventListeners.ContainsKey(callingObject))
            {
                instanceEventListeners.Add(callingObject, new Dictionary<System.Type, List<LocalEventListener>>());
            }

            if (instanceEventListeners[callingObject].ContainsKey(eventType) == false || instanceEventListeners[callingObject][eventType] == null)
            {
                instanceEventListeners[callingObject][eventType] = new List<LocalEventListener>();
            }

            // Wrap a type converstion around the event listener
            LocalEventListener wrapper = (ei) => { listener((T)ei); };

            instanceEventListeners[callingObject][eventType].Add(wrapper);
        }

        public void UnregisterLocalListener(object callingObject)
        {
            if (instanceEventListeners != null && 
                instanceEventListeners.ContainsKey(callingObject) && 
                instanceEventListeners[callingObject] != null)
            {
                instanceEventListeners[callingObject].Clear();
            }

            if (unityEventListeners != null &&
                unityEventListeners.ContainsKey(callingObject) && 
                 unityEventListeners[callingObject] != null)
            {
                unityEventListeners[callingObject].Clear();
            }
        }

        public void RegisterByString(UnityEventEventInfo unityEvent, string eventTypeString, object callingObject)
        {
            Type parameterType = GenericFactory<EventInfoBase>.GetFactoryObjectType(eventTypeString);


            if (unityEventListeners == null)
            {
                unityEventListeners = new Dictionary<object, Dictionary<Type, List<UnityEventEventInfo>>>();
            }

            if (!unityEventListeners.ContainsKey(callingObject))
            {
                unityEventListeners[callingObject] = new Dictionary<Type, List<UnityEventEventInfo>>();
            }

            if (!unityEventListeners[callingObject].ContainsKey(parameterType))
            {
                unityEventListeners[callingObject].Add(parameterType, new List<UnityEventEventInfo>());
            }

            unityEventListeners[callingObject][parameterType].Add(unityEvent);
        }

        /// <summary>
        /// Fires an event of type EventInfoParameter ONLY to the components on this game object
        /// who have subscribed
        /// </summary>
        /// <param name="eventInfo"></param>
        public void FireLocalEvent(EventInfoBase eventInfo)
        {
            if (!enabled) return;
            StartCoroutine(Fire(eventInfo));
            StartCoroutine(FireUnityEvents(eventInfo));
        }

        private IEnumerator Fire(EventInfoBase eventInfo)
        {
            System.Type trueEventInfoClass = eventInfo.GetType();

            if (instanceEventListeners == null) yield break;

            foreach (Dictionary<System.Type, List<LocalEventListener>> eventListeners in instanceEventListeners.Values)
            {
                if (!eventListeners.ContainsKey(trueEventInfoClass) || eventListeners[trueEventInfoClass] == null)
                {
                    // No one is listening, we are done.
                    continue;
                }

                foreach (LocalEventListener el in eventListeners[trueEventInfoClass])
                {
                    el(eventInfo);
                }

            }
            yield return null;
        }

        private IEnumerator FireUnityEvents(EventInfoBase eventInfo)
        {
            System.Type trueEventInfoClass = eventInfo.GetType();

            if (unityEventListeners == null) yield break;

            foreach (Dictionary<System.Type, List<UnityEventEventInfo>> eventListeners in unityEventListeners.Values)
            {
                if (!eventListeners.ContainsKey(trueEventInfoClass) || eventListeners[trueEventInfoClass] == null)
                {
                    // No one is listening, we are done.
                    continue;
                }

                foreach (UnityEventEventInfo el in eventListeners[trueEventInfoClass])
                {
                    el.Invoke(eventInfo);
                }

            }
            yield return null;
        }
    }
}

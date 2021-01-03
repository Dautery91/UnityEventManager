using UnityEngine;
using HitTheStuff.Utility;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace HitTheStuff.Events
{
    public class CustomEventsListener : MonoBehaviour
    {
#pragma warning disable CS0649
        [SerializeField] bool useGlobalEvents = true;
        [SerializeField] bool useLocalEvents = true;

#if UNITY_EDITOR
[ShowIf("useGlobalEvents")]
#endif
        [SerializeField] CustomEventToUnityEvent[] eventListenerMappings;

#if UNITY_EDITOR
        [ShowIf("useLocalEvents")]
#endif
        [SerializeField] CustomEventToUnityEvent[] localEventListenerMappings;

#pragma warning restore CS0649

        private GOLocalEventManager localEM = null;

#if UNITY_EDITOR
        [Button("Validate Strings")]
#endif
        private void Validation()
        {
            if (useGlobalEvents)
            {
                foreach (CustomEventToUnityEvent mapping in eventListenerMappings)
                {
                    if (!GenericFactory<EventInfoBase>.CheckIfTypeExists(mapping.eventType))
                    {
                        Debug.Log("No event exists with this name! : " + mapping.eventType);
                    }
                }
            }

            if (useLocalEvents)
            {
                foreach (CustomEventToUnityEvent mapping in localEventListenerMappings)
                {
                    if (!GenericFactory<EventInfoBase>.CheckIfTypeExists(mapping.eventType))
                    {
                        Debug.Log("No event exists with this name! : " + mapping.eventType);
                    }
                }
            }
        }

        private void Awake()
        {
            if (useLocalEvents && localEM == null)
            {
                localEM = GetComponent<GOLocalEventManager>();
                if (localEM == null) localEM = GetComponentInParent<GOLocalEventManager>();
                if (localEM == null) localEM = GetComponentInChildren<GOLocalEventManager>();
            }
        }

        private void OnEnable()
        {
            if (useGlobalEvents)
            {
                foreach (CustomEventToUnityEvent mapping in eventListenerMappings)
                {
                    EventManager.Current.RegisterByString(mapping.Listeners, mapping.eventType);
                }
            }

            if (useLocalEvents)
            {
                foreach (CustomEventToUnityEvent mapping in localEventListenerMappings)
                {
                    localEM.RegisterByString(mapping.Listeners, mapping.eventType, this);
                }
            }
        }

        private void OnDisable()
        {
            if (useGlobalEvents && EventManager.Current != null)
            {
                foreach (CustomEventToUnityEvent mapping in eventListenerMappings)
                {
                    EventManager.Current.UnregisterByString(mapping.Listeners, mapping.eventType);
                }
            }

            if (useLocalEvents && localEM != null)
            {
                localEM.UnregisterLocalListener(this);
            }
        }
    }
}

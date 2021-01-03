using UnityEngine;

namespace HitTheStuff.Events
{
    [System.Serializable]
    public struct CustomEventToUnityEvent
    {
        [EventType] public string eventType;
        public UnityEventEventInfo Listeners;
    }
}

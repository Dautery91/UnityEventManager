using HitTheStuff.Utility;
using UnityEngine;

namespace HitTheStuff.Events
{
    /// <summary>
    /// Need a new one of these for each new event type.  Can pass as much or as little info as you want
    /// </summary>
    public abstract class EventInfoBase : FactoryObjectBase
    {
        public string EventDescription = "Default";

        public EventInfoBase()
        { 
            
        }
    }

    public class EventTypeAttribute : PropertyAttribute
    { }
}

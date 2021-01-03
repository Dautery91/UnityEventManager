using System;

namespace HitTheStuff.Events
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class EventListenerAtt : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class GlobalEventListenerAtt : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class LocalEventListenerAtt : Attribute
    { 
        
    }

}

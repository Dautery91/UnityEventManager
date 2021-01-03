using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using HitTheStuff.Utility;

namespace HitTheStuff.Events
{
    public class EventManager : MonoBehaviour
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        void OnEnable()
        {
            __Current = this;
        }

        static private EventManager __Current;
        static public EventManager Current
        {
            get
            {
                if (__Current == null)
                {
                    __Current = GameObject.FindObjectOfType<EventManager>();
                }

                return __Current;
            }
        }

        bool isSetUp = false;

        Dictionary<System.Type, List<MethodInfo>> typeToMethods;
        Dictionary<System.Type, List<Tuple<MethodInfo,object>>> globalEventListeners;
        Dictionary<System.Type, List<UnityEventEventInfo>> unityEventListeners;

        private void Awake()
        {
            ReflectListenerClasses();
        }

        private IEnumerator WaitForFinishLoading()
        {
            yield return waitForEndOfFrame;
            __Current = GameObject.FindObjectOfType<EventManager>();
        }

        /// <summary>
        /// Registers all of the calling class' methods who have been tagged with the proper EventListener attributes as listeners
        /// to the events whose info is taken in as the method's parameter
        /// </summary>
        /// <param name="callingObject"></param>
        public void RegisterMyListeners(object callingObject)
        {
            if (!isSetUp)
            {
                ReflectListenerClasses();
            }

            if (globalEventListeners == null)
            {
                globalEventListeners = new Dictionary<System.Type, List<Tuple<MethodInfo, object>>>();
            }

            if (!typeToMethods.ContainsKey(callingObject.GetType()))
            {
                //Debug.Log("NO Methods to register as listeners!:" + callingObject.GetType());
                return;
            }

            foreach (MethodInfo mi in typeToMethods[callingObject.GetType()])
            {
                Type parameterType = mi.GetParameters()[0].ParameterType;

                if (!globalEventListeners.ContainsKey(parameterType) || globalEventListeners[parameterType] == null)
                {
                    globalEventListeners[parameterType] = new List<Tuple<MethodInfo, object>>();
                }

                globalEventListeners[parameterType].Add(new Tuple<MethodInfo, object>(mi, callingObject));

            }
        }

        /// <summary>
        /// Unregisters all of the calling class' methods who have been tagged with the proper EventListener attributes
        /// </summary>
        /// <param name="callingObject"></param>
        public void UnRegisterMyListeners(object callingObject)
        {
            if (callingObject == null)
            {
                return;
            }

            if (globalEventListeners == null || !typeToMethods.ContainsKey(callingObject.GetType()))
            {
                //Debug.Log("Trying to remove listener that isnt registered");
                return;
            }

            foreach (MethodInfo mi in typeToMethods[callingObject.GetType()])
            {
                Type parameterType = mi.GetParameters()[0].ParameterType;

                if (!globalEventListeners.ContainsKey(parameterType) || globalEventListeners[parameterType] == null)
                {
                    //Debug.Log("Trying to remove listener that isnt registered");
                    return;
                }

                globalEventListeners[parameterType].RemoveAll(item => item.Item2 == callingObject);
            }

        }

        public void RegisterByString(UnityEventEventInfo unityEvent, string eventTypeString)
        {
            Type parameterType = GenericFactory<EventInfoBase>.GetFactoryObjectType(eventTypeString);

            if (unityEventListeners == null)
            {
                unityEventListeners = new Dictionary<Type, List<UnityEventEventInfo>>();
            }

            if (!unityEventListeners.ContainsKey(parameterType) || unityEventListeners[parameterType] == null)
            {
                unityEventListeners[parameterType] = new List<UnityEventEventInfo>();
            }

            unityEventListeners[parameterType].Add(unityEvent);
        }

        public void UnregisterByString(UnityEventEventInfo unityEvent, string eventTypeString)
        {
            Type parameterType = GenericFactory<EventInfoBase>.GetFactoryObjectType(eventTypeString);
            if (!unityEventListeners.ContainsKey(parameterType) || unityEventListeners[parameterType] == null)
            {
                return;
            }
            unityEventListeners[parameterType].Remove(unityEvent);
        }

        public void FireGlobalEvent(EventInfoBase eventInfo)
        {
            System.Type trueEventInfoClass = eventInfo.GetType();

            if ((globalEventListeners == null || !globalEventListeners.ContainsKey(trueEventInfoClass)) &&
                (unityEventListeners == null || !unityEventListeners.ContainsKey(trueEventInfoClass)))
            {
                // No one is listening, we are done.
                return;
            }

            StartCoroutine(FireEventRoutine(eventInfo, trueEventInfoClass));
        }

        private IEnumerator FireEventRoutine(EventInfoBase eventInfo, Type trueEventInfoClass)
        {
            object[] param = new object[1] { eventInfo };

            if (globalEventListeners.ContainsKey(trueEventInfoClass))
            {
                foreach (Tuple<MethodInfo, object> tuple in globalEventListeners[trueEventInfoClass])
                {
                    tuple.Item1.Invoke(tuple.Item2, param);
                }
            }

            if (unityEventListeners == null) yield break;
            if (!unityEventListeners.ContainsKey(trueEventInfoClass)) yield break;

            foreach (UnityEventEventInfo unityEventInfo in unityEventListeners[trueEventInfoClass])
            {
                unityEventInfo.Invoke(eventInfo);
            }

            yield return null;
        }

        // Maps and stores each listener type's listener methods once on Awake for more efficient use
        private void ReflectListenerClasses()
        {
            typeToMethods = new Dictionary<Type, List<MethodInfo>>();

            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.GetCustomAttribute(typeof(EventListenerAtt)) != null)
                {
                    foreach (MethodInfo mi in t.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    {
                        AddMethodsWithAttributes(t, mi);
                    }
                }
            }

            isSetUp = true;
        }

        private void AddMethodsWithAttributes(Type t, MethodInfo mi)
        {
            if (mi.GetCustomAttribute(typeof(GlobalEventListenerAtt)) != null)
            {
                if (!typeToMethods.ContainsKey(t))
                {
                    typeToMethods.Add(t, new List<MethodInfo>());
                }

                typeToMethods[t].Add(mi);

            }
        }
    }
}

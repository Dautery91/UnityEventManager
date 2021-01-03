using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using HitTheStuff.Utility;

namespace HitTheStuff.Events.Editor
{
    /// <summary>
    /// Draws a string field with this attribute as a drop down containing all EventInfo types.  Could exted easily to other types
    /// using the GenericFactory
    /// </summary>
    [CustomPropertyDrawer(typeof(EventTypeAttribute))]
    public class EventInfoPropertyDrawer : PropertyDrawer
    {
        public int index;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string eventInfoName = "NOT ASSIGNED";
            var eventInfoNames = new List<string>();

            if (GenericFactory<EventInfoBase>.TypesByName != null)
            {
                eventInfoNames.AddRange(GenericFactory<EventInfoBase>.TypesByName.Keys);
            }
            else
            {
                Debug.LogError("No Event Info Base types found!");
            }

            eventInfoNames.Sort();

            index = eventInfoNames.IndexOf(property.stringValue);

            index = EditorGUI.Popup(position, label.text, index, eventInfoNames.ToArray());
            eventInfoName = eventInfoNames[index];

            property.stringValue = eventInfoName;
        }
    }
}

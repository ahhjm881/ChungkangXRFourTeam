using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace XRProject.Utils
{
    [CustomPropertyDrawer(typeof(XLogFilterPropertyDrawer))]
    public class XLogFilterPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            _property.intValue = EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditorInternal;

namespace FlexibleValue
{
    [CustomPropertyDrawer(typeof(Value))]
    public class ValueDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var fieldProp = property.FindPropertyRelative("_fv");
            EditorGUI.PropertyField(position, fieldProp, label);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight + 5;
        }
    }
}

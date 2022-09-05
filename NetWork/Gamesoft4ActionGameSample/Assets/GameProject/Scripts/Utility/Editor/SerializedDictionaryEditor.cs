using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
public class SerializedDictionaryEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var fieldProp = property.FindPropertyRelative("_list");
        EditorGUI.PropertyField(position, fieldProp, label, true);
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //            float height = base.GetPropertyHeight(property, label);
        float height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_list"), true) + EditorGUIUtility.standardVerticalSpacing;
        return height;
        //            return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight + 5;
    }
}

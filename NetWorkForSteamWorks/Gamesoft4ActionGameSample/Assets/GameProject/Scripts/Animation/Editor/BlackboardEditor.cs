using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditorInternal;

using UnityEditor.UIElements;
using UnityEngine.UIElements;

/*
[CustomPropertyDrawer(typeof(AnyValue))]
public class AnyValueDrawer : PropertyDrawer
{
    string[] options = { "Class", "int", "float", "string", "UnityObject" };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        EditorGUI.BeginProperty(position, label, property);

        var valueProp = property.FindPropertyRelative("Value");
        // ƒ‰ƒxƒ‹
        var rect = position;
        EditorGUI.LabelField(rect, label);
        rect.x += EditorGUIUtility.labelWidth;
        rect.width -= EditorGUIUtility.labelWidth;
        rect.height = EditorGUIUtility.singleLineHeight;
        // ’l
        //        EditorGUI.PropertyField(rect, valueProp, GUIContent.none);

        // 
        int index = 0;
        var anyV = (AnyValue)property.managedReferenceValue;
        if (anyV.Value is int)
        {
            index = 1;
            anyV.Value = EditorGUI.IntField(rect, (int)anyV.Value);
        }
        else if (anyV.Value is float)
        {
            index = 2;
            anyV.Value = EditorGUI.FloatField(rect, (float)anyV.Value);
        }
        else if (anyV.Value is string)
        {
            index = 3;
            anyV.Value = EditorGUI.TextField(rect, (string)anyV.Value);
        }
        else
        {
            index = 4;
            anyV.Value = EditorGUI.ObjectField(rect, (Object)anyV.Value, typeof(Object), true);
        }

        rect.x = position.x + EditorGUIUtility.labelWidth;
        rect.y += EditorGUIUtility.singleLineHeight;
        rect.width = position.width - EditorGUIUtility.labelWidth;
        rect.height = EditorGUIUtility.singleLineHeight;

        int retIndex = EditorGUI.Popup(
                    rect,
                    index,
                    options);

        if(retIndex != index)
        {
            switch(retIndex)
            {
                case 1:
                    property.managedReferenceValue = new AnyValue(0);
                    break;
                case 3:
                    property.managedReferenceValue = new AnyValue("");
                    break;
                case 4:
                    property.managedReferenceValue = new AnyValue(null);
                    break;
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight;
    }
}
*/

/*

[CustomPropertyDrawer(typeof(IntValue))]
public class IntValueDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var valueProp = property.FindPropertyRelative("Value");

        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.LabelField(position, label);
        position.x += EditorGUIUtility.labelWidth;
        position.width -= EditorGUIUtility.labelWidth;
        EditorGUI.PropertyField(position, valueProp, GUIContent.none);

        EditorGUI.EndProperty();
    }

}

[CustomPropertyDrawer(typeof(ObjectValue))]
public class ObjectValueDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var valueProp = property.FindPropertyRelative("Value");

        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.LabelField(position, label);
        position.x += EditorGUIUtility.labelWidth;
        position.width -= EditorGUIUtility.labelWidth;
        EditorGUI.PropertyField(position, valueProp, GUIContent.none);

        EditorGUI.EndProperty();
    }

}
*/

/*
[CustomEditor(typeof(Blackboard))]
public class BlackboardEditor : Editor
{
    private ReorderableList _reorderableList;

    public void OnEnable()
    {
        var dicProp = serializedObject.FindProperty("_datas");
        var listProp = dicProp.FindPropertyRelative("_list");

        _reorderableList = new ReorderableList(dicProp.serializedObject, listProp);

        _reorderableList.elementHeight = 130;
        _reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = listProp.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

//        var listProp = serializedObject.FindProperty("_datas");

        // •`‰æ
        _reorderableList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
*/

/*
[CustomPropertyDrawer(typeof(AnyDictionary.Node))]
public class SerializableDictionaryObjectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;

        var valueTypeProp = property.FindPropertyRelative("ValueType");

        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("Key"), GUIContent.none);

        var type = (AnyDictionary.Types)valueTypeProp.intValue;

        if (type == AnyDictionary.Types.Object)
        {
            rect.y += EditorGUIUtility.singleLineHeight;
            //        var rcValue = new Rect(position.x, position.y)
            EditorGUI.ObjectField(rect, property.FindPropertyRelative("Value"), GUIContent.none);
        }

        // 
        rect.y += EditorGUIUtility.singleLineHeight;
        Rect rcButton = rect;
        rcButton.width /= 4;
        if (GUI.Button(rect, "int"))
        {

        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 3;
    }
}
*/
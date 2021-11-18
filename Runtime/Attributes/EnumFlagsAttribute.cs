using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

// via http://www.sharkbombs.com/2015/02/17/unity-editor-enum-flags-as-toggle-buttons/

public class EnumFlagAttribute : PropertyAttribute
{

}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{
 
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        int buttonsIntValue = 0;
        int enumLength = _property.enumNames.Length;
        bool[] buttonPressed = new bool[enumLength];
        float buttonWidth = (_position.width - EditorGUIUtility.labelWidth) / enumLength;
 
        EditorGUI.LabelField(new Rect(_position.x, _position.y, EditorGUIUtility.labelWidth, _position.height), _label);
 
        EditorGUI.BeginChangeCheck ();
 
        for (int i = 0; i < enumLength; i++) {
 
            // Check if the button is/was pressed 
            if ( ( _property.intValue & (1 << i) ) == 1 << i ) {
                buttonPressed[i] = true;
            }
 
            Rect buttonPos = new Rect (_position.x + EditorGUIUtility.labelWidth + buttonWidth * i, _position.y, buttonWidth, _position.height);
 
            buttonPressed[i] = GUI.Toggle(buttonPos, buttonPressed[i], _property.enumNames[i],  "Button");
 
            if (buttonPressed[i])
                buttonsIntValue += 1 << i;
        }
 
        if (EditorGUI.EndChangeCheck()) {
            _property.intValue = buttonsIntValue;
        }
    }
}

#endif // UNITY_EDITOR
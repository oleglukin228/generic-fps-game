using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractableActions))]
public class InteractableActionsEditor : Editor
{
    private SerializedProperty itemsProperty;

    private void OnEnable()
    {
        itemsProperty = serializedObject.FindProperty("items");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        for (int i = 0; i < itemsProperty.arraySize; i++)
        {
            SerializedProperty itemProperty = itemsProperty.GetArrayElementAtIndex(i);
            SerializedProperty selectionProperty = itemProperty.FindPropertyRelative("selection");
            SerializedProperty vectorDirectionProperty = itemProperty.FindPropertyRelative("vectorDirection");
            SerializedProperty interactionSpeedProperty = itemProperty.FindPropertyRelative("interactionSpeed");
            SerializedProperty tweensProperty = itemProperty.FindPropertyRelative("tweens");
            SerializedProperty startEventProperty = itemProperty.FindPropertyRelative("onStartActionTrigger");
            SerializedProperty endEventProperty = itemProperty.FindPropertyRelative("onEndActionTrigger");

            EditorGUILayout.LabelField("Action " + i);
            EditorGUILayout.PropertyField(interactionSpeedProperty);
            EditorGUILayout.PropertyField(vectorDirectionProperty);
            EditorGUILayout.PropertyField(tweensProperty);
            EditorGUILayout.PropertyField(startEventProperty);
            EditorGUILayout.PropertyField(endEventProperty);

            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Add Action"))
        {
            itemsProperty.InsertArrayElementAtIndex(itemsProperty.arraySize);
        }

        if (GUILayout.Button("Remove Last Action"))
        {
            if (itemsProperty.arraySize > 0)
            {
                itemsProperty.DeleteArrayElementAtIndex(itemsProperty.arraySize - 1);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}

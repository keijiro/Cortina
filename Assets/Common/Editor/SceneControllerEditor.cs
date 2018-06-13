using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Cortina
{
    [CustomEditor(typeof(SceneController))]
    sealed class SceneControllerEditor : Editor
    {
        ReorderableList _activators;

        static class Styles
        {
            public static readonly GUIContent Line = new GUIContent("Line");
        }

        void OnEnable()
        {
            _activators = new ReorderableList(
                serializedObject,
                serializedObject.FindProperty("_activators"),
                true, // draggable
                true, // displayHeader
                true, // displayAddButton
                true  // displayRemoveButton
            );

            _activators.drawHeaderCallback = (Rect rect) => {  
                EditorGUI.LabelField(rect, "Activators");
            };

            _activators.drawElementCallback = (Rect frame, int index, bool isActive, bool isFocused) => {
                var activator = _activators.serializedProperty.GetArrayElementAtIndex(index);

                var rect = frame;
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;

                rect.width = (frame.width - 96) / 2;
                EditorGUI.PropertyField(rect, activator.FindPropertyRelative("Intro"), GUIContent.none);

                rect.x += rect.width + 8;
                EditorGUI.PropertyField(rect, activator.FindPropertyRelative("Outro"), GUIContent.none);

                rect.x = frame.x + frame.width - 80;
                rect.width = 80;
                EditorGUI.PropertyField(rect, activator.FindPropertyRelative("Key"), GUIContent.none);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            _activators.DoLayoutList();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}

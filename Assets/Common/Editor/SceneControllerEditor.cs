using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Cortina
{
    [CustomEditor(typeof(SceneController))]
    sealed class SceneControllerEditor : Editor
    {
        ReorderableList _effectActivators;
        ReorderableList _optionActivators;

        void DrawActivatorElement(Rect frame, SerializedProperty prop)
        {
            var rect = frame;
            rect.y += 2;
            rect.height = EditorGUIUtility.singleLineHeight;

            rect.width = (frame.width - 96) / 2;
            EditorGUI.PropertyField(rect, prop.FindPropertyRelative("Intro"), GUIContent.none);

            rect.x += rect.width + 8;
            EditorGUI.PropertyField(rect, prop.FindPropertyRelative("Outro"), GUIContent.none);

            rect.x = frame.x + frame.width - 80;
            rect.width = 80;
            EditorGUI.PropertyField(rect, prop.FindPropertyRelative("Key"), GUIContent.none);
        }

        void OnEnable()
        {
            _effectActivators = new ReorderableList(
                serializedObject, serializedObject.FindProperty("_effectActivators"),
                true, true, true, true
            );

            _optionActivators = new ReorderableList(
                serializedObject, serializedObject.FindProperty("_optionActivators"),
                true, true, true, true
            );

            _effectActivators.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Effects");
            }; 

            _optionActivators.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Options");
            }; 

            _effectActivators.drawElementCallback = (Rect frame, int index, bool isActive, bool isFocused) => {
                DrawActivatorElement(frame, _effectActivators.serializedProperty.GetArrayElementAtIndex(index));
            };

            _optionActivators.drawElementCallback = (Rect frame, int index, bool isActive, bool isFocused) => {
                DrawActivatorElement(frame, _optionActivators.serializedProperty.GetArrayElementAtIndex(index));
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();
            _effectActivators.DoLayoutList();
            EditorGUILayout.Space();
            _optionActivators.DoLayoutList();
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }
    }
}

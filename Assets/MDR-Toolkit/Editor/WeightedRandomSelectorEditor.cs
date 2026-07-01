// ============================================================
// MDR Unity Toolkit - WeightedRandomSelector
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MDR.Toolkit.Utility
{
    /// <summary>
    /// Custom Inspector for <see cref="WeightedRandomSelectorDemo"/> that visualizes each
    /// entry's weight as a percentage bar and allows picking a random entry in edit mode.
    /// </summary>
    [CustomEditor(typeof(WeightedRandomSelectorDemo))]
    public class WeightedRandomSelectorEditor : Editor
    {
        private SerializedProperty entriesProperty;

        private void OnEnable()
        {
            entriesProperty = serializedObject.FindProperty("entries");
        }

        /// <summary>
        /// Draws the default Inspector plus a weight visualization and a manual pick button.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Weight Distribution", EditorStyles.boldLabel);
            DrawWeightBars();

            EditorGUILayout.Space();
            if (GUILayout.Button("Pick Random (Editor)"))
            {
                ((WeightedRandomSelectorDemo)target).PickRandom();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawWeightBars()
        {
            if (entriesProperty == null || entriesProperty.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No entries configured.", MessageType.Info);
                return;
            }

            float totalWeight = 0f;
            for (int i = 0; i < entriesProperty.arraySize; i++)
            {
                SerializedProperty entry = entriesProperty.GetArrayElementAtIndex(i);
                totalWeight += entry.FindPropertyRelative("Weight").floatValue;
            }

            for (int i = 0; i < entriesProperty.arraySize; i++)
            {
                SerializedProperty entry = entriesProperty.GetArrayElementAtIndex(i);
                string value = entry.FindPropertyRelative("Value").stringValue;
                float weight = entry.FindPropertyRelative("Weight").floatValue;
                float percentage = totalWeight > 0f ? weight / totalWeight * 100f : 0f;

                Rect rowRect = EditorGUILayout.GetControlRect(false, 20f);
                Rect labelRect = new Rect(rowRect.x, rowRect.y, rowRect.width * 0.3f, rowRect.height);
                Rect barBackgroundRect = new Rect(labelRect.xMax + 4f, rowRect.y, rowRect.width * 0.55f, rowRect.height);
                Rect percentRect = new Rect(barBackgroundRect.xMax + 4f, rowRect.y, rowRect.width * 0.15f - 8f, rowRect.height);

                EditorGUI.LabelField(labelRect, string.IsNullOrEmpty(value) ? "(empty)" : value);

                GUI.Box(barBackgroundRect, GUIContent.none);
                Rect barFillRect = new Rect(barBackgroundRect.x, barBackgroundRect.y, barBackgroundRect.width * (percentage / 100f), barBackgroundRect.height);
                EditorGUI.DrawRect(barFillRect, new Color(0.3f, 0.6f, 1f, 0.8f));

                EditorGUI.LabelField(percentRect, $"{percentage:0.0}%");
            }
        }
    }
}
#endif

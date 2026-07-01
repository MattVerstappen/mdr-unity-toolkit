// ============================================================
// MDR Unity Toolkit - AudioRandomizer
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MDR.Toolkit.Audio
{
    /// <summary>
    /// Custom Inspector for <see cref="AudioRandomizer"/> that visualizes each entry's
    /// weight as a percentage bar and provides an in-editor playback preview.
    /// </summary>
    [CustomEditor(typeof(AudioRandomizer))]
    public class AudioRandomizerEditor : Editor
    {
        private SerializedProperty entriesProperty;

        private void OnEnable()
        {
            entriesProperty = serializedObject.FindProperty("entries");
        }

        /// <summary>
        /// Draws the default Inspector plus a clip weight visualization and a preview section.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Clip Weight Distribution", EditorStyles.boldLabel);
            DrawWeightBars();

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

            AudioRandomizer randomizer = (AudioRandomizer)target;

            if (GUILayout.Button("Play Random (Preview)"))
            {
                randomizer.PlayRandom();
                Repaint();
            }

            AudioClip lastPlayed = randomizer.GetLastPlayed();
            EditorGUILayout.LabelField("Last Played:", lastPlayed != null ? lastPlayed.name : "None");

            List<AudioClip> queue = randomizer.GetAntiRepeatQueue();
            string queueLabel = queue.Count > 0 ? string.Join(", ", queue.ConvertAll(c => c != null ? c.name : "None")) : "(empty)";
            EditorGUILayout.LabelField("Anti-Repeat Queue:", queueLabel);
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
                SerializedProperty clipProperty = entry.FindPropertyRelative("Clip");
                float weight = entry.FindPropertyRelative("Weight").floatValue;
                float percentage = totalWeight > 0f ? weight / totalWeight * 100f : 0f;

                AudioClip clip = clipProperty.objectReferenceValue as AudioClip;
                string label = clip != null ? clip.name : "(no clip)";

                Rect rowRect = EditorGUILayout.GetControlRect(false, 20f);
                Rect labelRect = new Rect(rowRect.x, rowRect.y, rowRect.width * 0.3f, rowRect.height);
                Rect barBackgroundRect = new Rect(labelRect.xMax + 4f, rowRect.y, rowRect.width * 0.55f, rowRect.height);
                Rect percentRect = new Rect(barBackgroundRect.xMax + 4f, rowRect.y, rowRect.width * 0.15f - 8f, rowRect.height);

                EditorGUI.LabelField(labelRect, label);

                GUI.Box(barBackgroundRect, GUIContent.none);
                Rect barFillRect = new Rect(barBackgroundRect.x, barBackgroundRect.y, barBackgroundRect.width * (percentage / 100f), barBackgroundRect.height);
                EditorGUI.DrawRect(barFillRect, new Color(1f, 0.6f, 0.2f, 0.8f));

                EditorGUI.LabelField(percentRect, $"{percentage:0.0}%");
            }
        }
    }
}
#endif

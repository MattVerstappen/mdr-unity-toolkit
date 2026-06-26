// ============================================================
// MDR Unity Toolkit - WeightedRandomSelector
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MDR.Toolkit.Utility
{
    /// <summary>
    /// Custom Inspector for <see cref="WeightedRandomSelectorDemo"/>. Draws a visual
    /// weight bar for each entry, showing the percentage each weight contributes to the
    /// total, and adds a button to pick a random value directly in edit mode.
    /// </summary>
    [CustomEditor(typeof(WeightedRandomSelectorDemo))]
    public class WeightedRandomSelectorEditor : Editor
    {
        private const float BarHeight = 18f;

        public override void OnInspectorGUI()
        {
            // Draw the default fields (the entry list, etc.) first.
            DrawDefaultInspector();

            WeightedRandomSelectorDemo demo = (WeightedRandomSelectorDemo)target;
            IReadOnlyList<WeightedEntry<string>> entries = demo.Entries;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Weight Distribution", EditorStyles.boldLabel);

            if (entries == null || entries.Count == 0)
            {
                EditorGUILayout.HelpBox("No entries configured.", MessageType.Info);
            }
            else
            {
                float totalWeight = 0f;
                for (int i = 0; i < entries.Count; i++)
                {
                    if (entries[i] != null && entries[i].Weight > 0f)
                    {
                        totalWeight += entries[i].Weight;
                    }
                }

                if (totalWeight <= 0f)
                {
                    EditorGUILayout.HelpBox(
                        "Total weight is zero. Assign positive weights to see the distribution.",
                        MessageType.Warning);
                }
                else
                {
                    for (int i = 0; i < entries.Count; i++)
                    {
                        WeightedEntry<string> entry = entries[i];
                        if (entry == null)
                        {
                            continue;
                        }

                        float weight = Mathf.Max(0f, entry.Weight);
                        float percent = (weight / totalWeight) * 100f;
                        DrawWeightBar(entry.Value, percent);
                    }
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Pick Random (Editor)"))
            {
                demo.PickRandom();
            }
        }

        /// <summary>
        /// Draws a single labelled weight bar filled to <paramref name="percent"/> of its width.
        /// </summary>
        /// <param name="label">The entry value to display.</param>
        /// <param name="percent">The percentage of the total weight (0–100).</param>
        private static void DrawWeightBar(string label, float percent)
        {
            string displayLabel = string.IsNullOrEmpty(label) ? "(empty)" : label;

            // Reserve a full-width row for the bar.
            Rect row = EditorGUILayout.GetControlRect(false, BarHeight);

            // Background track.
            GUI.Box(row, GUIContent.none);

            // Filled portion proportional to the percentage.
            float fillWidth = row.width * Mathf.Clamp01(percent / 100f);
            Rect fill = new Rect(row.x, row.y, fillWidth, row.height);

            Color previous = GUI.color;
            GUI.color = new Color(0.30f, 0.65f, 1f, 0.85f);
            GUI.Box(fill, GUIContent.none);
            GUI.color = previous;

            // Overlay the label and percentage text on top of the bar.
            Rect labelRect = new Rect(row.x + 6f, row.y, row.width - 12f, row.height);
            string text = $"{displayLabel}    {percent:0.0}%";
            EditorGUI.LabelField(labelRect, text, EditorStyles.miniBoldLabel);
        }
    }
}
#endif

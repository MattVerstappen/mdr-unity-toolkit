// ============================================================
// MDR Unity Toolkit - TraumaCameraShake
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MDR.Toolkit.Camera
{
    /// <summary>
    /// Custom Inspector for <see cref="TraumaCameraShake"/>. Shows a live trauma readout
    /// and quick-test buttons during Play Mode, and an explanation of the trauma model otherwise.
    /// </summary>
    [CustomEditor(typeof(TraumaCameraShake))]
    public class TraumaCameraShakeEditor : Editor
    {
        /// <summary>
        /// Draws the default Inspector plus a play-mode trauma preview, or a help box in edit mode.
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            if (Application.isPlaying)
            {
                DrawPlayModeControls();
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "TraumaCameraShake uses the GDC trauma model: trauma is a value from 0 to 1 that " +
                    "decays over time, and shake magnitude is trauma^2 for a natural falloff curve. " +
                    "Offsets are sampled from Perlin noise per axis rather than random jitter. " +
                    "Enter Play Mode to test trauma live.",
                    MessageType.Info);
            }
        }

        /// <summary>
        /// Forces continuous repaint during Play Mode so the trauma bar updates live as it decays.
        /// </summary>
        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

        private void DrawPlayModeControls()
        {
            TraumaCameraShake shake = (TraumaCameraShake)target;
            float trauma = shake.GetTrauma();

            EditorGUILayout.LabelField("Live Trauma", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Current Trauma: {trauma:0.00}");

            Rect barRect = EditorGUILayout.GetControlRect(false, 18f);
            GUI.Box(barRect, GUIContent.none);
            Rect fillRect = new Rect(barRect.x, barRect.y, barRect.width * trauma, barRect.height);
            EditorGUI.DrawRect(fillRect, new Color(1f, 0.3f, 0.2f, 0.85f));

            EditorGUILayout.Space();

            if (GUILayout.Button("Add 0.3 Trauma"))
            {
                shake.AddTrauma(0.3f);
            }

            if (GUILayout.Button("Add 0.5 Trauma"))
            {
                shake.AddTrauma(0.5f);
            }

            if (GUILayout.Button("Add Full Trauma (1.0)"))
            {
                shake.AddTrauma(1.0f);
            }

            if (GUILayout.Button("Reset Trauma"))
            {
                shake.SetTrauma(0f);
            }
        }
    }
}
#endif

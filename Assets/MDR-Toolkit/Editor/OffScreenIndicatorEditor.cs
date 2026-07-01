// ============================================================
// MDR Unity Toolkit - OffScreenIndicator
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MDR.Toolkit.UI
{
    /// <summary>
    /// Custom Inspector for <see cref="OffScreenIndicator"/>. Shows registered targets and
    /// their on/off-screen status during Play Mode, and setup instructions otherwise.
    /// </summary>
    [CustomEditor(typeof(OffScreenIndicator))]
    public class OffScreenIndicatorEditor : Editor
    {
        /// <summary>
        /// Draws the default Inspector plus a play-mode target list, or setup instructions in edit mode.
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            OffScreenIndicator indicator = (OffScreenIndicator)target;

            if (Application.isPlaying)
            {
                DrawRuntimeTargetList(indicator);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Setup:\n" +
                    "1. Attach this component to a Canvas set to Screen Space - Overlay.\n" +
                    "2. Assign an indicator prefab (RectTransform + Image, arrow pointing up).\n" +
                    "3. Add OffScreenIndicatorTarget to any world GameObject you want tracked.\n" +
                    "4. Enter Play Mode to see registered targets and their on/off-screen status here.",
                    MessageType.Info);
            }
        }

        /// <summary>
        /// Forces continuous repaint during Play Mode so target statuses update live.
        /// </summary>
        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

        private void DrawRuntimeTargetList(OffScreenIndicator indicator)
        {
            IReadOnlyList<OffScreenIndicatorTarget> targets = indicator.GetRegisteredTargets();

            EditorGUILayout.LabelField("Registered Targets", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Total: {targets.Count}");

            EditorGUILayout.Space();

            for (int i = 0; i < targets.Count; i++)
            {
                OffScreenIndicatorTarget entryTarget = targets[i];
                if (entryTarget == null)
                {
                    continue;
                }

                EditorGUILayout.BeginHorizontal();

                bool onScreen = indicator.IsTargetOnScreen(entryTarget);
                EditorGUILayout.LabelField(entryTarget.name, onScreen ? "On Screen" : "Off Screen");

                if (GUILayout.Button("Select Target", GUILayout.Width(110f)))
                {
                    EditorGUIUtility.PingObject(entryTarget.gameObject);
                    Selection.activeGameObject = entryTarget.gameObject;
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
#endif

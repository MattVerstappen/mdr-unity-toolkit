// ============================================================
// MDR Unity Toolkit - SaveSystem
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MDR.Toolkit.Save
{
    /// <summary>
    /// Editor window for inspecting, deleting, and browsing save files written by <see cref="SaveSystem"/>.
    /// </summary>
    public class SaveSystemEditor : EditorWindow
    {
        private Vector2 scrollPosition;

        /// <summary>
        /// Opens the Save System Inspector window.
        /// </summary>
        [MenuItem("Tools/MDR Toolkit/Save System Inspector")]
        public static void ShowWindow()
        {
            SaveSystemEditor window = GetWindow<SaveSystemEditor>("Save System");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Current Schema Version:", SaveSystem.CURRENT_SCHEMA_VERSION.ToString());
            EditorGUILayout.Space();

            if (GUILayout.Button("Open Save Folder"))
            {
                Application.OpenURL(SaveSystem.SAVE_FOLDER);
            }

            if (GUILayout.Button("Force Save (All Slots)"))
            {
                Debug.Log("[SaveSystemEditor] Force Save only works in Play Mode, since it reads live state from ISaveable objects in the running scene.");
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Save Slots", EditorStyles.boldLabel);

            List<SaveSlot> slots = SaveSystem.GetAllSlots();

            if (slots.Count == 0)
            {
                EditorGUILayout.HelpBox("No save files found.", MessageType.Info);
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < slots.Count; i++)
            {
                SaveSlot slot = slots[i];

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Slot:", slot.slotName);
                EditorGUILayout.LabelField("Schema Version:", slot.schemaVersion.ToString());
                EditorGUILayout.LabelField("Last Saved:", slot.lastSavedAt);

                if (GUILayout.Button("Delete"))
                {
                    SaveSystem.Delete(slot.slotName);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndScrollView();
                    GUIUtility.ExitGUI();
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
#endif

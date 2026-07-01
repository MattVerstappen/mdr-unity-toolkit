// ============================================================
// MDR Unity Toolkit - SaveSystem
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

using System;
using System.IO;
using UnityEngine;

namespace MDR.Toolkit.Save
{
    /// <summary>
    /// Lightweight metadata describing a save file, without fully deserializing its payload
    /// into live game state.
    /// </summary>
    [Serializable]
    public class SaveSlot
    {
        /// <summary>The reserved <see cref="SaveData"/> key used to store total playtime, if tracked.</summary>
        public const string TotalPlaytimeKey = "totalPlaytimeSeconds";

        /// <summary>The name of the save slot (derived from the file name).</summary>
        public string slotName;

        /// <summary>The full path to the save file on disk.</summary>
        public string filePath;

        /// <summary>The schema version the save file was written with.</summary>
        public int schemaVersion;

        /// <summary>The last time this save file was written, as an ISO 8601 datetime string.</summary>
        public string lastSavedAt;

        /// <summary>Total playtime recorded in this save, in seconds, if tracked.</summary>
        public float totalPlaytimeSeconds;

        /// <summary>
        /// Reads slot metadata from a save file on disk, without distributing the data to
        /// any registered <see cref="ISaveable"/> objects.
        /// </summary>
        /// <param name="filePath">The full path to the save file.</param>
        /// <returns>A <see cref="SaveSlot"/> describing the file's metadata.</returns>
        public static SaveSlot FromFile(string filePath)
        {
            SaveSlot slot = new SaveSlot
            {
                slotName = Path.GetFileNameWithoutExtension(filePath),
                filePath = filePath,
                schemaVersion = 0,
                lastSavedAt = string.Empty,
                totalPlaytimeSeconds = 0f
            };

            if (!File.Exists(filePath))
            {
                return slot;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                if (data != null)
                {
                    slot.schemaVersion = data.schemaVersion;
                    slot.totalPlaytimeSeconds = data.Get(TotalPlaytimeKey, 0f);
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[SaveSlot] Failed to read metadata from '{filePath}': {exception.Message}");
            }

            slot.lastSavedAt = File.GetLastWriteTimeUtc(filePath).ToString("o");
            return slot;
        }
    }
}

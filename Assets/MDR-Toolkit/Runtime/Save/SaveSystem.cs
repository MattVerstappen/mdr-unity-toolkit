// ============================================================
// MDR Unity Toolkit - SaveSystem
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MDR.Toolkit.Save
{
    /// <summary>
    /// Global, static entry point for saving and loading game state. Collects data from all
    /// registered <see cref="ISaveable"/> objects, writes it to disk as versioned JSON, and
    /// migrates old saves forward via <see cref="SaveMigrator"/> when loading.
    /// </summary>
    public static class SaveSystem
    {
        /// <summary>
        /// The current save data schema version. Increment this whenever the meaning or
        /// structure of saved data changes, and add a corresponding migration step to
        /// <see cref="SaveMigrator"/>.
        /// </summary>
        public const int CURRENT_SCHEMA_VERSION = 1;

        /// <summary>The folder all save files are written to and read from.</summary>
        public static readonly string SAVE_FOLDER = Path.Combine(Application.persistentDataPath, "MDRSaves") + Path.DirectorySeparatorChar;

        private static readonly List<ISaveable> saveables = new List<ISaveable>();

        /// <summary>
        /// Collects data from all registered <see cref="ISaveable"/> objects and writes them
        /// to a save file for the given slot.
        /// </summary>
        /// <param name="slotName">The name of the slot to save to.</param>
        public static void Save(string slotName)
        {
            EnsureSaveFolder();

            SaveData rootData = new SaveData { schemaVersion = CURRENT_SCHEMA_VERSION };

            for (int i = 0; i < saveables.Count; i++)
            {
                ISaveable saveable = saveables[i];
                if (saveable == null)
                {
                    continue;
                }

                SaveData objectData = new SaveData { schemaVersion = CURRENT_SCHEMA_VERSION };
                saveable.OnSave(objectData);
                rootData.Set(saveable.GetSaveKey(), objectData);
            }

            string json = JsonUtility.ToJson(rootData);
            File.WriteAllText(GetFilePath(slotName), json);
        }

        /// <summary>
        /// Reads the save file for the given slot, migrates it to the current schema version
        /// if needed, then distributes the data to all registered <see cref="ISaveable"/> objects.
        /// </summary>
        /// <param name="slotName">The name of the slot to load.</param>
        /// <returns>True if the file existed and was loaded successfully.</returns>
        public static bool Load(string slotName)
        {
            string filePath = GetFilePath(slotName);
            if (!File.Exists(filePath))
            {
                return false;
            }

            string json = File.ReadAllText(filePath);
            SaveData rootData = JsonUtility.FromJson<SaveData>(json);
            if (rootData == null)
            {
                return false;
            }

            if (rootData.schemaVersion < CURRENT_SCHEMA_VERSION)
            {
                rootData = SaveMigrator.Migrate(rootData);
            }

            for (int i = 0; i < saveables.Count; i++)
            {
                ISaveable saveable = saveables[i];
                if (saveable == null)
                {
                    continue;
                }

                SaveData objectData = rootData.Get(saveable.GetSaveKey(), new SaveData { schemaVersion = rootData.schemaVersion });
                saveable.OnLoad(objectData);
            }

            return true;
        }

        /// <summary>
        /// Deletes the save file for the given slot, if it exists.
        /// </summary>
        /// <param name="slotName">The name of the slot to delete.</param>
        public static void Delete(string slotName)
        {
            string filePath = GetFilePath(slotName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Checks whether a save file exists for the given slot.
        /// </summary>
        /// <param name="slotName">The name of the slot to check.</param>
        /// <returns>True if a save file exists for the slot.</returns>
        public static bool SlotExists(string slotName)
        {
            return File.Exists(GetFilePath(slotName));
        }

        /// <summary>
        /// Returns metadata for every save file found in the save folder.
        /// </summary>
        /// <returns>A list of <see cref="SaveSlot"/> metadata, one per save file.</returns>
        public static List<SaveSlot> GetAllSlots()
        {
            List<SaveSlot> slots = new List<SaveSlot>();

            if (!Directory.Exists(SAVE_FOLDER))
            {
                return slots;
            }

            string[] files = Directory.GetFiles(SAVE_FOLDER, "*.save");
            for (int i = 0; i < files.Length; i++)
            {
                slots.Add(SaveSlot.FromFile(files[i]));
            }

            return slots;
        }

        /// <summary>
        /// Registers an <see cref="ISaveable"/> so it is included in future saves and loads.
        /// Useful for dynamically spawned objects that cannot be found via a scene scan.
        /// </summary>
        /// <param name="saveable">The object to register.</param>
        public static void RegisterSaveable(ISaveable saveable)
        {
            if (saveable != null && !saveables.Contains(saveable))
            {
                saveables.Add(saveable);
            }
        }

        /// <summary>
        /// Unregisters a previously registered <see cref="ISaveable"/>.
        /// </summary>
        /// <param name="saveable">The object to unregister.</param>
        public static void UnregisterSaveable(ISaveable saveable)
        {
            saveables.Remove(saveable);
        }

        private static void EnsureSaveFolder()
        {
            if (!Directory.Exists(SAVE_FOLDER))
            {
                Directory.CreateDirectory(SAVE_FOLDER);
            }
        }

        private static string GetFilePath(string slotName)
        {
            return Path.Combine(SAVE_FOLDER, slotName + ".save");
        }
    }
}

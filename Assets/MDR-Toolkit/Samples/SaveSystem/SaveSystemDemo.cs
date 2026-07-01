// ============================================================
// MDR Unity Toolkit - SaveSystem
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

using UnityEngine;

namespace MDR.Toolkit.Save
{
    /// <summary>
    /// Minimal <see cref="ISaveable"/> example demonstrating registration, saving, and
    /// loading of a single tracked value.
    /// </summary>
    public class SaveSystemDemo : MonoBehaviour, ISaveable
    {
        private int demoCounter;

        private void Start()
        {
            SaveSystem.RegisterSaveable(this);
        }

        private void OnDestroy()
        {
            SaveSystem.UnregisterSaveable(this);
        }

        /// <summary>
        /// Returns the unique save key for this demo object.
        /// </summary>
        /// <returns>The string "demo".</returns>
        public string GetSaveKey()
        {
            return "demo";
        }

        /// <summary>
        /// Writes the demo counter into the save data.
        /// </summary>
        /// <param name="data">The save data scoped to this object.</param>
        public void OnSave(SaveData data)
        {
            data.Set("demoCounter", demoCounter);
        }

        /// <summary>
        /// Reads the demo counter back from the save data.
        /// </summary>
        /// <param name="data">The save data scoped to this object.</param>
        public void OnLoad(SaveData data)
        {
            demoCounter = data.Get("demoCounter", demoCounter);
        }

        /// <summary>
        /// Increments the demo counter and saves it to "slot1".
        /// </summary>
        [ContextMenu("Increment and Save")]
        public void IncrementAndSave()
        {
            demoCounter++;
            SaveSystem.Save("slot1");
            Debug.Log($"[SaveSystemDemo] demoCounter incremented to {demoCounter} and saved.");
        }

        /// <summary>
        /// Loads "slot1" and logs the resulting demo counter value.
        /// </summary>
        [ContextMenu("Load")]
        public void Load()
        {
            SaveSystem.Load("slot1");
            Debug.Log($"[SaveSystemDemo] demoCounter loaded as {demoCounter}.");
        }
    }
}

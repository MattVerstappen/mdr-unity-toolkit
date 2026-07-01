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
    /// Sample <see cref="ISaveable"/> implementation tracking basic player state, for the
    /// SaveSystem demo.
    /// </summary>
    public class PlayerSaveData : MonoBehaviour, ISaveable
    {
        /// <summary>The player's current level.</summary>
        public int level = 1;

        /// <summary>The player's current health.</summary>
        public float health = 100f;

        /// <summary>The player's last known world position.</summary>
        public Vector3 lastPosition;

        /// <summary>The player's display name.</summary>
        public string playerName = "Player";

        private void Awake()
        {
            SaveSystem.RegisterSaveable(this);
        }

        private void OnDestroy()
        {
            SaveSystem.UnregisterSaveable(this);
        }

        /// <summary>
        /// Returns the unique save key for the player's data.
        /// </summary>
        /// <returns>The string "player".</returns>
        public string GetSaveKey()
        {
            return "player";
        }

        /// <summary>
        /// Writes the player's current state into the save data.
        /// </summary>
        /// <param name="data">The save data scoped to this object.</param>
        public void OnSave(SaveData data)
        {
            data.Set("level", level);
            data.Set("health", health);
            data.Set("lastPosition", lastPosition);
            data.Set("playerName", playerName);
        }

        /// <summary>
        /// Reads the player's state back from the save data.
        /// </summary>
        /// <param name="data">The save data scoped to this object.</param>
        public void OnLoad(SaveData data)
        {
            level = data.Get("level", level);
            health = data.Get("health", health);
            lastPosition = data.Get("lastPosition", lastPosition);
            playerName = data.Get("playerName", playerName);
        }

        /// <summary>
        /// Saves the current game state to "slot1".
        /// </summary>
        [ContextMenu("Save Now")]
        public void SaveNow()
        {
            SaveSystem.Save("slot1");
        }

        /// <summary>
        /// Loads game state from "slot1".
        /// </summary>
        [ContextMenu("Load Now")]
        public void LoadNow()
        {
            SaveSystem.Load("slot1");
        }
    }
}

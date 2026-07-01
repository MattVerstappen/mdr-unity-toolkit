// ============================================================
// MDR Unity Toolkit - SaveSystem
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

namespace MDR.Toolkit.Save
{
    /// <summary>
    /// Implement on any MonoBehaviour to opt into <see cref="SaveSystem"/> saving and loading.
    /// </summary>
    public interface ISaveable
    {
        /// <summary>
        /// Returns a unique string key identifying this object's data within a save file.
        /// </summary>
        /// <returns>A unique save key, e.g. "player" or "inventory".</returns>
        string GetSaveKey();

        /// <summary>
        /// Called by <see cref="SaveSystem"/> when saving. Write this object's state into <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The save data scoped to this object.</param>
        void OnSave(SaveData data);

        /// <summary>
        /// Called by <see cref="SaveSystem"/> when loading. Read this object's state from <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The save data scoped to this object.</param>
        void OnLoad(SaveData data);
    }
}

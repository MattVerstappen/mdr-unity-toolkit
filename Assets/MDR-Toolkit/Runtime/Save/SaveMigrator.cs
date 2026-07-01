// ============================================================
// MDR Unity Toolkit - SaveSystem
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

using System;
using UnityEngine;

namespace MDR.Toolkit.Save
{
    /// <summary>
    /// Upgrades save data from an older schema version to <see cref="SaveSystem.CURRENT_SCHEMA_VERSION"/>.
    /// When you increment <see cref="SaveSystem.CURRENT_SCHEMA_VERSION"/>, add a new
    /// private MigrateVXToVY step below and a matching case in <see cref="Migrate"/>.
    /// </summary>
    public static class SaveMigrator
    {
        /// <summary>
        /// Steps <paramref name="data"/> forward one version at a time until it reaches
        /// <see cref="SaveSystem.CURRENT_SCHEMA_VERSION"/>. Migration always runs before
        /// data is distributed to any registered <see cref="ISaveable"/>.
        /// </summary>
        /// <param name="data">The save data to migrate, at any prior schema version.</param>
        /// <returns>The same <see cref="SaveData"/> instance, upgraded in place.</returns>
        public static SaveData Migrate(SaveData data)
        {
            while (data.schemaVersion < SaveSystem.CURRENT_SCHEMA_VERSION)
            {
                switch (data.schemaVersion)
                {
                    case 0:
                        MigrateV0ToV1(data);
                        break;

                    // Add new cases here as CURRENT_SCHEMA_VERSION increases, e.g.:
                    // case 1:
                    //     MigrateV1ToV2(data);
                    //     break;

                    default:
                        Debug.LogWarning($"[SaveMigrator] No migration step defined for schema version {data.schemaVersion}. Stopping migration.");
                        return data;
                }
            }

            return data;
        }

        /// <summary>
        /// Example migration step: adds a "firstPlayDate" key if it is missing, then
        /// advances the schema version to 1.
        /// </summary>
        /// <param name="data">The version-0 save data to migrate.</param>
        private static void MigrateV0ToV1(SaveData data)
        {
            if (!data.Has("firstPlayDate"))
            {
                data.Set("firstPlayDate", DateTime.UtcNow.ToString("o"));
            }

            data.schemaVersion = 1;
        }
    }
}

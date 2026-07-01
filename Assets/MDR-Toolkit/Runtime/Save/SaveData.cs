// ============================================================
// MDR Unity Toolkit - SaveSystem
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MDR.Toolkit.Save
{
    /// <summary>
    /// Versioned container for a set of key/value entries, each serialized independently
    /// to JSON via <see cref="JsonUtility"/>. Since JsonUtility cannot serialize
    /// <see cref="Dictionary{TKey, TValue}"/> directly, entries are stored as two parallel
    /// lists (<c>keys</c> and <c>values</c>) and exposed through a lazily-built dictionary.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        /// <summary>The schema version this data was saved with.</summary>
        public int schemaVersion;

        [SerializeField]
        private List<string> keys = new List<string>();

        [SerializeField]
        private List<string> values = new List<string>();

        [NonSerialized]
        private Dictionary<string, string> payload;

        private Dictionary<string, string> Payload
        {
            get
            {
                if (payload == null)
                {
                    RebuildPayloadFromLists();
                }

                return payload;
            }
        }

        /// <summary>
        /// Serializes <paramref name="value"/> to JSON and stores it under <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="T">The type of value to store.</typeparam>
        /// <param name="key">The key to store the value under.</param>
        /// <param name="value">The value to serialize and store.</param>
        public void Set<T>(string key, T value)
        {
            ValueWrapper<T> wrapper = new ValueWrapper<T> { value = value };
            Payload[key] = JsonUtility.ToJson(wrapper);
            SyncListsFromPayload();
        }

        /// <summary>
        /// Deserializes the value stored under <paramref name="key"/>, or returns
        /// <paramref name="defaultValue"/> if the key is missing.
        /// </summary>
        /// <typeparam name="T">The type of value to retrieve.</typeparam>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">The value to return if the key is not present.</param>
        /// <returns>The deserialized value, or <paramref name="defaultValue"/> if missing.</returns>
        public T Get<T>(string key, T defaultValue = default)
        {
            if (!Payload.TryGetValue(key, out string json))
            {
                return defaultValue;
            }

            ValueWrapper<T> wrapper = JsonUtility.FromJson<ValueWrapper<T>>(json);
            return wrapper.value;
        }

        /// <summary>
        /// Returns whether a value is stored under <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key exists.</returns>
        public bool Has(string key)
        {
            return Payload.ContainsKey(key);
        }

        /// <summary>
        /// Removes the value stored under <paramref name="key"/>, if present.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        public void Remove(string key)
        {
            if (Payload.Remove(key))
            {
                SyncListsFromPayload();
            }
        }

        private void RebuildPayloadFromLists()
        {
            payload = new Dictionary<string, string>();
            int count = Mathf.Min(keys.Count, values.Count);
            for (int i = 0; i < count; i++)
            {
                payload[keys[i]] = values[i];
            }
        }

        private void SyncListsFromPayload()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<string, string> pair in Payload)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        [Serializable]
        private struct ValueWrapper<T>
        {
            public T value;
        }
    }
}

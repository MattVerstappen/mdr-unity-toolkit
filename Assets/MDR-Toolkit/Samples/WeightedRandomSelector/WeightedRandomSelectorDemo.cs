// ============================================================
// MDR Unity Toolkit - WeightedRandomSelector
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

using System.Collections.Generic;
using UnityEngine;

namespace MDR.Toolkit.Utility
{
    /// <summary>
    /// Demonstrates configuring a <see cref="WeightedRandomSelector{T}"/> of strings
    /// directly in the Unity Inspector and picking a random entry at runtime.
    /// </summary>
    public class WeightedRandomSelectorDemo : MonoBehaviour
    {
        /// <summary>The weighted pool of strings, configurable in the Inspector.</summary>
        [SerializeField]
        private List<WeightedEntry<string>> entries = new List<WeightedEntry<string>>();

        private WeightedRandomSelector<string> selector;

        private void Start()
        {
            BuildSelector();
        }

        private void BuildSelector()
        {
            selector = new WeightedRandomSelector<string>();
            for (int i = 0; i < entries.Count; i++)
            {
                selector.Add(entries[i].Value, entries[i].Weight);
            }
        }

        /// <summary>
        /// Picks a random entry from the configured pool and logs the result.
        /// </summary>
        [ContextMenu("Pick Random")]
        public void PickRandom()
        {
            if (selector == null)
            {
                BuildSelector();
            }

            string result = selector.Pick();
            Debug.Log($"[WeightedRandomSelectorDemo] Picked: {result}");
        }
    }
}

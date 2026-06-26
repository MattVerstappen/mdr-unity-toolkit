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
    /// Demonstrates <see cref="WeightedRandomSelector{T}"/> by exposing a list of
    /// weighted string entries that can be configured directly in the Unity Inspector.
    /// Attach this component to any GameObject, fill in the entries, then press Play
    /// or use the context-menu / custom-editor buttons to pick a random value.
    /// </summary>
    public class WeightedRandomSelectorDemo : MonoBehaviour
    {
        [Tooltip("The weighted string pool. Weights are relative positive values.")]
        [SerializeField]
        private List<WeightedEntry<string>> _entries = new List<WeightedEntry<string>>
        {
            new WeightedEntry<string>("Common", 60f),
            new WeightedEntry<string>("Rare", 30f),
            new WeightedEntry<string>("Legendary", 10f),
        };

        private WeightedRandomSelector<string> _selector;

        /// <summary>
        /// The list of entries configured in the Inspector. Exposed so the custom
        /// editor can draw weight bars without mutating the underlying field.
        /// </summary>
        public IReadOnlyList<WeightedEntry<string>> Entries => _entries;

        private void Start()
        {
            BuildSelector();
        }

        /// <summary>
        /// Builds (or rebuilds) the runtime selector from the Inspector entries.
        /// </summary>
        public void BuildSelector()
        {
            _selector = new WeightedRandomSelector<string>(_entries);
        }

        /// <summary>
        /// Picks a random entry from the configured pool and logs it to the console.
        /// Safe to call from edit mode; the selector is rebuilt on demand if needed.
        /// </summary>
        [ContextMenu("Pick Random")]
        public void PickRandom()
        {
            if (_entries == null || _entries.Count == 0)
            {
                Debug.LogWarning($"{nameof(WeightedRandomSelectorDemo)}: No entries configured to pick from.", this);
                return;
            }

            // Rebuild on demand so the method works in edit mode and after Inspector edits.
            BuildSelector();

            string result = _selector.Pick();
            Debug.Log($"{nameof(WeightedRandomSelectorDemo)} picked: {result}", this);
        }
    }
}

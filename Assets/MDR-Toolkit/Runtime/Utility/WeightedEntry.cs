// ============================================================
// MDR Unity Toolkit - WeightedRandomSelector
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

using System;
using UnityEngine;

namespace MDR.Toolkit.Utility
{
    /// <summary>
    /// A simple serializable container that pairs a value of type <typeparamref name="T"/>
    /// with a float weight. Used by <see cref="WeightedRandomSelector{T}"/> to describe the
    /// relative likelihood of each value being picked.
    /// </summary>
    /// <typeparam name="T">The type of the stored value.</typeparam>
    [Serializable]
    public class WeightedEntry<T>
    {
        /// <summary>The value associated with this entry.</summary>
        [SerializeField]
        public T Value;

        /// <summary>
        /// The relative weight of this entry. Must be greater than zero.
        /// Weights are relative, so they do not need to sum to any particular total.
        /// </summary>
        [SerializeField]
        public float Weight;

        /// <summary>
        /// Creates a new weighted entry.
        /// </summary>
        /// <param name="value">The value to store.</param>
        /// <param name="weight">The relative weight of the value. Must be positive.</param>
        public WeightedEntry(T value, float weight)
        {
            Value = value;
            Weight = weight;
        }
    }
}

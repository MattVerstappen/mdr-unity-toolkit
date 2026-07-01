// ============================================================
// MDR Unity Toolkit - WeightedRandomSelector
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

using System;

namespace MDR.Toolkit.Utility
{
    /// <summary>
    /// Serializable data container pairing a value of type <typeparamref name="T"/>
    /// with a positive float weight, used by <see cref="WeightedRandomSelector{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of value stored in this entry.</typeparam>
    [Serializable]
    public class WeightedEntry<T>
    {
        /// <summary>The value associated with this entry.</summary>
        public T Value;

        /// <summary>The relative weight of this entry. Must be greater than zero.</summary>
        public float Weight;

        /// <summary>
        /// Creates a new weighted entry.
        /// </summary>
        /// <param name="value">The value to store.</param>
        /// <param name="weight">The relative weight of the value. Must be greater than zero.</param>
        public WeightedEntry(T value, float weight)
        {
            Value = value;
            Weight = weight;
        }
    }
}

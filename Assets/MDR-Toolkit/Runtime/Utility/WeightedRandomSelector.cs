// ============================================================
// MDR Unity Toolkit - WeightedRandomSelector
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MDR.Toolkit.Utility
{
    /// <summary>
    /// A generic weighted random selection system. Stores a pool of values paired with
    /// positive float weights and picks a random value with probability proportional to
    /// its weight. Weights do not need to sum to any particular total - any positive
    /// float values work.
    /// </summary>
    /// <typeparam name="T">The type of value to select from.</typeparam>
    public class WeightedRandomSelector<T>
    {
        private readonly List<WeightedEntry<T>> entries = new List<WeightedEntry<T>>();

        /// <summary>
        /// Adds a new entry to the selection pool.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <param name="weight">The relative weight of the value. Must be greater than zero.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="weight"/> is zero or negative.</exception>
        public void Add(T value, float weight)
        {
            if (weight <= 0f)
            {
                throw new ArgumentException($"Weight must be greater than zero. Received: {weight}", nameof(weight));
            }

            entries.Add(new WeightedEntry<T>(value, weight));
        }

        /// <summary>
        /// Removes all entries whose value equals <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to remove from the pool.</param>
        public void Remove(T value)
        {
            entries.RemoveAll(entry => EqualityComparer<T>.Default.Equals(entry.Value, value));
        }

        /// <summary>
        /// Removes all entries from the selection pool.
        /// </summary>
        public void Clear()
        {
            entries.Clear();
        }

        /// <summary>
        /// Returns the sum of the weights of all entries currently in the pool.
        /// </summary>
        /// <returns>The total combined weight.</returns>
        public float GetTotalWeight()
        {
            float total = 0f;
            for (int i = 0; i < entries.Count; i++)
            {
                total += entries[i].Weight;
            }

            return total;
        }

        /// <summary>
        /// Returns a copy of the current entries in the selection pool.
        /// </summary>
        /// <returns>A new list containing the same <see cref="WeightedEntry{T}"/> references.</returns>
        public List<WeightedEntry<T>> GetEntries()
        {
            return new List<WeightedEntry<T>>(entries);
        }

        /// <summary>
        /// Picks a random value from the pool with probability proportional to each entry's weight.
        /// </summary>
        /// <returns>The selected value.</returns>
        /// <exception cref="ArgumentException">Thrown if the pool is empty or contains a zero/negative weight.</exception>
        public T Pick()
        {
            if (entries.Count == 0)
            {
                throw new ArgumentException("Cannot pick from an empty WeightedRandomSelector.");
            }

            float totalWeight = GetTotalWeight();
            if (totalWeight <= 0f)
            {
                throw new ArgumentException("Total weight must be greater than zero.");
            }

            float roll = UnityEngine.Random.Range(0f, totalWeight);
            float accumulated = 0f;

            for (int i = 0; i < entries.Count; i++)
            {
                float weight = entries[i].Weight;
                if (weight <= 0f)
                {
                    throw new ArgumentException($"Entry weight must be greater than zero. Received: {weight}");
                }

                accumulated += weight;
                if (roll < accumulated)
                {
                    return entries[i].Value;
                }
            }

            return entries[entries.Count - 1].Value;
        }

        /// <summary>
        /// Picks a random value from the pool with probability proportional to each entry's weight,
        /// excluding any entries whose value equals <paramref name="exclude"/>. Useful for
        /// anti-repeat selection, e.g. avoiding the same result twice in a row.
        /// </summary>
        /// <param name="exclude">The value to exclude from consideration.</param>
        /// <returns>The selected value.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the pool is empty, contains a zero/negative weight, or every entry equals
        /// <paramref name="exclude"/>.
        /// </exception>
        public T PickExcluding(T exclude)
        {
            if (entries.Count == 0)
            {
                throw new ArgumentException("Cannot pick from an empty WeightedRandomSelector.");
            }

            List<WeightedEntry<T>> candidates = new List<WeightedEntry<T>>();
            for (int i = 0; i < entries.Count; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(entries[i].Value, exclude))
                {
                    candidates.Add(entries[i]);
                }
            }

            if (candidates.Count == 0)
            {
                throw new ArgumentException("No entries remain after excluding the given value.");
            }

            float totalWeight = 0f;
            for (int i = 0; i < candidates.Count; i++)
            {
                totalWeight += candidates[i].Weight;
            }

            if (totalWeight <= 0f)
            {
                throw new ArgumentException("Total weight must be greater than zero.");
            }

            float roll = UnityEngine.Random.Range(0f, totalWeight);
            float accumulated = 0f;

            for (int i = 0; i < candidates.Count; i++)
            {
                float weight = candidates[i].Weight;
                if (weight <= 0f)
                {
                    throw new ArgumentException($"Entry weight must be greater than zero. Received: {weight}");
                }

                accumulated += weight;
                if (roll < accumulated)
                {
                    return candidates[i].Value;
                }
            }

            return candidates[candidates.Count - 1].Value;
        }
    }
}

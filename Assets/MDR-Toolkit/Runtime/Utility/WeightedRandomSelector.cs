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
    /// A fully generic weighted random selection system. Stores a collection of
    /// <see cref="WeightedEntry{T}"/> objects and picks values with a probability
    /// proportional to each entry's weight.
    ///
    /// Weights are relative positive floats and do not need to sum to any particular
    /// total (1, 100, or otherwise). Works with any type: strings, enums, prefabs,
    /// ScriptableObjects, or custom classes.
    /// </summary>
    /// <typeparam name="T">The type of value being selected.</typeparam>
    public class WeightedRandomSelector<T>
    {
        private readonly List<WeightedEntry<T>> _entries = new List<WeightedEntry<T>>();

        /// <summary>The number of entries currently stored.</summary>
        public int Count => _entries.Count;

        /// <summary>
        /// Creates an empty selector. Add entries with <see cref="Add"/>.
        /// </summary>
        public WeightedRandomSelector()
        {
        }

        /// <summary>
        /// Creates a selector pre-populated from an existing collection of entries.
        /// Each entry is copied so the source collection is not retained by reference.
        /// </summary>
        /// <param name="entries">The entries to seed the selector with.</param>
        public WeightedRandomSelector(IEnumerable<WeightedEntry<T>> entries)
        {
            if (entries == null)
            {
                return;
            }

            foreach (WeightedEntry<T> entry in entries)
            {
                if (entry == null)
                {
                    continue;
                }

                Add(entry.Value, entry.Weight);
            }
        }

        /// <summary>
        /// Adds an entry at runtime.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <param name="weight">The relative weight. Must be greater than zero.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="weight"/> is zero or negative.</exception>
        public void Add(T value, float weight)
        {
            if (weight <= 0f)
            {
                throw new ArgumentException(
                    $"Weight must be greater than zero (got {weight}).", nameof(weight));
            }

            _entries.Add(new WeightedEntry<T>(value, weight));
        }

        /// <summary>
        /// Removes the first entry whose value equals <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        /// <returns><c>true</c> if an entry was removed; otherwise <c>false</c>.</returns>
        public bool Remove(T value)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            for (int i = 0; i < _entries.Count; i++)
            {
                if (comparer.Equals(_entries[i].Value, value))
                {
                    _entries.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all entries.
        /// </summary>
        public void Clear()
        {
            _entries.Clear();
        }

        /// <summary>
        /// Returns the sum of all weights in the selector.
        /// </summary>
        /// <returns>The total accumulated weight.</returns>
        public float GetTotalWeight()
        {
            float total = 0f;

            for (int i = 0; i < _entries.Count; i++)
            {
                total += _entries[i].Weight;
            }

            return total;
        }

        /// <summary>
        /// Returns a shallow copy of the entries list so callers cannot mutate the
        /// selector's internal state directly.
        /// </summary>
        /// <returns>A new list containing the current entries.</returns>
        public List<WeightedEntry<T>> GetEntries()
        {
            return new List<WeightedEntry<T>>(_entries);
        }

        /// <summary>
        /// Picks a random value weighted by each entry's weight.
        /// </summary>
        /// <returns>The selected value.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the selector is empty, or if any entry has a zero or negative weight.
        /// </exception>
        public T Pick()
        {
            if (_entries.Count == 0)
            {
                throw new ArgumentException("Cannot Pick() from an empty WeightedRandomSelector.");
            }

            float totalWeight = GetTotalWeight();
            ValidateWeights(totalWeight);

            float roll = UnityEngine.Random.Range(0f, totalWeight);
            float cumulative = 0f;

            for (int i = 0; i < _entries.Count; i++)
            {
                cumulative += _entries[i].Weight;
                if (roll < cumulative)
                {
                    return _entries[i].Value;
                }
            }

            // Fallback for floating-point edge cases where roll == totalWeight.
            return _entries[_entries.Count - 1].Value;
        }

        /// <summary>
        /// Picks a weighted random value that is not equal to <paramref name="exclude"/>.
        /// Useful for anti-repeat behaviour where the same result should not appear twice
        /// in a row.
        /// </summary>
        /// <param name="exclude">The value to exclude from the selection.</param>
        /// <returns>The selected value, guaranteed not to equal <paramref name="exclude"/>.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the selector is empty, if no entries remain after excluding
        /// <paramref name="exclude"/>, or if any entry has a zero or negative weight.
        /// </exception>
        public T PickExcluding(T exclude)
        {
            if (_entries.Count == 0)
            {
                throw new ArgumentException("Cannot PickExcluding() from an empty WeightedRandomSelector.");
            }

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            // Accumulate the weight of all entries that are not excluded.
            float totalWeight = 0f;
            for (int i = 0; i < _entries.Count; i++)
            {
                float weight = _entries[i].Weight;
                if (weight <= 0f)
                {
                    throw new ArgumentException(
                        $"Entry '{_entries[i].Value}' has an invalid weight of {weight}. Weights must be greater than zero.");
                }

                if (!comparer.Equals(_entries[i].Value, exclude))
                {
                    totalWeight += weight;
                }
            }

            if (totalWeight <= 0f)
            {
                throw new ArgumentException(
                    "No entries remain after excluding the supplied value.");
            }

            float roll = UnityEngine.Random.Range(0f, totalWeight);
            float cumulative = 0f;

            for (int i = 0; i < _entries.Count; i++)
            {
                if (comparer.Equals(_entries[i].Value, exclude))
                {
                    continue;
                }

                cumulative += _entries[i].Weight;
                if (roll < cumulative)
                {
                    return _entries[i].Value;
                }
            }

            // Fallback for floating-point edge cases: return the last non-excluded entry.
            for (int i = _entries.Count - 1; i >= 0; i--)
            {
                if (!comparer.Equals(_entries[i].Value, exclude))
                {
                    return _entries[i].Value;
                }
            }

            // Unreachable: totalWeight > 0 guarantees at least one non-excluded entry.
            throw new ArgumentException("No entries remain after excluding the supplied value.");
        }

        /// <summary>
        /// Validates that the selector is in a pickable state.
        /// </summary>
        /// <param name="totalWeight">The pre-computed total weight to validate against.</param>
        /// <exception cref="ArgumentException">Thrown if any weight is zero or negative.</exception>
        private void ValidateWeights(float totalWeight)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].Weight <= 0f)
                {
                    throw new ArgumentException(
                        $"Entry '{_entries[i].Value}' has an invalid weight of {_entries[i].Weight}. Weights must be greater than zero.");
                }
            }

            if (totalWeight <= 0f)
            {
                throw new ArgumentException("Total weight must be greater than zero.");
            }
        }
    }
}

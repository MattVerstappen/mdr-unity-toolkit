// ============================================================
// MDR Unity Toolkit - AudioRandomizer
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

using System.Collections.Generic;
using UnityEngine;

namespace MDR.Toolkit.Audio
{
    /// <summary>
    /// Plays a randomised clip from a weighted pool through an <see cref="AudioSource"/>,
    /// blocking recently played clips to eliminate the "machine gun" repeat effect and
    /// applying randomised pitch and volume on every play.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioRandomizer : MonoBehaviour
    {
        /// <summary>The pool of clips to randomise from, each with its own weight.</summary>
        [SerializeField]
        private List<AudioEntry> entries = new List<AudioEntry>();

        /// <summary>How many of the most recently played clips to block from being re-picked.</summary>
        [SerializeField]
        private int antiRepeatCount = 2;

        /// <summary>Minimum pitch multiplier applied before each play.</summary>
        [SerializeField]
        private float pitchMin = 0.95f;

        /// <summary>Maximum pitch multiplier applied before each play.</summary>
        [SerializeField]
        private float pitchMax = 1.05f;

        /// <summary>Minimum volume multiplier applied on top of the AudioSource's base volume.</summary>
        [SerializeField]
        private float volumeMin = 0.9f;

        /// <summary>Maximum volume multiplier applied on top of the AudioSource's base volume.</summary>
        [SerializeField]
        private float volumeMax = 1.0f;

        /// <summary>If true, <see cref="PlayRandom"/> is called automatically on enable.</summary>
        [SerializeField]
        private bool playOnEnable = false;

        private AudioSource audioSource;
        private float baseVolume;
        private readonly Queue<AudioClip> recentlyPlayed = new Queue<AudioClip>();
        private AudioClip lastPlayed;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            baseVolume = audioSource.volume;
        }

        private void OnEnable()
        {
            if (playOnEnable)
            {
                PlayRandom();
            }
        }

        private void OnValidate()
        {
            antiRepeatCount = Mathf.Max(0, antiRepeatCount);
        }

        /// <summary>
        /// Picks a clip from the pool that is not blocked by the anti-repeat queue, applies
        /// randomised pitch and volume, and plays it through the attached <see cref="AudioSource"/>.
        /// </summary>
        public void PlayRandom()
        {
            if (entries == null || entries.Count == 0)
            {
                Debug.LogWarning($"[AudioRandomizer] No entries configured on '{name}'.", this);
                return;
            }

            List<AudioEntry> candidates = GetAvailableEntries();
            AudioEntry chosen = PickWeighted(candidates);
            if (chosen == null || chosen.Clip == null)
            {
                Debug.LogWarning($"[AudioRandomizer] Selected entry has no AudioClip assigned on '{name}'.", this);
                return;
            }

            RegisterPlayed(chosen.Clip);

            audioSource.pitch = Random.Range(pitchMin, pitchMax);
            audioSource.volume = baseVolume * Random.Range(volumeMin, volumeMax);
            audioSource.clip = chosen.Clip;
            audioSource.Play();
            audioSource.volume = baseVolume;

            lastPlayed = chosen.Clip;
        }

        /// <summary>
        /// Stops playback on the attached <see cref="AudioSource"/>.
        /// </summary>
        public void Stop()
        {
            audioSource.Stop();
        }

        /// <summary>
        /// Returns the most recently played clip, or null if nothing has been played yet.
        /// </summary>
        /// <returns>The last played <see cref="AudioClip"/>.</returns>
        public AudioClip GetLastPlayed()
        {
            return lastPlayed;
        }

        /// <summary>
        /// Returns a snapshot of the clips currently blocked by the anti-repeat queue.
        /// </summary>
        /// <returns>A new list of the currently blocked clips, oldest first.</returns>
        public List<AudioClip> GetAntiRepeatQueue()
        {
            return new List<AudioClip>(recentlyPlayed);
        }

        private List<AudioEntry> GetAvailableEntries()
        {
            int cap = Mathf.Max(0, antiRepeatCount);
            if (cap == 0 || recentlyPlayed.Count == 0)
            {
                return entries;
            }

            List<AudioEntry> candidates = new List<AudioEntry>();
            for (int i = 0; i < entries.Count; i++)
            {
                if (!recentlyPlayed.Contains(entries[i].Clip))
                {
                    candidates.Add(entries[i]);
                }
            }

            if (candidates.Count == 0)
            {
                recentlyPlayed.Clear();
                return entries;
            }

            return candidates;
        }

        private static AudioEntry PickWeighted(List<AudioEntry> candidates)
        {
            float totalWeight = 0f;
            for (int i = 0; i < candidates.Count; i++)
            {
                totalWeight += Mathf.Max(0f, candidates[i].Weight);
            }

            if (totalWeight <= 0f)
            {
                return candidates[Random.Range(0, candidates.Count)];
            }

            float roll = Random.Range(0f, totalWeight);
            float accumulated = 0f;

            for (int i = 0; i < candidates.Count; i++)
            {
                accumulated += Mathf.Max(0f, candidates[i].Weight);
                if (roll < accumulated)
                {
                    return candidates[i];
                }
            }

            return candidates[candidates.Count - 1];
        }

        private void RegisterPlayed(AudioClip clip)
        {
            int cap = Mathf.Max(0, antiRepeatCount);
            if (cap == 0)
            {
                return;
            }

            recentlyPlayed.Enqueue(clip);
            while (recentlyPlayed.Count > cap)
            {
                recentlyPlayed.Dequeue();
            }
        }
    }
}

// ============================================================
// MDR Unity Toolkit - AudioRandomizer
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

using UnityEngine;

namespace MDR.Toolkit.Audio
{
    /// <summary>
    /// Sanity-check companion for <see cref="AudioRandomizer"/>. Attach alongside an
    /// <see cref="AudioRandomizer"/> to confirm setup and trigger a test play without
    /// writing any additional code.
    /// </summary>
    [RequireComponent(typeof(AudioRandomizer))]
    public class AudioRandomizerDemo : MonoBehaviour
    {
        private AudioRandomizer audioRandomizer;

        private void Start()
        {
            audioRandomizer = GetComponent<AudioRandomizer>();
            Debug.Log($"[AudioRandomizerDemo] AudioRandomizer is set up on '{name}'. Use the 'Test Play' context menu or call PlayRandom() to hear a clip.");
        }

        /// <summary>
        /// Triggers a randomised playback on the attached <see cref="AudioRandomizer"/>.
        /// </summary>
        [ContextMenu("Test Play")]
        public void TestPlay()
        {
            if (audioRandomizer == null)
            {
                audioRandomizer = GetComponent<AudioRandomizer>();
            }

            audioRandomizer.PlayRandom();
        }
    }
}

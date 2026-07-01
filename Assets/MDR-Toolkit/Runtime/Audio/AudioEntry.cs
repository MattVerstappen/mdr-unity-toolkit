// ============================================================
// MDR Unity Toolkit - AudioRandomizer
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

using System;
using UnityEngine;

namespace MDR.Toolkit.Audio
{
    /// <summary>
    /// Serializable data container pairing an <see cref="AudioClip"/> with a positive
    /// float weight, used by <see cref="AudioRandomizer"/> to build a weighted clip pool.
    /// </summary>
    [Serializable]
    public class AudioEntry
    {
        /// <summary>The clip associated with this entry.</summary>
        public AudioClip Clip;

        /// <summary>The relative weight of this entry within the pool.</summary>
        public float Weight = 1f;
    }
}

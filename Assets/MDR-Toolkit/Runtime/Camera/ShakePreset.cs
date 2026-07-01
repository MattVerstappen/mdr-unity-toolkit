// ============================================================
// MDR Unity Toolkit - TraumaCameraShake
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

using UnityEngine;

namespace MDR.Toolkit.Camera
{
    /// <summary>
    /// A reusable configuration for <see cref="TraumaCameraShake"/>. Save as an asset so
    /// designers can author and share shake profiles, or use one of the built-in static
    /// presets without creating an asset at all.
    /// </summary>
    [CreateAssetMenu(fileName = "ShakePreset", menuName = "MDR Toolkit/Camera/Shake Preset")]
    public class ShakePreset : ScriptableObject
    {
        /// <summary>How fast trauma falls per second.</summary>
        public float traumaDecayRate = 1.2f;

        /// <summary>Max position displacement in units at full trauma.</summary>
        public float maxPositionOffset = 0.3f;

        /// <summary>Max rotation angle in degrees at full trauma.</summary>
        public float maxRotationOffset = 5f;

        /// <summary>How fast the Perlin noise seed advances.</summary>
        public float perlinSpeed = 25f;

        /// <summary>Whether the X position axis is shaken.</summary>
        public bool affectPositionX = true;

        /// <summary>Whether the Y position axis is shaken.</summary>
        public bool affectPositionY = true;

        /// <summary>Whether the Z position axis is shaken.</summary>
        public bool affectPositionZ = false;

        /// <summary>Whether roll (Z rotation) is shaken.</summary>
        public bool affectRotationZ = true;

        private static ShakePreset lightPreset;
        private static ShakePreset mediumPreset;
        private static ShakePreset heavyPreset;
        private static ShakePreset explosionPreset;

        /// <summary>A subtle shake suitable for small impacts, footsteps, or gunfire.</summary>
        public static ShakePreset Light => lightPreset != null ? lightPreset : (lightPreset = Create(1.5f, 0.1f, 2f));

        /// <summary>A moderate shake suitable for hits or nearby impacts.</summary>
        public static ShakePreset Medium => mediumPreset != null ? mediumPreset : (mediumPreset = Create(1.2f, 0.3f, 5f));

        /// <summary>A strong shake suitable for heavy blows or large impacts.</summary>
        public static ShakePreset Heavy => heavyPreset != null ? heavyPreset : (heavyPreset = Create(0.8f, 0.6f, 10f));

        /// <summary>An intense, slow-decaying shake suitable for explosions.</summary>
        public static ShakePreset Explosion => explosionPreset != null ? explosionPreset : (explosionPreset = Create(0.6f, 1.0f, 15f));

        private static ShakePreset Create(float decayRate, float maxPosition, float maxRotation)
        {
            ShakePreset preset = CreateInstance<ShakePreset>();
            preset.traumaDecayRate = decayRate;
            preset.maxPositionOffset = maxPosition;
            preset.maxRotationOffset = maxRotation;
            preset.perlinSpeed = 25f;
            preset.affectPositionX = true;
            preset.affectPositionY = true;
            preset.affectPositionZ = false;
            preset.affectRotationZ = true;
            return preset;
        }
    }
}

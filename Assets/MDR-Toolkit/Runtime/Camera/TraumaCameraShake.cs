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
    /// GDC-recommended trauma-based camera shake. Trauma (0-1) decays over time and is
    /// squared to produce shake magnitude, which is sampled from Perlin noise per axis
    /// instead of random jitter, giving smooth, natural-feeling shake.
    /// </summary>
    public class TraumaCameraShake : MonoBehaviour
    {
        private const float PositionSeedOffsetX = 0f;
        private const float PositionSeedOffsetY = 100f;
        private const float PositionSeedOffsetZ = 200f;
        private const float RotationSeedOffsetZ = 300f;

        /// <summary>How fast trauma falls per second.</summary>
        [SerializeField]
        private float traumaDecayRate = 1.2f;

        /// <summary>Max position displacement in units at full trauma.</summary>
        [SerializeField]
        private float maxPositionOffset = 0.3f;

        /// <summary>Max rotation angle in degrees at full trauma.</summary>
        [SerializeField]
        private float maxRotationOffset = 5f;

        /// <summary>How fast the Perlin noise seed advances.</summary>
        [SerializeField]
        private float perlinSpeed = 25f;

        /// <summary>Whether the X position axis is shaken.</summary>
        [SerializeField]
        private bool affectPositionX = true;

        /// <summary>Whether the Y position axis is shaken.</summary>
        [SerializeField]
        private bool affectPositionY = true;

        /// <summary>Whether the Z position axis is shaken.</summary>
        [SerializeField]
        private bool affectPositionZ = false;

        /// <summary>Whether roll (Z rotation) is shaken.</summary>
        [SerializeField]
        private bool affectRotationZ = true;

        /// <summary>Optional preset applied automatically on startup and in the Inspector.</summary>
        [SerializeField]
        private ShakePreset activePreset;

        private float trauma;
        private Vector3 basePosition;
        private Quaternion baseRotation;

        private void Awake()
        {
            if (activePreset != null)
            {
                ApplyPreset(activePreset);
            }
        }

        private void OnEnable()
        {
            basePosition = transform.localPosition;
            baseRotation = transform.localRotation;
        }

        private void LateUpdate()
        {
            if (trauma <= 0f)
            {
                return;
            }

            float shake = trauma * trauma;
            float seed = Time.time * perlinSpeed;

            float offsetX = affectPositionX ? Remap(Mathf.PerlinNoise(seed + PositionSeedOffsetX, 0f)) * shake * maxPositionOffset : 0f;
            float offsetY = affectPositionY ? Remap(Mathf.PerlinNoise(seed + PositionSeedOffsetY, 0f)) * shake * maxPositionOffset : 0f;
            float offsetZ = affectPositionZ ? Remap(Mathf.PerlinNoise(seed + PositionSeedOffsetZ, 0f)) * shake * maxPositionOffset : 0f;
            float offsetRotationZ = affectRotationZ ? Remap(Mathf.PerlinNoise(seed + RotationSeedOffsetZ, 0f)) * shake * maxRotationOffset : 0f;

            transform.localPosition = basePosition + new Vector3(offsetX, offsetY, offsetZ);
            transform.localRotation = baseRotation * Quaternion.Euler(0f, 0f, offsetRotationZ);

            trauma -= traumaDecayRate * Time.deltaTime;
            if (trauma <= 0f)
            {
                trauma = 0f;
                transform.localPosition = basePosition;
                transform.localRotation = baseRotation;
            }
        }

        /// <summary>
        /// Adds trauma to the current value, clamped to [0, 1]. Multiple trauma events stack.
        /// </summary>
        /// <param name="amount">The amount of trauma to add.</param>
        public void AddTrauma(float amount)
        {
            trauma = Mathf.Clamp01(trauma + amount);
        }

        /// <summary>
        /// Sets the current trauma value directly, clamped to [0, 1].
        /// </summary>
        /// <param name="amount">The trauma value to set.</param>
        public void SetTrauma(float amount)
        {
            trauma = Mathf.Clamp01(amount);
        }

        /// <summary>
        /// Returns the current trauma value.
        /// </summary>
        /// <returns>The current trauma, in the range [0, 1].</returns>
        public float GetTrauma()
        {
            return trauma;
        }

        /// <summary>
        /// Loads shake configuration values from a <see cref="ShakePreset"/>.
        /// </summary>
        /// <param name="preset">The preset to apply. If null, nothing happens.</param>
        public void ApplyPreset(ShakePreset preset)
        {
            if (preset == null)
            {
                return;
            }

            traumaDecayRate = preset.traumaDecayRate;
            maxPositionOffset = preset.maxPositionOffset;
            maxRotationOffset = preset.maxRotationOffset;
            perlinSpeed = preset.perlinSpeed;
            affectPositionX = preset.affectPositionX;
            affectPositionY = preset.affectPositionY;
            affectPositionZ = preset.affectPositionZ;
            affectRotationZ = preset.affectRotationZ;
        }

        private static float Remap(float perlinNoise)
        {
            return (perlinNoise - 0.5f) * 2f;
        }
    }
}

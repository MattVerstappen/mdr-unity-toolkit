// ============================================================
// MDR Unity Toolkit - OffScreenIndicator
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

using UnityEngine;

namespace MDR.Toolkit.UI
{
    /// <summary>
    /// Marks a world-space GameObject for tracking by an <see cref="OffScreenIndicator"/>.
    /// </summary>
    public class OffScreenIndicatorTarget : MonoBehaviour
    {
        /// <summary>Custom color for this target's indicator.</summary>
        [SerializeField]
        private Color indicatorColor = Color.white;

        /// <summary>Optional custom icon sprite; if null uses the prefab default.</summary>
        [SerializeField]
        private Sprite customIcon;

        /// <summary>Optional label shown near the indicator.</summary>
        [SerializeField]
        private string label = string.Empty;

        /// <summary>If true, finds and registers with an <see cref="OffScreenIndicator"/> on Start().</summary>
        [SerializeField]
        private bool autoRegister = true;

        private OffScreenIndicator registeredIndicator;

        /// <summary>The custom color for this target's indicator.</summary>
        public Color IndicatorColor => indicatorColor;

        /// <summary>The custom icon sprite for this target, if any.</summary>
        public Sprite CustomIcon => customIcon;

        /// <summary>The optional label for this target.</summary>
        public string Label => label;

        /// <summary>Convenience accessor for this target's transform.</summary>
        public Transform WorldTransform => transform;

        private void Start()
        {
            if (autoRegister)
            {
                OffScreenIndicator indicator = FindObjectOfType<OffScreenIndicator>();
                if (indicator != null)
                {
                    indicator.RegisterTarget(this);
                }
            }
        }

        private void OnDestroy()
        {
            if (registeredIndicator != null)
            {
                registeredIndicator.UnregisterTarget(this);
            }
        }

        /// <summary>
        /// Sets the indicator color for this target at runtime, e.g. when the target
        /// GameObject is created and configured entirely from script.
        /// </summary>
        /// <param name="color">The color to tint this target's indicator.</param>
        public void SetIndicatorColor(Color color)
        {
            indicatorColor = color;
        }

        /// <summary>
        /// Called by <see cref="OffScreenIndicator"/> to track which manager this target is
        /// currently registered with, so it can unregister itself on destroy.
        /// </summary>
        /// <param name="indicator">The manager this target is registered with, or null.</param>
        internal void SetRegisteredIndicator(OffScreenIndicator indicator)
        {
            registeredIndicator = indicator;
        }
    }
}

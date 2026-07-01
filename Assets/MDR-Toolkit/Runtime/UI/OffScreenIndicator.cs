// ============================================================
// MDR Unity Toolkit - OffScreenIndicator
// Author: Matthew Derek Rall
// Repo: github.com/MattVerstappen/mdr-unity-toolkit
// License: MIT
// ============================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MDR.Toolkit.UI
{
    /// <summary>
    /// Draws screen-edge arrows/icons that point toward registered off-screen targets.
    /// Attach to a Canvas set to Screen Space - Overlay. Indicators are pooled and reused
    /// as targets register and unregister.
    /// </summary>
    public class OffScreenIndicator : MonoBehaviour
    {
        private class ActiveIndicator
        {
            public GameObject GameObjectInstance;
            public RectTransform RectTransform;
            public Image IconImage;
            public Text LabelText;
            public bool IsOnScreen;
        }

        /// <summary>The camera used to project world positions. Defaults to Camera.main.</summary>
        [SerializeField]
        private Camera targetCamera;

        /// <summary>Prefab containing a RectTransform with an Image (the arrow/icon).</summary>
        [SerializeField]
        private GameObject indicatorPrefab;

        /// <summary>Distance from the screen edge, in pixels.</summary>
        [SerializeField]
        private float edgePadding = 20f;

        /// <summary>If true, hides the indicator when the target is visible on screen.</summary>
        [SerializeField]
        private bool hideWhenOnScreen = true;

        /// <summary>If true, shows a distance-in-metres label on each indicator.</summary>
        [SerializeField]
        private bool showDistanceLabel = false;

        /// <summary>Tint applied to indicators with no custom color set.</summary>
        [SerializeField]
        private Color defaultColor = Color.white;

        private readonly List<OffScreenIndicatorTarget> targets = new List<OffScreenIndicatorTarget>();
        private readonly Dictionary<OffScreenIndicatorTarget, ActiveIndicator> activeIndicators = new Dictionary<OffScreenIndicatorTarget, ActiveIndicator>();
        private readonly List<GameObject> pool = new List<GameObject>();

        private void Awake()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }

        private void LateUpdate()
        {
            if (targetCamera == null || targets.Count == 0)
            {
                return;
            }

            for (int i = 0; i < targets.Count; i++)
            {
                UpdateIndicator(targets[i]);
            }
        }

        /// <summary>
        /// Registers a target for tracking, assigning it a pooled or newly-instantiated indicator.
        /// </summary>
        /// <param name="target">The target to start tracking.</param>
        public void RegisterTarget(OffScreenIndicatorTarget target)
        {
            if (target == null || activeIndicators.ContainsKey(target))
            {
                return;
            }

            GameObject instance = GetPooledIndicator();
            if (instance == null)
            {
                return;
            }

            ActiveIndicator active = new ActiveIndicator
            {
                GameObjectInstance = instance,
                RectTransform = instance.GetComponent<RectTransform>(),
                IconImage = instance.GetComponent<Image>(),
                LabelText = instance.GetComponentInChildren<Text>(true)
            };

            if (active.IconImage == null)
            {
                active.IconImage = instance.GetComponentInChildren<Image>(true);
            }

            ApplyAppearance(target, active);

            activeIndicators.Add(target, active);
            targets.Add(target);
            target.SetRegisteredIndicator(this);
        }

        /// <summary>
        /// Unregisters a target and returns its indicator to the pool.
        /// </summary>
        /// <param name="target">The target to stop tracking.</param>
        public void UnregisterTarget(OffScreenIndicatorTarget target)
        {
            if (target == null || !activeIndicators.TryGetValue(target, out ActiveIndicator active))
            {
                return;
            }

            ReturnToPool(active.GameObjectInstance);
            activeIndicators.Remove(target);
            targets.Remove(target);
            target.SetRegisteredIndicator(null);
        }

        /// <summary>
        /// Updates the camera used to project world positions to screen space.
        /// </summary>
        /// <param name="cam">The camera to use from now on.</param>
        public void SetCamera(Camera cam)
        {
            targetCamera = cam;
        }

        /// <summary>
        /// Returns the list of currently registered targets.
        /// </summary>
        /// <returns>A read-only view of the registered targets.</returns>
        public IReadOnlyList<OffScreenIndicatorTarget> GetRegisteredTargets()
        {
            return targets;
        }

        /// <summary>
        /// Returns whether the given target's last computed position was on screen.
        /// </summary>
        /// <param name="target">The target to query.</param>
        /// <returns>True if the target is currently on screen.</returns>
        public bool IsTargetOnScreen(OffScreenIndicatorTarget target)
        {
            if (target != null && activeIndicators.TryGetValue(target, out ActiveIndicator active))
            {
                return active.IsOnScreen;
            }

            return false;
        }

        private void UpdateIndicator(OffScreenIndicatorTarget target)
        {
            if (target == null || !activeIndicators.TryGetValue(target, out ActiveIndicator active))
            {
                return;
            }

            Vector3 worldPos = target.WorldTransform.position;
            Vector3 viewportPoint = targetCamera.WorldToViewportPoint(worldPos);
            bool isOnScreen = viewportPoint.z > 0f && viewportPoint.x > 0f && viewportPoint.x < 1f && viewportPoint.y > 0f && viewportPoint.y < 1f;
            active.IsOnScreen = isOnScreen;

            if (isOnScreen && hideWhenOnScreen)
            {
                active.GameObjectInstance.SetActive(false);
                return;
            }

            active.GameObjectInstance.SetActive(true);

            if (isOnScreen)
            {
                Vector2 onScreenPos = new Vector2(viewportPoint.x * Screen.width, viewportPoint.y * Screen.height);
                active.RectTransform.position = onScreenPos;
                active.RectTransform.rotation = Quaternion.identity;
            }
            else
            {
                Vector3 clampedViewport = viewportPoint;
                if (clampedViewport.z < 0f)
                {
                    clampedViewport.x = 1f - clampedViewport.x;
                    clampedViewport.y = 1f - clampedViewport.y;
                }

                Vector2 screenPos = new Vector2(clampedViewport.x * Screen.width, clampedViewport.y * Screen.height);
                screenPos.x = Mathf.Clamp(screenPos.x, edgePadding, Screen.width - edgePadding);
                screenPos.y = Mathf.Clamp(screenPos.y, edgePadding, Screen.height - edgePadding);

                float angle = Mathf.Atan2(screenPos.y - Screen.height / 2f, screenPos.x - Screen.width / 2f) * Mathf.Rad2Deg;

                active.RectTransform.position = screenPos;
                active.RectTransform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
            }

            UpdateLabel(target, active, worldPos);
        }

        private void ApplyAppearance(OffScreenIndicatorTarget target, ActiveIndicator active)
        {
            if (active.IconImage == null)
            {
                return;
            }

            if (target.CustomIcon != null)
            {
                active.IconImage.sprite = target.CustomIcon;
            }

            active.IconImage.color = target.IndicatorColor == Color.white ? defaultColor : target.IndicatorColor;
        }

        private void UpdateLabel(OffScreenIndicatorTarget target, ActiveIndicator active, Vector3 worldPos)
        {
            if (active.LabelText == null)
            {
                return;
            }

            if (!showDistanceLabel && string.IsNullOrEmpty(target.Label))
            {
                active.LabelText.text = string.Empty;
                return;
            }

            string text = target.Label;
            if (showDistanceLabel)
            {
                float distance = Vector3.Distance(targetCamera.transform.position, worldPos);
                string distanceText = $"{distance:0}m";
                text = string.IsNullOrEmpty(text) ? distanceText : $"{text} ({distanceText})";
            }

            active.LabelText.text = text;
        }

        private GameObject GetPooledIndicator()
        {
            for (int i = pool.Count - 1; i >= 0; i--)
            {
                GameObject pooled = pool[i];
                pool.RemoveAt(i);
                if (pooled != null)
                {
                    pooled.SetActive(true);
                    return pooled;
                }
            }

            if (indicatorPrefab == null)
            {
                Debug.LogWarning("[OffScreenIndicator] No indicator prefab assigned.", this);
                return null;
            }

            return Instantiate(indicatorPrefab, transform);
        }

        private void ReturnToPool(GameObject instance)
        {
            if (instance == null)
            {
                return;
            }

            instance.SetActive(false);
            pool.Add(instance);
        }
    }
}

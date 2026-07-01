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
    /// Spawns three colored sphere targets around the scene to demonstrate
    /// <see cref="OffScreenIndicator"/> without requiring any external assets.
    /// </summary>
    public class OffScreenIndicatorDemo : MonoBehaviour
    {
        /// <summary>Radius within which demo targets are spawned around the origin.</summary>
        [SerializeField]
        private float spawnRadius = 20f;

        private void Start()
        {
            SpawnTarget("Target A (Red)", Color.red);
            SpawnTarget("Target B (Green)", Color.green);
            SpawnTarget("Target C (Blue)", Color.blue);
        }

        private void SpawnTarget(string targetName, Color color)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = targetName;
            sphere.transform.position = Random.insideUnitSphere * spawnRadius;

            Renderer sphereRenderer = sphere.GetComponent<Renderer>();
            if (sphereRenderer != null)
            {
                sphereRenderer.material.color = color;
            }

            OffScreenIndicatorTarget indicatorTarget = sphere.AddComponent<OffScreenIndicatorTarget>();
            indicatorTarget.SetIndicatorColor(color);

            Debug.Log($"[OffScreenIndicatorDemo] Spawned '{targetName}' at {sphere.transform.position}.");
        }
    }
}

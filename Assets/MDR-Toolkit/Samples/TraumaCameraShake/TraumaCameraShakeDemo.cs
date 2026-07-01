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
    /// Demonstrates triggering <see cref="TraumaCameraShake"/> from keyboard input.
    /// </summary>
    public class TraumaCameraShakeDemo : MonoBehaviour
    {
        /// <summary>The camera shake component to trigger.</summary>
        [SerializeField]
        private TraumaCameraShake cameraShake;

        /// <summary>Key that triggers a light shake.</summary>
        [SerializeField]
        private KeyCode lightShakeKey = KeyCode.Alpha1;

        /// <summary>Key that triggers a heavy shake.</summary>
        [SerializeField]
        private KeyCode heavyShakeKey = KeyCode.Alpha2;

        /// <summary>Key that triggers an explosion-level shake.</summary>
        [SerializeField]
        private KeyCode explosionKey = KeyCode.Alpha3;

        private void Update()
        {
            if (cameraShake == null)
            {
                return;
            }

            if (Input.GetKeyDown(lightShakeKey))
            {
                cameraShake.AddTrauma(0.3f);
                Debug.Log("[TraumaCameraShakeDemo] Light shake triggered.");
            }

            if (Input.GetKeyDown(heavyShakeKey))
            {
                cameraShake.AddTrauma(0.6f);
                Debug.Log("[TraumaCameraShakeDemo] Heavy shake triggered.");
            }

            if (Input.GetKeyDown(explosionKey))
            {
                cameraShake.AddTrauma(1.0f);
                Debug.Log("[TraumaCameraShakeDemo] Explosion shake triggered.");
            }
        }
    }
}

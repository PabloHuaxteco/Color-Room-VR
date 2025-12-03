using UnityEngine;

namespace ColorRoomVR
{
    public class PaintVFXManager : MonoBehaviour
    {
        // Private serialized fields
        [SerializeField] private GameObject paintParticlePrefab;

        public void PlayAt(Vector3 position, Vector3 normal)
        {
            if (!paintParticlePrefab) return;

            var go = Instantiate(paintParticlePrefab, position, Quaternion.LookRotation(normal));
            Destroy(go, 2f); // TODO: use pooling instead
        }
    }
}

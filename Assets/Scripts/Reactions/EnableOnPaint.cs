using UnityEngine;

namespace ColorRoomVR
{
    public class EnableOnPaint : MonoBehaviour
    {
        // Private serialized fields
        [SerializeField] private GameObject[] objectsToEnable;
        [SerializeField] private PaintableObject target;

        private void OnEnable()
        {
            target.OnPainted.AddListener(OnPainted);
        }

        private void OnDisable()
        {
            target.OnPainted.RemoveListener(OnPainted);
        }

        private void OnPainted()
        {
            foreach (var go in objectsToEnable)
                go.SetActive(true);
        }
    }
}

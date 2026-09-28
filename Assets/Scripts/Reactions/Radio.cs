using UnityEngine;

namespace ColorRoomVR
{
    public class Radio : MonoBehaviour
    {
        // Private serialized fields
        [SerializeField] private PaintableObject baseTarget;
        [SerializeField] private PaintableGroup buttonsTarget;
        [SerializeField] private GameObject notes;
        [SerializeField] private Animator animator;

        private bool isBasePainted;
        private bool isButtonsPainted;
        private bool isPlaying;

        private void OnEnable()
        {
            baseTarget.OnPainted.AddListener(OnBasePainted);
            buttonsTarget.OnPainted.AddListener(OnButtonsPainted);
        }

        private void OnDisable()
        {
            baseTarget.OnPainted.RemoveListener(OnBasePainted);
            buttonsTarget.OnPainted.RemoveListener(OnButtonsPainted);
        }

        private void OnBasePainted()
        {
            isBasePainted = true;
            TryPlay();
        }

        private void OnButtonsPainted()
        {
            isButtonsPainted = true;
            TryPlay();
        }

        private void TryPlay()
        {
            if (isPlaying || !isBasePainted || !isButtonsPainted) return;

            isPlaying = true;
            notes.SetActive(true);
            animator.SetTrigger("Play");
        }
    }
}

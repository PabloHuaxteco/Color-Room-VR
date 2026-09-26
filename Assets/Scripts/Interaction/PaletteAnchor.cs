using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ColorRoomVR
{
    /// <summary>
    /// Keeps the palette canvas attached to the left hand, or pinned in the world in front of the player.
    /// </summary>
    public class PaletteAnchor : MonoBehaviour
    {
        public enum Mode { Hand, World }

        [Header("References")]
        [SerializeField] private Transform leftHand;
        [SerializeField] private Transform head;

        [Header("Hand mode")]
        [SerializeField] private Vector3 handPositionOffset = new Vector3(0f, 0.1f, 0.1f);
        [SerializeField] private Vector3 handRotationOffset = new Vector3(45f, 0f, 0f);

        [Header("World mode")]
        [Tooltip("Distance in front of the head where the palette is pinned.")]
        [SerializeField] private float worldDistance = 0.5f;
        [Tooltip("Horizontal offset to the left (negative) or right (positive) of the head's forward.")]
        [SerializeField] private float worldSideOffset = -0.2f;

        [Header("Input")]
        [Tooltip("Toggles between hand and world mode (left controller primary button).")]
        [SerializeField] private InputActionProperty toggleAction = new InputActionProperty(
            new InputAction("Toggle Palette Mode", InputActionType.Button, "<XRController>{LeftHand}/primaryButton"));

        [Header("Events")]
        [Tooltip("Fired with true when the palette is pinned to the world, false when attached to the hand.")]
        public UnityEvent<bool> OnModeChanged;

        private Mode _mode = Mode.Hand;
        private Vector3 _worldPosition;
        private Quaternion _worldRotation;

        public bool IsWorldMode => _mode == Mode.World;

        private void OnEnable() => toggleAction.action?.Enable();

        private void OnDisable() => toggleAction.action?.Disable();

        private void Update()
        {
            if (toggleAction.action != null && toggleAction.action.WasPressedThisFrame())
                ToggleMode();
        }

        private void LateUpdate()
        {
            if (_mode == Mode.Hand)
            {
                if (leftHand == null) return;
                transform.SetPositionAndRotation(
                    leftHand.TransformPoint(handPositionOffset),
                    leftHand.rotation * Quaternion.Euler(handRotationOffset));
            }
            else
            {
                transform.SetPositionAndRotation(_worldPosition, _worldRotation);
            }
        }

        public void ToggleMode() => SetWorldMode(_mode == Mode.Hand);

        public void SetWorldMode(bool world)
        {
            Mode newMode = world ? Mode.World : Mode.Hand;
            if (newMode == _mode) return;

            _mode = newMode;
            if (world)
                PinInFrontOfPlayer();

            OnModeChanged?.Invoke(world);
        }

        private void PinInFrontOfPlayer()
        {
            if (head == null) return;

            Vector3 forward = Vector3.ProjectOnPlane(head.forward, Vector3.up);
            if (forward.sqrMagnitude < 0.0001f)
                forward = Vector3.forward;
            forward.Normalize();
            Vector3 right = Vector3.Cross(Vector3.up, forward);

            // Keep the height of the hand so the palette stays where the player was already looking.
            float height = leftHand != null ? leftHand.position.y : head.position.y - 0.3f;
            Vector3 pos = head.position + forward * worldDistance + right * worldSideOffset;
            pos.y = height;

            _worldPosition = pos;
            // Canvas faces the player: its forward points away from the head.
            _worldRotation = Quaternion.LookRotation(pos - head.position, Vector3.up);
        }
    }
}

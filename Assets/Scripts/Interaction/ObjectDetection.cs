using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;

namespace ColorRoomVR
{
    public class ObjectDetection : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Ray used to find paintable objects. Its raycast mask and max distance decide what can be hit.")]
        [SerializeField] private XRRayInteractor rayInteractor;
        [Tooltip("Line visual of the ray. While a paintable is hovered its color is tinted with the selected color. Defaults to the one on the ray interactor.")]
        [SerializeField] private XRInteractorLineVisual lineVisual;
        [Tooltip("Action that paints the hovered object (right hand trigger).")]
        [SerializeField] private InputActionProperty paintAction;
        [SerializeField] private PaintVFXManager vfxManager;
        [SerializeField] private ColorPaletteController palette;
#if UNITY_EDITOR
        [Tooltip("Editor only: raycast from the mouse and paint with left click instead of the controller.")]
        [SerializeField] private bool mouseFallbackInEditor;
#endif

        private PaintableObject _hoveredObject;
        private PaintableGroup _hoveredGroup;
        private Gradient _originalInvalidGradient;
        private bool _lineTinted;

        private void Awake()
        {
            if (lineVisual == null && rayInteractor != null)
                lineVisual = rayInteractor.GetComponent<XRInteractorLineVisual>();
            if (lineVisual != null)
                _originalInvalidGradient = CopyGradient(lineVisual.invalidColorGradient);
        }

        private void OnEnable()
        {
            paintAction.action?.Enable();
            if (palette != null)
                palette.OnColorChange.AddListener(OnPaletteColorChanged);
        }

        private void OnDisable()
        {
            paintAction.action?.Disable();
            if (palette != null)
                palette.OnColorChange.RemoveListener(OnPaletteColorChanged);
            ClearHovered();
        }

        /// <summary>When false, nothing is hovered, outlined or painted (e.g. during the first tutorial steps).</summary>
        public bool PaintingEnabled
        {
            get => _paintingEnabled;
            set
            {
                _paintingEnabled = value;
                if (!value) ClearHovered();
            }
        }

        private bool _paintingEnabled = true;

        private void Update()
        {
            if (!_paintingEnabled)
                return;

            if (TryGetHit(out RaycastHit hit, out bool paintPressed)
                && hit.collider.TryGetComponent(out PaintableObject obj))
            {
                UpdateHover(obj);
                if (paintPressed)
                    Paint(hit);
            }
            else
            {
                ClearHovered();
            }
        }

        private bool TryGetHit(out RaycastHit hit, out bool paintPressed)
        {
            hit = default;
            paintPressed = false;

#if UNITY_EDITOR
            if (mouseFallbackInEditor && Mouse.current != null && Camera.main != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                paintPressed = Mouse.current.leftButton.wasPressedThisFrame;
                return Physics.Raycast(ray, out hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore);
            }
#endif
            if (rayInteractor == null || rayInteractor.IsOverUIGameObject())
                return false;

            paintPressed = paintAction.action != null && paintAction.action.WasPressedThisFrame();
            return rayInteractor.TryGetCurrent3DRaycastHit(out hit);
        }

        private void UpdateHover(PaintableObject obj)
        {
            var group = obj.Group;
            if (group != null)
            {
                if (group == _hoveredGroup) return;
                ClearHovered();
                _hoveredGroup = group;
                _hoveredObject = obj;
                group.EnableOutline(palette.SelectedColor);
            }
            else if (obj != _hoveredObject || _hoveredGroup != null)
            {
                ClearHovered();
                _hoveredObject = obj;
                obj.EnableOutline(palette.SelectedColor);
            }
            else
            {
                return;
            }

            SetLineColor(palette.SelectedColor);
        }

        private void Paint(RaycastHit hit)
        {
            if (_hoveredGroup != null)
                _hoveredGroup.SetColor(palette.SelectedColor);
            else if (_hoveredObject != null)
                _hoveredObject.SetColor(palette.SelectedColor);
            else
                return;

            vfxManager?.PlayAt(hit.point, hit.normal);
            ClearHovered();
        }

        private void OnPaletteColorChanged(Color color)
        {
            if (_hoveredGroup != null)
                _hoveredGroup.EnableOutline(color);
            else if (_hoveredObject != null)
                _hoveredObject.EnableOutline(color);
            else
                return;

            SetLineColor(color);
        }

        private void ClearHovered()
        {
            if (_hoveredObject != null) _hoveredObject.DisableOutline();
            if (_hoveredGroup != null) _hoveredGroup.DisableOutline();
            _hoveredObject = null;
            _hoveredGroup = null;
            RestoreLineColor();
        }

        // The line only turns "valid" over XR interactables or UI, so paintables would show the invalid (red) color.
        // Tint the invalid gradient with the selected color instead, as a preview of what will be painted.
        private void SetLineColor(Color color)
        {
            if (lineVisual == null) return;

            var gradient = new Gradient();
            gradient.SetKeys(
                new[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            lineVisual.invalidColorGradient = gradient;
            _lineTinted = true;
        }

        private void RestoreLineColor()
        {
            if (!_lineTinted || lineVisual == null) return;

            lineVisual.invalidColorGradient = _originalInvalidGradient;
            _lineTinted = false;
        }

        private static Gradient CopyGradient(Gradient source)
        {
            var copy = new Gradient { mode = source.mode };
            copy.SetKeys(source.colorKeys, source.alphaKeys);
            return copy;
        }
    }
}

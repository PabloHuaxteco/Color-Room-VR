using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace ColorRoomVR
{
    public class ObjectDetection : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask paintableMask;
        [SerializeField] private float maxDistance = 10f;
        [SerializeField] private XRController controller;
        [SerializeField] private PaintVFXManager vfxManager;
        [SerializeField] private ColorPaletteController palette;

        private Camera _mainCamera;
        private PaintableObject _hoveredObject;
        private PaintableGroup _hoveredGroup;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            //if (Physics.Raycast(controller.transform.position, controller.transform.forward, out var hit, maxDistance, paintableMask))
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var obj = hit.collider.GetComponent<PaintableObject>();
                var group = obj.Group;

                if (group != null)
                {
                    if (group != _hoveredGroup)
                    {
                        ClearHovered();
                        _hoveredGroup = group;
                        _hoveredObject = obj;
                        group.EnableOutline(palette.SelectedColor);
                    }
                    // if the group is the same, no need to change hoveredGroup
                }
                else if (obj != null && obj != _hoveredObject)
                {
                    ClearHovered();
                    _hoveredObject = obj;
                    obj.EnableOutline(palette.SelectedColor);
                }

                //if (controller.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool pressed) && pressed)
                if (Input.GetMouseButtonDown(0))
                {
                    if (_hoveredGroup)
                    {
                        _hoveredGroup.SetColor(palette.SelectedColor);
                        vfxManager?.PlayAt(hit.point, hit.normal);
                        ClearHovered();
                    }
                    else if (_hoveredObject)
                    {
                        _hoveredObject.SetColor(palette.SelectedColor);
                        vfxManager?.PlayAt(hit.point, hit.normal);
                        ClearHovered();
                    }
                }
            }
            else
            {
                ClearHovered();
            }
        }

        private void ClearHovered()
        {
            _hoveredObject?.DisableOutline();
            _hoveredObject = null;
            _hoveredGroup?.DisableOutline();
            _hoveredGroup = null;
        }
    }
}

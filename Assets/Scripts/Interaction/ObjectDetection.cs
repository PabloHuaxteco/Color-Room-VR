using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ObjectDetection : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask paintableMask;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private XRController controller;
    [SerializeField] private PaintVFXManager vfxManager;
    [SerializeField] private ColorPaletteController palette;

    private PaintableObject _hoveredObject;
    private PaintableGroup _hoveredGroup;

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(controller.transform.position, controller.transform.forward, out var hit, maxDistance, paintableMask))
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var obj = hit.collider.GetComponent<PaintableObject>();
            var group = obj.Group;

            if (group != null && group != _hoveredGroup)
            {
                ClearHovered();
                _hoveredGroup = group;
                group.EnableOutline(palette.CurrentColor);
            }
            else if (obj != null && obj != _hoveredObject)
            {
                ClearHovered();
                _hoveredObject = obj;
                obj.EnableOutline(palette.CurrentColor);
            }

            //if (controller.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool pressed) && pressed)
            //{
                if (_hoveredObject)
                {
                    _hoveredObject.SetColor(palette.CurrentColor);
                    vfxManager?.PlayAt(hit.point, hit.normal);
                    ClearHovered();
                }
                else if (_hoveredGroup)
                {
                    _hoveredGroup.SetColor(palette.CurrentColor);
                    vfxManager?.PlayAt(hit.point, hit.normal);
                    ClearHovered();
                }
            //}
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

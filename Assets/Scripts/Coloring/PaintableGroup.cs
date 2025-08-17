using System;
using UnityEngine;

public class PaintableGroup : MonoBehaviour
{
    // Private serialized fields
    [Tooltip("Unique ID for the entire group (e.g., 'Commode').")]
    [SerializeField] private string groupID;
    [Tooltip("All paintable parts belonging to this group.")]
    [SerializeField] private PaintableObject[] members;

    public string GroupID => groupID;

    private void Reset()
    {
        // Automatically set the objectID to the GameObject's name when creating or resetting the component.
        if (string.IsNullOrEmpty(groupID))
            groupID = gameObject.name;
    }

    private void Start()
    {
        if (ColorsDataManager.Instance.TryGetColor(groupID, out var saved))
        {
            SetColor(saved, false);
        }
    }

    public void SetColor(Color color, bool isPlayerAction = true)
    {
        foreach (var m in members)
        {
            m.SetColor(color, false);
        }

        if (isPlayerAction)
        {
            ColorsDataManager.Instance.SetColor(groupID, color);

            // Get the primary member of this group and invoke its OnPainted event.
            // This centralizes the event for the whole group.
            var primaryObject = members[0];
            if (primaryObject != null)
            {
                primaryObject.OnPainted?.Invoke();
            }
        }
    }

    public void EnableOutline(Color color)
    {
        foreach (var m in members)
        {
            m.EnableOutline(color);
        }
    }

    public void DisableOutline()
    {
        foreach (var m in members)
        {
            m.DisableOutline();
        }
    }
}

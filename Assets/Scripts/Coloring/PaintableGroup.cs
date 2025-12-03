using System.Collections.Generic;
using UnityEngine;

namespace ColorRoomVR
{
    public class PaintableGroup : MonoBehaviour
    {
        // Private serialized fields
        [Tooltip("Unique ID for the entire group (e.g., 'Commode').")]
        [SerializeField] private string groupID;
        [Tooltip("All paintable parts belonging to this group.")]
        [SerializeField] private List<PaintableObject> members = new List<PaintableObject>();

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
                SetColor(saved, false);
            else
                SetColor(Color.white, false);
        }

        public void AddMember(PaintableObject member)
        {
            if (member != null && !members.Contains(member))
                members.Add(member);
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
                var primaryObject = members.Count > 0 ? members[0] : null;
                if (primaryObject != null)
                    primaryObject.OnPainted?.Invoke();
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
}

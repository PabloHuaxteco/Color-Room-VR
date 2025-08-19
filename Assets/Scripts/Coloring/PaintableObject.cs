using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class PaintableObject : MonoBehaviour
{
    public enum ApplyMode { OneMaterial, AllMaterials }

    // Private serialized fields
    [Header("Group Reference")]
    [Tooltip("Reference to the PaintableGroup this object belongs to.")]
    [SerializeField] private PaintableGroup paintableGroup;
    [Tooltip("Unique identifier for this object. Used for saving/loading color.")]
    [SerializeField] private string objectID;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private ApplyMode applyMode;
    [Tooltip("The material index to change if ApplyMode is OneMaterial."), Min(0)]
    [SerializeField] private int materialIndex = 0;
    [Header("Events")]
    [Tooltip("Fired when the object's color is set by the player.")]
    public UnityEvent OnPainted;

    // Private fields
    private MeshRenderer _meshRenderer;
    private MaterialPropertyBlock _mpb;
    private bool _isPainted;

    // Public properties
    public string ObjectID => objectID;
    public bool IsPainted => _isPainted;
    public bool IsPartOfGroup => paintableGroup != null;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _mpb = new MaterialPropertyBlock();

        if (paintableGroup != null)
            paintableGroup.AddMember(this);
    }

    private void Reset()
    {
        // Automatically set the objectID to the GameObject's name when creating or resetting the component.
        if (string.IsNullOrEmpty(objectID))
            objectID = gameObject.name;
    }

    private void Start()
    {
        if (IsPartOfGroup)
            return;

        if (ColorsDataManager.Instance.TryGetColor(objectID, out var saved))
        {
            SetColor(saved, false);
            // If an object is already painted on load, we should also trigger its event
            // to ensure animations/unlocks are activated correctly.
            OnPainted?.Invoke();
            _isPainted = true;
        }
        else
        {
            SetColor(color, false);
            _isPainted = false;
        }
    }

    /// <summary>
    /// Applies a color to the object's material(s).
    /// </summary>
    /// <param name="color">The color to apply.</param>
    /// <param name="isPlayerAction">If true, this action was initiated by the player, saving the color and invoking the OnPainted event.</param>
    public void SetColor(Color color, bool isPlayerAction = true)
    {
        if (applyMode == ApplyMode.AllMaterials)
        {
            for (int i = 0; i < _meshRenderer.sharedMaterials.Length; i++)
            {
                _meshRenderer.GetPropertyBlock(_mpb, i);
                _mpb.SetColor("_BaseColor", color);
                _meshRenderer.SetPropertyBlock(_mpb, i);
            }
        }
        else
        {
            _meshRenderer.GetPropertyBlock(_mpb, materialIndex);
            _mpb.SetColor("_BaseColor", color);
            _meshRenderer.SetPropertyBlock(_mpb, materialIndex);
        }

        if (isPlayerAction)
        {
            ColorsDataManager.Instance.SetColor(objectID, color);
            OnPainted?.Invoke();
        }
    }

    public void EnableOutline(Color color)
    {
    }

    public void DisableOutline()
    {
    }
}

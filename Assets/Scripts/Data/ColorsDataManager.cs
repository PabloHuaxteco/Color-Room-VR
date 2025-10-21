using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ColorsDataManager : MonoBehaviour
{
    // Static variables and properties
    public static ColorsDataManager Instance { get; private set; }

    //Private variables (serialized fields)
    [SerializeField, Min(0)]
    private int roomID = 1;

    // Private variables
    private Dictionary<string, Color> _colors = new();
    private IColorPersistenceService _persistence;
    private readonly float _saveDelay = 1f;
    private float _lastSaveTime;
    private bool _dirty;

    //Properties
    public string OfflinePersistentDataPath { get { return Application.persistentDataPath; } private set { } }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        string path = Path.Combine(OfflinePersistentDataPath, $"ColorsRoom_{roomID}");
        _persistence = new JsonFilePersistenceService(path);

        LoadColors();
    }

    private void Update()
    {
        if (_dirty && Time.time - _lastSaveTime > _saveDelay)
            SaveColors();
    }

    public bool TryGetColor(string id, out Color color) => _colors.TryGetValue(id, out color);

    public void SetColor(string id, Color color)
    {
        if (string.IsNullOrEmpty(id)) return;

        _colors[id] = color;
        _dirty = true;
        _lastSaveTime = Time.time;
    }

    private void SaveColors()
    {
        _persistence.Save(_colors);
        _dirty = false;
    }

    private void LoadColors()
    {
        _colors = _persistence.Load();
    }

    [ContextMenu("Open Offline Persistent Data Path")]
    private void OpenOfflinePersistentDataPath()
    {
        Application.OpenURL("file://" + OfflinePersistentDataPath);
    }
}

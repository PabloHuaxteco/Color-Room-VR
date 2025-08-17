using System.Collections.Generic;
using UnityEngine;

public interface IColorPersistenceService
{
    public Dictionary<string, Color> Load();
    public void Save(Dictionary<string, Color> colors);
}

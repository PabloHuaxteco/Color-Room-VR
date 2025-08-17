using UnityEngine;

public class ColorPaletteController : MonoBehaviour
{
    // Private serialized fields
    [SerializeField] private Color currentColor = Color.white;

    public Color CurrentColor => currentColor;
    public event System.Action<Color> OnColorSelected;

    public void SelectColor(Color color)
    {
        currentColor = color;
        OnColorSelected?.Invoke(color);
    }
}

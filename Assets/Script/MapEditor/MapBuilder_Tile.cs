using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ColorUtility = UnityEngine.ColorUtility;

/// <summary>
/// Controls interacting with tiles in the map builder.
/// </summary>
public class MapBuilder_Tile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    private MapBuilder _mapBuilder;
    public SpriteRenderer SpriteRenderer;
    public int X, Y;

    void Awake() {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetMapBuilder(MapBuilder mapBuilder) {
        _mapBuilder = mapBuilder;
    }

    public void SetColor(string color) {
        Color newColor;
        if (ColorUtility.TryParseHtmlString(color, out newColor)) {
            SetColor(newColor);
        }
    }

    public void SetColor(Color color) {
        SpriteRenderer.color = color;
    }

    public Color GetColor() {
        return SpriteRenderer.color;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        Color color = GetColor();
        if (color.Equals(_mapBuilder.PalletteColor)) return;
        switch (_mapBuilder.Tool) {
            case MapBuilder_Tool.BRUSH:
                _mapBuilder.AddRecentAction(new List<MapBuilder_Tile>{ this }, color);
                SetColor(_mapBuilder.PalletteColor);
                break;
            case MapBuilder_Tool.BUCKET:
                _mapBuilder.AddRecentAction(_mapBuilder.BucketPaintAdjacent(X, Y, color), color);
                break; 
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Color color = GetColor();
        if (color.Equals(_mapBuilder.PalletteColor)) return;
        if (_mapBuilder.Tool == MapBuilder_Tool.BRUSH && Input.GetMouseButton(0)) {
            _mapBuilder.AddRecentAction(new List<MapBuilder_Tile>{ this }, color);
            SetColor(_mapBuilder.PalletteColor);
        }
    }
}

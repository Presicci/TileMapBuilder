using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Controller for the map builder scene.
/// </summary>
public class MapBuilder : MonoBehaviour
{
    [Tooltip("GameObject taht all tiles will be built under.")]
    [SerializeField] private Transform tileContainer;

    [Tooltip("Prefab for a tile.")]
    [SerializeField] private MapBuilder_Tile tilePrefab;
    
    [Tooltip("Settings panel name input field.")]
    [SerializeField] private TMP_InputField nameField;

    [Tooltip("Settings panel x length input field.")]
    [SerializeField] private TMP_InputField xField;

    [Tooltip("Settings panel y length input field.")]
    [SerializeField] private TMP_InputField yField;

    [Tooltip("How many Unity tiles does the tile prefab occupy.")]
    [SerializeField] private float tileSize = 1f;

    [Header("Runtime")]
    public string MapName = "New Map";
    public int XLength = 5, YLength = 5;
    public MapBuilder_Tile[] Tiles;
    
    [Header("Camera Constraints")]
    public float cameraXBound = 0f;
    public float cameraYBound = 0f;

    // Used for undoing actions
    private Stack<List<MapBuilder_Tile>> _recentActionTiles = new Stack<List<MapBuilder_Tile>>();
    private Stack<Color> _recentActionColors = new Stack<Color>();

    // Used for redoing actions
    private Stack<List<MapBuilder_Tile>> _recentRedoTiles = new Stack<List<MapBuilder_Tile>>();
    private Stack<Color> _recentRedoColors = new Stack<Color>();

    public Color PalletteColor;
    public MapBuilder_Tool Tool;

    void Awake() {
        PalletteColor = Color.green;
    }

    void Update() {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z)) {
            Undo();
        } else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y)) {
            Redo();
        }
    }

    /// <summary>
    /// Builds a map fresh from the stored x and y lengths.
    /// </summary>
    public void BuildMap() {
        // Clear current children
        for (int index = 0; index < tileContainer.childCount; index++) {
            Destroy(tileContainer.GetChild(index).gameObject);
        }
        if (XLength == 0 || YLength == 0) {
            Debug.Log("All dimensions need to be non-zero.");
            return;
        }
        Tiles = new MapBuilder_Tile[XLength * YLength];
        float xSize = XLength * tileSize / 2;
        float ySize = YLength * tileSize / 2;
        cameraXBound = xSize;
        cameraYBound = ySize;
        float baseX = tileContainer.position.x - xSize;
        float baseY = tileContainer.position.y - ySize;
        int count = 0;
        for (int x = 0; x < XLength; x++) {
            for (int y = 0; y < YLength; y++) {
                MapBuilder_Tile newTile = Instantiate(tilePrefab, new Vector3(baseX + x * tileSize, baseY + y * tileSize, 0f), Quaternion.identity);
                newTile.transform.SetParent(tileContainer);
                newTile.X = x;
                newTile.Y = y;
                newTile.SetMapBuilder(this);
                Tiles[count++] = newTile;
            }
        }
    }

    /// <summary>
    /// Builds a map from a save.
    /// </summary>
    /// <param name="mapData">Save data to load.</param>
    public void BuildMap(MapData mapData) {
        // Clear current children
        for (int index = 0; index < tileContainer.childCount; index++) {
            Destroy(tileContainer.GetChild(index).gameObject);
        }
        MapName = mapData.Name;
        XLength = mapData.XLength;
        YLength = mapData.YLength;
        UpdateSettingsFields();
        Tiles = new MapBuilder_Tile[XLength * YLength];
        float xSize = XLength * tileSize / 2;
        float ySize = YLength * tileSize / 2;
        cameraXBound = xSize;
        cameraYBound = ySize;
        float baseX = tileContainer.position.x - xSize;
        float baseY = tileContainer.position.y - ySize;
        int count = 0;
        foreach (TileData tile in mapData.Tiles) {
            MapBuilder_Tile newTile = Instantiate(tilePrefab, new Vector3(baseX + tile.X * tileSize, baseY + tile.Y * tileSize, 0f), Quaternion.identity);
            newTile.transform.SetParent(tileContainer);
            newTile.X = tile.X;
            newTile.Y = tile.Y;
            newTile.SetMapBuilder(this);
            newTile.SetColor(tile.Color);
            Tiles[count++] = newTile;
        }
    }

    public List<MapBuilder_Tile> BucketPaintAdjacent(int x, int y, Color color) {
        List<MapBuilder_Tile> originalTiles = new List<MapBuilder_Tile>();
        if (PalletteColor == color || x < 0 || y < 0 || x >= XLength || y >= YLength) return originalTiles;
        MapBuilder_Tile tile = GetTile(x, y);
        if (tile.SpriteRenderer.color == color) {
            originalTiles.Add(tile);
            tile.SetColor(PalletteColor);
            originalTiles.AddRange(BucketPaintAdjacent(x - 1, y, color));
            originalTiles.AddRange(BucketPaintAdjacent(x + 1, y, color));
            originalTiles.AddRange(BucketPaintAdjacent(x, y - 1, color));
            originalTiles.AddRange(BucketPaintAdjacent(x, y + 1, color));
        }
        return originalTiles;
    }

    public void Undo() {
        if (_recentActionTiles.Count > 0) {
            List<MapBuilder_Tile> changedTiles = _recentActionTiles.Pop();
            Color color = _recentActionColors.Pop();
            _recentRedoTiles.Push(changedTiles);
            _recentRedoColors.Push(changedTiles[0].SpriteRenderer.color);
            foreach (MapBuilder_Tile tile in changedTiles) {
                tile.SetColor(color);
            }
        }
    }

    public void Redo() {
        if (_recentRedoTiles.Count > 0) {
            List<MapBuilder_Tile> changedTiles = _recentRedoTiles.Pop();
            Color color = _recentRedoColors.Pop();
            _recentActionTiles.Push(changedTiles);
            _recentActionColors.Push(changedTiles[0].SpriteRenderer.color);
            foreach (MapBuilder_Tile tile in changedTiles) {
                tile.SetColor(color);
            }
        }
    }

    /// <summary>
    /// Updates the input fields with the new MapData that is being loaded.
    /// </summary>
    private void UpdateSettingsFields() {
        nameField.text = MapName;
        xField.text = "" + XLength;
        yField.text = "" + YLength;
    }

    public void AddRecentAction(List<MapBuilder_Tile> tiles, Color color) {
        _recentActionTiles.Push(tiles);
        _recentActionColors.Push(color);
        _recentRedoTiles.Clear();
        _recentRedoColors.Clear();
    }

    private void SelectColor(Color color) {
        PalletteColor = color;
    }

    public void SelectColor(string color) {
        Color newColor;
        if (ColorUtility.TryParseHtmlString(color, out newColor)) {
            SelectColor(newColor);
        }
    }

    public void SetTool(string tool) {
        Tool = (MapBuilder_Tool) Enum.Parse(typeof(MapBuilder_Tool), tool);
    }

    public void SetName(string name) {
        MapName = name;
    }

    public void SetXLength(string x) {
        XLength = int.Parse(x);
    }

    public void SetYLength(string y) {
        YLength = int.Parse(y);
    }

    public MapBuilder_Tile GetTile(int x, int y) {
        return Tiles[x * YLength + y];
    }
}

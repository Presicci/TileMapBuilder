using System.IO;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles serialization of MapData objects.
/// </summary>
public class MapSerialization : MonoBehaviour
{
    [Tooltip("MapBuilder component that is used for saving/loading.")]
    [SerializeField] private MapBuilder mapBuilder;
    
    [Tooltip("Panel that will show when attempting to save with a filename that already exitsts.")]
    [SerializeField] private GameObject overwritePanel;

    private string _basePath;

    void Awake() {
        _basePath = Application.persistentDataPath + "/maps/";
    }

    /// <summary>
    /// Json save process called from the map editor.
    /// </summary>
    public void ToJson() {
        string path = _basePath + mapBuilder.MapName + ".json";
        // If the file exists, asks the user if they want to overwrite
        if (File.Exists(path)) {
            overwritePanel.SetActive(true);
            return;
        }
        ToJsonOverwrite();
    }

    /// <summary>
    /// Json save process that bypasses a overwrite check.
    /// </summary>
    public void ToJsonOverwrite() {
        MapData mapData = new MapData(mapBuilder.MapName, mapBuilder.XLength, mapBuilder.YLength, mapBuilder.Tiles);
        string path = _basePath + mapData.Name + ".json";
        string json = JsonUtility.ToJson(mapData);
        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Json load process called from the map editor and game startup.
    /// </summary>
    /// <param name="fileName">File name of the map, minus the extension.</param>
    public void FromJson(string fileName) {
        string path = _basePath + fileName + ".json";
        if (!File.Exists(path)) {
            Debug.Log("No such file.");
        }
        string json = File.ReadAllText(path);
        MapData mapData = JsonUtility.FromJson<MapData>(json);
        mapBuilder.BuildMap(mapData);
    }

    /// <summary>
    /// Json load process called from the map editor and game startup.
    /// </summary>
    /// <param name="textMesh">Extracts the text field as the file name.</param>
    public void FromJson(TextMeshProUGUI textMesh) {
        FromJson(textMesh.text);
    }
}

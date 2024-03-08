using System.IO;
using TMPro;
using UnityEngine;

/// <summary>
/// Container on the load save panel.
/// </summary>
public class UI_SaveFileContainer : MonoBehaviour
{
    [Tooltip("Serialization component used for loading the selected save.")]
    [SerializeField] private MapSerialization mapSerialization;

    [Tooltip("Currently selected file textmesh.")]
    [SerializeField] private TextMeshProUGUI currentFileTextMesh;

    [Tooltip("Prefab for the file button.")]
    [SerializeField] private Transform filePrefab;

    public void PopulateFileList() {
        for (int index = 0; index < transform.childCount; index++) {
            Destroy(transform.GetChild(index).gameObject);
        }
        string[] files = Directory.GetFiles(Application.persistentDataPath + "/maps/");
        // Iterate through each file in the directory
        foreach (string file in files) {
            string filename = Path.GetFileName(file);
            filename = filename.Replace(".json", "");
            Transform saveTransform = Instantiate(filePrefab, Vector3.zero, Quaternion.identity);
            saveTransform.GetComponentInChildren<TextMeshProUGUI>().text = filename;
            saveTransform.SetParent(transform);
        }
    }

    public void SelectFile(TextMeshProUGUI textMesh) {
        currentFileTextMesh.text = textMesh.text;
    }

    public void Load() {
        string fileName = currentFileTextMesh.text;
        if (!fileName.ToLower().Equals("select a file...")) {
            mapSerialization.FromJson(fileName);
        }
    }
}

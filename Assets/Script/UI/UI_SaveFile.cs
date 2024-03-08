using TMPro;
using UnityEngine;

/// <summary>
/// Button on the load map save panel, represents a single save file.
/// </summary>
public class UI_SaveFile : MonoBehaviour
{
    [Tooltip("TextMesh holding the save filename.")]
    [SerializeField] private TextMeshProUGUI textMesh;
    
    private UI_SaveFileContainer _container;

    void Start() {
        _container = transform.parent.GetComponent<UI_SaveFileContainer>();
    }

    public void Click() {
        _container.SelectFile(textMesh);
    }
}
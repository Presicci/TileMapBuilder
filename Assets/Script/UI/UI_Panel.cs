using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Collapsable panel that can be placed along the side of the screen.
/// </summary>
public class UI_Panel : MonoBehaviour
{
    [Tooltip("Content containers that the panel can hold.")]
    [SerializeField] private Transform[] containers;

    [Tooltip("Text for the button that toggles the panel open/closed.")]
    [SerializeField] private TextMeshProUGUI toggleButtonText;

    [Tooltip("Destination transform for what the panel should look like in the open postion.")]
    [SerializeField] private RectTransform openTransform;

    [Tooltip("Destination transform for what the panel should look like in the closed position.")]
    [SerializeField] private RectTransform closedTransform;

    [Tooltip("Text that the toggle button displays when in the open position. Defaults to '<'")]
    [SerializeField] private string toggleButtonOpenText = "<";

    [Tooltip("Text that the toggle button displays when in the closed position. Defaults to '>'")]
    [SerializeField] private string toggleButtonClosedText = ">";

    [Tooltip("Speed at which the panel closes/opens.")]
    [SerializeField] private float toggleSpeed = 3f;

    private RectTransform _rectTransform;

    void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void ToggleActive() {
        if (IsOpen()) {
            Close();
        } else {
            Open();
        }
    }

    public void Open() {
        if (IsOpen()) return;
        StartCoroutine(MoveTo(_rectTransform.sizeDelta, openTransform.sizeDelta, toggleSpeed, false));
        toggleButtonText.text = toggleButtonOpenText;
        SetContainerIndex(0);
    }

    private void Close() {
        StartCoroutine(MoveTo(_rectTransform.sizeDelta, closedTransform.sizeDelta, toggleSpeed, true));
        toggleButtonText.text = toggleButtonClosedText;
    }

    /// <summary>
    /// Coroutine that animates the panel opening/closing.
    /// </summary>
    IEnumerator MoveTo(Vector2 from, Vector2 to, float speed, bool close) {
        float time = 0f;
        while (time < 1f) {
            time += speed * Time.deltaTime;
            _rectTransform.sizeDelta = Vector2.Lerp(from, to, time);
            yield return null;
        }
        if (close) {
            foreach (Transform container in containers) {
                container.gameObject.SetActive(false);
            }
        }
    }

    private bool IsOpen() {
        return _rectTransform.sizeDelta == openTransform.sizeDelta;
    }

    public void SetContainerIndex(int index) {
        if (index >= containers.Length) return;
        foreach (Transform container in containers) {
            container.gameObject.SetActive(false);
        }
        containers[index].gameObject.SetActive(true);
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// Custom button component for our cute pixel button.
/// </summary>
public class UI_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Sprites")]
    [Tooltip("Sprite that is displayed at rest.")]
    [SerializeField] private Sprite sprite;

    [Tooltip("Sprite that is displayed when hovered.")]
    [SerializeField] private Sprite hoverSprite;

    [Tooltip("Sprite that is dispalyed when held down.")]
    [SerializeField] private Sprite clickSprite;

    [Header("Clicking")]
    [Tooltip("How far down the text moves when the button is clicked.")]
    [SerializeField] private float textMovement = 5f;

    [Tooltip("Events that are fired when the button is successfully clicked.")]
    [SerializeField] private UnityEvent clickEvent = new UnityEvent();

    private Image _image;
    private bool _hovered;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hovered = true;
        _image.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovered = false;
        if (_image.sprite == clickSprite) _text.rectTransform.anchoredPosition = Vector3.zero;
        _image.sprite = sprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _image.sprite = clickSprite;
        if (_text != null) _text.rectTransform.anchoredPosition = new Vector3(0, -textMovement, 0);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_hovered) return;
        _image.sprite = hoverSprite;
        if (_text != null) _text.rectTransform.anchoredPosition = Vector3.zero;
        clickEvent.Invoke();
    }
}

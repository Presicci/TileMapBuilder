using UnityEngine;

/// <summary>
/// Controls the camera for the map builder scene.
/// </summary>
public class MapBuilder_Camera : MonoBehaviour
{
    [Tooltip("MapBuilder object that the drag constraints are pulled from.")]
    [SerializeField] private MapBuilder mapBuilder;

    [Tooltip("Multiplier for mouse drag distance to camera distance.")]
    [Range(0.01f, 0.1f)]
    [SerializeField] private float dragSpeed = 1f;

    [Tooltip("Multiplier for zoom and drag camera movement interpolation.")]
    [Range(1, 10)]
    [SerializeField] private float lerpSpeed = 1f;

    [Header("Zoom")]

    [Tooltip("Min orthographic camera size.")]
    [Range(2f, 5f)]
    [SerializeField] private float minZoom = 4f;

    [Tooltip("Max orthographic camera size.")]
    [Range(5f, 15f)]
    [SerializeField] private float maxZoom = 10f;

    private Vector3 _dragStart;
    private Vector3 _destination;
    private float _scrollStep;
    private Camera _camera;


    void Awake() {
        _destination = transform.position;
        _scrollStep = 5f;
        _camera = GetComponent<Camera>();
    }

    void Update() {
        // Drag interpolation
        Vector3 dest = Vector2.Lerp(transform.position, _destination, Time.deltaTime * lerpSpeed);
        transform.position = dest + new Vector3(0, 0, -10); // Z is always -10, we 2D
        // Zoom interpolation
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _scrollStep, Time.deltaTime * lerpSpeed);

        if (Input.GetMouseButtonDown(2)) {      // Middle mouse pressed
            _dragStart = Input.mousePosition;
            return;
        } else if (Input.GetMouseButton(2)) {   // Middle mouse held
            // Update camera destination based on drag distance
            Vector3 delta = (Input.mousePosition - _dragStart) * dragSpeed;
            _dragStart = Input.mousePosition;
            _destination -= delta;
            // Clamps our camera destination to our map builder constraints
            if (_destination.x > mapBuilder.cameraXBound)
                _destination.x = mapBuilder.cameraXBound;
            if (_destination.x < -mapBuilder.cameraXBound)
                _destination.x = -mapBuilder.cameraXBound;
            if (_destination.y > mapBuilder.cameraYBound)
                _destination.y = mapBuilder.cameraYBound;
            if (_destination.y < -mapBuilder.cameraYBound)
                _destination.y = -mapBuilder.cameraYBound;
        }
        // Scrolling
        if (Input.GetAxis( "Mouse ScrollWheel" ) > 0f) {
            if (_scrollStep <= minZoom) return;
            _scrollStep -= 1f;  // Zoom in
        } else if (Input.GetAxis( "Mouse ScrollWheel" ) < 0f) {
            if (_scrollStep >= maxZoom) return;
            _scrollStep += 1f;  // Zoom out
        }
    }
}

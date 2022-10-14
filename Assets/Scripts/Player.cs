using UnityEngine;
using static UnityEngine.Vector3;
using static UnityEngine.Mathf;

public class Player : MonoBehaviour
{
    public Camera orthographicCamera;
    public float movementSpeed;
    public float originalMovementSpeed;
    public float movementTime;
    public float zoomSensitivity;
    private Vector3 _newPosition;
    private float _orthographicSize;
    private UIMain _uiMain;


    public void Start()
    {
        _newPosition = transform.position;
        orthographicCamera = GetComponent<Camera>();
        originalMovementSpeed = movementSpeed;
        _orthographicSize = orthographicCamera.orthographicSize;

        _uiMain = GameObject.Find("UIMain").GetComponent<UIMain>();
    }

    public void Update()
    {
        UseInput();
    }

    private void UseInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) _newPosition += transform.up * movementSpeed;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) _newPosition += transform.up * -movementSpeed;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            _newPosition += transform.right * -movementSpeed;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            _newPosition += transform.right * movementSpeed;
        if (Input.GetKey(KeyCode.Escape)) _uiMain.CloseLastWindow();
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            _orthographicSize += Input.mouseScrollDelta.y * zoomSensitivity;
            orthographicCamera.orthographicSize = _orthographicSize;
            orthographicCamera.orthographicSize = Clamp(_orthographicSize, 50f, 100f);

            var percentage = orthographicCamera.orthographicSize / 100;
            movementSpeed = originalMovementSpeed * percentage;
        }

        if (Input.GetMouseButtonDown(1)) SelectCountry();

        transform.position =
            Lerp(transform.position,
                _newPosition,
                Time.deltaTime * movementTime);
    }

    private void SelectCountry()
    {
        var mousePosition =
            orthographicCamera.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Raycast(mousePosition, -Vector2.up);

        if (hit.collider == null) return;
        if (!hit.transform.CompareTag("Country")) return;
        var hitCountry = hit.transform.GetComponent<Country>();
        CountryDisplayManager
            .Instance
            .SetIsCountrySelected(hitCountry.CountryData);
    }
}

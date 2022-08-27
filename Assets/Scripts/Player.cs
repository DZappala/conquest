using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera orthographicCamera;

    public float movementSpeed;

    public float originalMovementSpeed;

    public float movementTime;

    public Vector3 newPosition;

    public float zoomSpeedFactor;

    public GameObject countryDisplay;

    public bool selectedCountry;

    public void Start()
    {
        newPosition = transform.position;
        orthographicCamera = GetComponent<Camera>();
        originalMovementSpeed = movementSpeed;

        countryDisplay = GameObject.FindWithTag("CountryPanel");
    }

    public void Update()
    {
        UseInput();
    }

    public void UseInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += transform.up * movementSpeed;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += transform.up * -movementSpeed;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += transform.right * -movementSpeed;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += transform.right * movementSpeed;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            countryDisplay.SetActive(false);
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            orthographicCamera.orthographicSize -=
                Input.GetAxis("Mouse ScrollWheel") * zoomSpeedFactor;
            orthographicCamera.orthographicSize =
                Mathf.Clamp(orthographicCamera.orthographicSize, 10, 100);
            var orthographicSizePercentage =
                orthographicCamera.orthographicSize / 100;
            movementSpeed = originalMovementSpeed * orthographicSizePercentage;
        }

        if (Input.GetMouseButtonDown(1))
        {
            SelectCountry();
        }

        transform.position =
            Vector3
                .Lerp(transform.position,
                newPosition,
                Time.deltaTime * movementTime);
    }

    public void SelectCountry()
    {
        Vector2 mousePosition =
            orthographicCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, -Vector2.up);

        if (hit.collider != null)
        {
            if (hit.transform.tag == "Country")
            {
                var hitCountry = hit.transform.GetComponent<Country>();
                CountryDisplayManager
                    .Instance
                    .SetIsCountrySelected(hitCountry.CountryData);
            }
        }
        else
        {
            countryDisplay.SetActive(false);
        }
    }
}

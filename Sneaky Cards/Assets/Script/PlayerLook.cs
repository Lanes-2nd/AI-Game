using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 0.1f;
    public Transform playerBody;

    private PlayerControl controls;
    private float xRotation = 0f;

    private void Awake()
    {
        controls = new PlayerControl();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 mouseInput = controls.Player.Look.ReadValue<Vector2>();

        float mouseX = mouseInput.x * mouseSensitivity;
        float mouseY = mouseInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }
}
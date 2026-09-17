using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 0.1f;
    public Transform playerBody;

    [Header("Crouching")]
    public float standingCameraHeight = 1.6f;
    public float crouchingCameraHeight = 0.8f;
    public float cameraCrouchSpeed = 8f;

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
        Look();
        HandleCameraCrouch();
    }

    private void Look()
    {
        Vector2 mouseInput = controls.Player.Look.ReadValue<Vector2>();

        float mouseX = mouseInput.x * mouseSensitivity;
        float mouseY = mouseInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void HandleCameraCrouch()
    {
        bool isCrouching = controls.Player.Crouch.IsPressed();

        float targetHeight = isCrouching
            ? crouchingCameraHeight
            : standingCameraHeight;

        Vector3 cameraPosition = transform.localPosition;

        cameraPosition.y = Mathf.Lerp(
            cameraPosition.y,
            targetHeight,
            cameraCrouchSpeed * Time.deltaTime
        );

        transform.localPosition = cameraPosition;
    }
}
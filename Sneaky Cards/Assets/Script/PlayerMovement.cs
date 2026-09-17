using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float crouchSpeed = 2f;

    [Header("Gravity")]
    public float gravity = -20f;

    [Header("Crouching")]
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;
    public float crouchTransitionSpeed = 8f;

    private CharacterController controller;
    private PlayerControl controls;
    private Vector3 velocity;

    private bool isCrouching;

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
        controller = GetComponent<CharacterController>();

        controller.height = standingHeight;
        controller.center = new Vector3(0f, standingHeight / 2f, 0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleCrouch();
        MovePlayer();
    }

    private void HandleCrouch()
    {
        isCrouching = controls.Player.Crouch.IsPressed();

        float targetHeight = isCrouching
            ? crouchingHeight
            : standingHeight;

        controller.height = Mathf.Lerp(
            controller.height,
            targetHeight,
            crouchTransitionSpeed * Time.deltaTime
        );

        controller.center = new Vector3(
            0f,
            controller.height / 2f,
            0f
        );
    }

    private void MovePlayer()
    {
        Vector2 input = controls.Player.Move.ReadValue<Vector2>();

        Vector3 move =
            transform.right * input.x +
            transform.forward * input.y;

        float speed;

        if (isCrouching)
        {
            speed = crouchSpeed;
        }
        else if (controls.Player.Sprint.IsPressed())
        {
            speed = sprintSpeed;
        }
        else
        {
            speed = walkSpeed;
        }

        controller.Move(move * speed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
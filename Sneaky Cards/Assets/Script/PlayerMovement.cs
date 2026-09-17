using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float gravity = -20f;

    private CharacterController controller;
    private PlayerControl controls;
    private Vector3 velocity;

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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector2 input = controls.Player.Move.ReadValue<Vector2>();

        Vector3 move =
            transform.right * input.x +
            transform.forward * input.y;

        float speed = controls.Player.Sprint.IsPressed()
            ? sprintSpeed
            : walkSpeed;

        controller.Move(move * speed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
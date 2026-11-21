using UnityEngine;
using UnityEngine.InputSystem;

// Moves player left/right (side-scroller)
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f; // units per second

    private Rigidbody2D rb;
    private InputAction moveAction;
    private float moveInput; // -1 = left, 0 = idle, 1 = right

    private void Awake()
    {
        // cache rigidbody
        rb = GetComponent<Rigidbody2D>();

        // get "Move" action from project-wide input actions
        moveAction = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
        // read input (Vector2), keep only x
        moveInput = moveAction.ReadValue<Vector2>().x;
    }

    private void FixedUpdate()
    {
        // set horizontal velocity, keep vertical velocity
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }
}
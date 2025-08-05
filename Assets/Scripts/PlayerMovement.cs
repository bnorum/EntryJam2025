using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputAction moveAction;
    public InputAction jumpAction;

    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState != GameManager.GameState.FREE)
        {
            return; // Skip movement if not in free state
        }
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        rb.position += new Vector2(moveInput.x * moveSpeed, 0) * Time.deltaTime;

        if (jumpAction.triggered && Mathf.Abs(rb.linearVelocity.y) < 0.001f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}

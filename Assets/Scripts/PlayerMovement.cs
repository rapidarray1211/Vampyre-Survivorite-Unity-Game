using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 4000f;
    public float dashDuration = 3.65f;
    public float dashCooldown = 2f;
    private Rigidbody2D rb;
    public Vector2 movementInput;
    private bool dashUnlocked = false;
    public bool isDashing = false;
    private float lastDashTime = -Mathf.Infinity;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void HandleMovement()
    {
        if (isDashing) return;

        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        movementInput = movementInput.normalized;

        // DASH
        if (dashUnlocked && Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.time >= lastDashTime + dashCooldown && movementInput != Vector2.zero)
            {
                StartCoroutine(PerformDash(movementInput));
                lastDashTime = Time.time;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.linearVelocity = movementInput * moveSpeed;
        }
    }

    private System.Collections.IEnumerator PerformDash(Vector2 direction)
    {
        isDashing = true;
        rb.linearVelocity = direction * dashSpeed * moveSpeed;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }

    public void EnableUpgrade(string upgrade)
    {
        if (upgrade.Contains("Dash"))
        {
            dashUnlocked = true;
        }
    }
}

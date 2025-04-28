using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public Animator animator;
    public PlayerStats stats;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 12f;
    public float dashSpeed = 20f;
    public float levitateForce = 5f;
    public float dashDuration = 0.2f;

    private PlayerInputActions input;
    private Vector2 moveInput;
    private bool isRunning;
    private bool isJumping;
    private bool isLevitating;
    private bool canDoubleJump;
    private bool isDashing;

    private bool facingRight = true;

    private void Awake()
    {
        input = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        input.Player.Enable();
        input.Player.Jump.performed += ctx => OnJump();
        input.Player.Dash.performed += ctx => StartCoroutine(Dash());
        input.Player.Ability1.performed += ctx => UseLevitation();
    }

    private void OnDisable()
    {
        input.Player.Disable();
    }

    private void Update()
    {
        moveInput = input.Player.Move.ReadValue<Vector2>();
        isRunning = Keyboard.current.leftShiftKey.isPressed;

        Flip(moveInput.x);

        // Some animations
        animator?.SetFloat("Speed", Mathf.Abs(moveInput.x));
        animator?.SetBool("isJumping", !IsGrounded());
        animator?.SetBool("isFalling", rb.linearVelocity.y < -0.1f);

        if (!isJumping && IsGrounded() && rb.linearVelocity.y <= 0.1f)
        {
            animator?.SetTrigger("Land");
        }

        if (isLevitating && stats.currentMana > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, levitateForce);
            stats.UseMana(Time.deltaTime * 5f);
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        float speed = isRunning ? runSpeed : walkSpeed;

        if (isRunning) stats.UseStamina(Time.fixedDeltaTime * 5f);

        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
    }

    void OnJump()
    {
        if (IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = true;
        }
        else if (canDoubleJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = false;
        }
    }

    void UseLevitation()
    {
        if (stats.currentMana > 0)
            isLevitating = true;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        float dir = moveInput.x != 0 ? Mathf.Sign(moveInput.x) : transform.localScale.x;
        rb.linearVelocity = new Vector2(dir * dashSpeed, 0);
        animator?.SetTrigger("Dash");
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }

    private bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 1.1f, LayerMask.GetMask("Ground"));
    }

    void Flip(float moveX)
    {
        if (moveX > 0 && !facingRight)
        {
            facingRight = true;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        else if (moveX < 0 && facingRight)
        {
            facingRight = false;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}
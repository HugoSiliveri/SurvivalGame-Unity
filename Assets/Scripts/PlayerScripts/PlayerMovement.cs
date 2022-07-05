using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;

    Vector2 movement;

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if (movement.x > 0)
        {
            animator.SetBool("Right", true);
            animator.SetBool("Left", false);
            animator.SetBool("Back", false);
            animator.SetBool("Front", false);
        }
        else if (movement.x < 0)
        {
            animator.SetBool("Right", false);
            animator.SetBool("Left", true);
            animator.SetBool("Back", false);
            animator.SetBool("Front", false);
        }
        else if (movement.y > 0)
        {
            animator.SetBool("Right", false);
            animator.SetBool("Left", false);
            animator.SetBool("Back", true);
            animator.SetBool("Front", false);
        }
        else if (movement.y < 0)
        {
            animator.SetBool("Right", false);
            animator.SetBool("Left", false);
            animator.SetBool("Back", false);
            animator.SetBool("Front", true);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}

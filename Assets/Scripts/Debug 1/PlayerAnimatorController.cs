using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float walkThreshold = 0.1f;

    private float moveX;
    private bool isGrounded = true;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        ReadInput();
        UpdateAnimator();
    }

    private void ReadInput()
    {
        moveX = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayJump();
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J))
        {
            PlayAttack();
        }
    }

    private void UpdateAnimator()
    {
        float speed = Mathf.Abs(moveX);

        animator.SetFloat(SpeedHash, speed);
        animator.SetBool(IsGroundedHash, isGrounded);

        // Opcional: voltear personaje
        if (moveX > walkThreshold)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveX < -walkThreshold)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void PlayJump()
    {
        if (!isGrounded)
            return;

        isGrounded = false;
        animator.SetTrigger(JumpHash);
    }

    public void PlayAttack()
    {
        animator.SetTrigger(AttackHash);
    }

    // Llama esto desde tu sistema de movimiento cuando toque el suelo
    public void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
    }
}
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private PlayerController controller;

    private static readonly int XSpeed = Animator.StringToHash("xSpeed");
    private static readonly int YVelocity = Animator.StringToHash("yVelocity");
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        animator.SetFloat(XSpeed, Mathf.Abs(controller.xInput));
        animator.SetFloat(YVelocity, controller.yVelocity);
        animator.SetBool(IsGrounded, controller.isGrounded);
    }
}

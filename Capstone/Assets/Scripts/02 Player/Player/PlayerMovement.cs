using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController
{
    [SerializeField]
    private float moveSpeed = 5f;         // 움직임 속도
    [SerializeField]
    private float jumpHeight = 3f;        // 점프 세기
    [SerializeField]
    private float runSpeed = 1.8f;        // 달리기 속도  

    private Vector3 moveDirection;
    private Vector3 moveVelocity;

    public bool IsGround { get; private set; }
    public bool IsMove { get; private set; }
    public bool IsRun { get; private set; }

    private void PlayerJump()
    {
        if (JumpTrigger && IsGround)
        {
            Vector3 jumpForce = Vector3.up * Mathf.Sqrt(jumpHeight * -Physics.gravity.y);
            playerRB.AddForce(jumpForce, ForceMode.VelocityChange);
            IsGround = false;
        }
    }

    private void PlayerMove()
    {
        moveDirection = transform.forward * MoveInput.y + transform.right * MoveInput.x;
        moveVelocity = moveDirection * moveSpeed;

        playerRB.velocity = new Vector3(moveVelocity.x, playerRB.velocity.y, moveVelocity.z);

        IsMove = true;
    }

    private void PlayerRun()
    {
        if(RunTrigger)
        {
            playerRB.velocity = new Vector3(moveVelocity.x, playerRB.velocity.y, moveVelocity.z) * runSpeed;

            IsRun = true;
        }
    }
}

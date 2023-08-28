using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    public Rigidbody 지훈;

    private Vector3 velocity;
    float moveSpeed = 10;


    void Update()
    {
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;

        // 만들어진 moveVelocity를 PlayerController에 전달해서 물리적인 부분들을 처리한다.
        Move(moveVelocity);

        지훈.MovePosition(지훈.position + velocity * Time.fixedDeltaTime);
    }

    public void Move(Vector3 velocity)
    {
        this.velocity = velocity;
    }
}

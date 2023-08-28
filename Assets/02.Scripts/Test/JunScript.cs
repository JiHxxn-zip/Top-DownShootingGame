using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunScript : MonoBehaviour
{
    public Rigidbody 지훈;

    private Vector3 velocity;
    public float movespeed = 10;


    void Update()
    {
        Vector3 Hi = new Vector3(   1.5f, 
                                    1.2f, 
                                    1.5f);

        Vector3 moveinput = new Vector3(    0,
                                            Input.GetAxisRaw("Horizontal"),
                                            Input.GetAxisRaw("Vertical"));

        Vector3 movevelocity = moveinput.normalized * movespeed;

        Move(movevelocity);

        지훈.MovePosition(지훈.position + velocity * Time.fixedDeltaTime);
    }


    public void Move (Vector3 velocity)
    {
        this.velocity = velocity;
    }

    
}

using UnityEngine;

public class MyAgent : MonoBehaviour
{
    public float speed = 3f;

    private Rigidbody rb;
    private Animator animator;
    private bool isMoving = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        SoldierRotation(moveHorizontal, moveVertical);

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.velocity = movement * speed;

        isMoving = movement.magnitude > 0;

        animator.SetBool("IsRunning", isMoving);
    }

    private void SoldierRotation(float horizontal, float vertical)
    {
        //make the rotation smooth
        if (horizontal != 0 || vertical != 0)
        {
            Vector3 lookDirection = new Vector3(horizontal, 0, vertical);
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10);
        }

        //if the player is not moving, keep the last rotation
        if (!isMoving)
        {
            animator.SetBool("IsRunning", false);
        }
    }
}

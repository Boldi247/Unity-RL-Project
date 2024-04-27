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

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.velocity = movement * speed;

        isMoving = movement.magnitude > 0;

        animator.SetBool("IsRunning", isMoving);
    }
}

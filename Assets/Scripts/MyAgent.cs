using Unity.VisualScripting;
using UnityEngine;

public class MyAgent : MonoBehaviour
{
    public float speed = 3f;

    private Rigidbody rb;
    private Animator animator;
    private bool isMoving = false;

    public GameObject bullet;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        bullet.SetActive(false);
    }

    private void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootBullet();
        }

        SoldierRotation(moveHorizontal, moveVertical);

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.velocity = movement * speed;

        isMoving = movement.magnitude > 0;

        animator.SetBool("IsRunning", isMoving);
    }

    private void ShootBullet()
    {
        GameObject bulletInstance = Instantiate(bullet, bullet.transform.position, bullet.transform.rotation);
        bulletInstance.SetActive(true);

        Rigidbody bulletRb = bulletInstance.GetComponent<Rigidbody>();
        bulletRb.velocity = transform.forward * 10;
    }

    private void SoldierRotation(float horizontal, float vertical)
    {
        if (horizontal != 0 || vertical != 0)
        {
            Vector3 lookDirection = new Vector3(horizontal, 0, vertical);
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10);
        }

        if (!isMoving)
        {
            animator.SetBool("IsRunning", false);
        }
    }
}

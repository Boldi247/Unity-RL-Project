using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MyAgent : Agent
{
    public float speed = 3f;
    public GameObject bulletPrefab;

    private Rigidbody rb;
    private Animator animator;
    private bool isMoving = false;
    private GameObject bullet;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        bullet.SetActive(false);
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        bullet.SetActive(false);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.forward);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveHorizontal = actions.ContinuousActions[0];
        float moveVertical = actions.ContinuousActions[1];
        int shootAction = actions.DiscreteActions[0];

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.velocity = movement * speed;

        isMoving = movement.magnitude > 0;

        SoldierRotation(moveHorizontal, moveVertical);

        animator.SetBool("IsRunning", isMoving);

        if (shootAction == 1)
        {
            ShootBullet();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");

        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void ShootBullet()
    {
        float maxDistance = 100f;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
        {
            Debug.Log("Hit object: " + hit.collider.gameObject.name + ", Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                bullet.SetActive(true);
                bullet.transform.position = transform.position + transform.forward * 2;
                bullet.transform.rotation = transform.rotation;

                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                bulletRb.velocity = transform.forward * 10;


                return;
            }
        }

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

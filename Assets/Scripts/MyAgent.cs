using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MyAgent : Agent
{
    private Animator animator;

    private int enemyCount = 5;
    public int score = 0;
    public float speed = 3f;
    public float rotationSpeed = 3f;

    public Transform shootingPoint;
    public int minStepsBetweenShots = 50;
    public int damage = 100;

    //TODO, add reference to scripts: projectile, enemymanager
    public Projectile projectile;
    public EnemyManager enemyManager;
    //----------------------------------------------

    private bool shotAvailable = true;
    private int stepsUntilShotIsAvailable = 0;

    private Vector3 startingPosition;
    private Rigidbody rb;

    public event Action OnEnvironmentReset;

    public override void OnEpisodeBegin()
    {
        OnEnvironmentReset?.Invoke();

        minStepsBetweenShots = 25;

        transform.position = startingPosition;
        rb.velocity = Vector3.zero;
        shotAvailable = true;
    }

    public void RegisterKill()
    {
        score++;
        AddReward(1.0f / enemyCount);
    }

    private void Shoot()
    {
        if (!shotAvailable)
            return;

        var layerMask = 1 << LayerMask.NameToLayer("enemy");
        var direction = transform.forward;

        var spawnedProjectile = Instantiate(projectile, shootingPoint.position, Quaternion.Euler(0f, -90f, 0f));
        spawnedProjectile.SetDirection(direction);

        Debug.DrawRay(transform.position, direction, Color.blue, 1f);

        if (Physics.Raycast(shootingPoint.position, direction, out var hit, 200f, layerMask))
        {
            hit.transform.GetComponent<Enemy>().GetShot(damage, this);
        }
        else
        {
            AddReward(-0.033f);
        }

        shotAvailable = false;
        stepsUntilShotIsAvailable = minStepsBetweenShots;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(shotAvailable);
    }

    private void FixedUpdate()
    {
        if (!shotAvailable)
        {
            stepsUntilShotIsAvailable--;

            if (stepsUntilShotIsAvailable <= 0)
                shotAvailable = true;
        }

        AddReward(-1f / MaxStep);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionTaken = actions.ContinuousActions;

        if (Mathf.RoundToInt(actionTaken[0]) >= 1)
        {
            Shoot();
        }

        rb.velocity = new Vector3(actionTaken[1] * speed, 0f, actionTaken[2] * speed);
        transform.Rotate(Vector3.up, actionTaken[3] * rotationSpeed);
    }

    public override void Initialize()
    {
        startingPosition = transform.position;
        rb = GetComponent<Rigidbody>();


        rb.freezeRotation = true;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actions = actionsOut.ContinuousActions;

        actions[0] = Input.GetKey(KeyCode.P) ? 1f : 0f;
        actions[2] = Input.GetAxis("Horizontal");
        actions[3] = Input.GetAxis("Vertical");
    }

    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log("Collision detected!" + collision.gameObject.tag);

        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Wall")
        {
            enemyManager.SetEnemiesActive();
            AddReward(-1f);
            EndEpisode();
        }
    }

    //Should delete this method
    private void SoldierRotation(float horizontal, float vertical)
    {
        if (horizontal != 0 || vertical != 0)
        {
            Vector3 lookDirection = new Vector3(horizontal, 0, vertical);
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10);
        }

        /* if (!isMoving)
        {
            animator.SetBool("IsRunning", false);
        } */
    }
}

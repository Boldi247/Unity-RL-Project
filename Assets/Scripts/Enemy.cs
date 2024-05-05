using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int startingHealth = 100;
    public EnemyManager enemyManager;

    private int CurrentHealth;
    private Vector3 StartPosition;

    public float randomRangeX_Pos = 0f;
    public float randomRangeX_Neg = 0f;
    public float randomRangeZ_Pos = 0f;
    public float randomRangeZ_Neg = 0f;

    public MyAgent Agent;

    private void Start()
    {
        StartPosition = transform.position;
        CurrentHealth = startingHealth;

        Agent.OnEnvironmentReset += Respawn;
    }

    public void GetShot(int damage, MyAgent shooter)
    {
        ApplyDamage(damage, shooter);
    }

    private void ApplyDamage(int damage, MyAgent shooter)
    {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            Die(shooter);
        }
    }

    private void Die(MyAgent shooter)
    {
        shooter.RegisterKill();

        gameObject.SetActive(false);
        enemyManager.RegisterDeath();
    }
    public void Respawn()
    {
        CurrentHealth = startingHealth;

        transform.position = new Vector3(StartPosition.x + Random.Range(randomRangeX_Neg, randomRangeX_Pos), StartPosition.y, StartPosition.z + Random.Range(randomRangeZ_Neg, randomRangeZ_Pos));
    }
}

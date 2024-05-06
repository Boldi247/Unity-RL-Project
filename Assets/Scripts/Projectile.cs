using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody Rigidbody;
    private Vector3 dirVector;

    public float bulletSpeed = 20f;

    void SelfDestruct()
    {
        Destroy(this.gameObject);
    }

    public void SetDirection(Vector3 direction)
    {
        dirVector = direction;

        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.velocity = dirVector * bulletSpeed;
    }

    void Start()
    {
        Invoke(nameof(SelfDestruct), 1f);
    }
}

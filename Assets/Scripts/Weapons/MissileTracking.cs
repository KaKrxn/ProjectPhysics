using System.Collections;
using UnityEngine;

public class MissileTracking : MonoBehaviour
{
    [SerializeField] float lifetime = 5f;
    [SerializeField] float speed = 50f;
    [SerializeField] float trackingAngle = 60f;
    [SerializeField] float turningRate = 5f;
    [SerializeField] LayerMask collisionMask;
    [SerializeField] GameObject explosionEffect;

    private Transform owner;
    private Transform target;
    private Rigidbody rb;
    private bool exploded = false;
    private float timer;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        timer = lifetime;
    }

    private void FixedUpdate()
    {
        if (exploded) return;

        timer -= Time.fixedDeltaTime;
        if (timer <= 0) Explode();

        CheckCollision();
        TrackTarget();
        rb.linearVelocity = transform.forward * speed;
    }

    private void CheckCollision()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, speed * Time.fixedDeltaTime, collisionMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Explode();
            }
        }
    }

    private void TrackTarget()
    {
        if (target == null) return;

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        if (angleToTarget > trackingAngle)
        {
            target = null; // เป้าหมายหลุดจากระยะติดตาม
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turningRate * Time.fixedDeltaTime * 100);
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;

        //Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
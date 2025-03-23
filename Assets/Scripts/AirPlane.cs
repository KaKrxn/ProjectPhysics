using System;
using UnityEngine;
using UnityEngine.Serialization;

public class AirPlane : MonoBehaviour
{

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float enginePower = -20f;
    [SerializeField] private float liftBooter = 0.5f;
    [SerializeField] private float drag = 0.001f;
    [SerializeField] float angularDrag = 0.001f;

    [SerializeField] private float yawPower = 50f;
    [FormerlySerializedAs("pitPower")] [SerializeField] private float pitchPower = 50f;
    [SerializeField] private float rollPower = 30f;  
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float yaw = Input.GetAxis("Horizontal") * yawPower;
        float pitch = Input.GetAxis("Vertical") * pitchPower;
        float roll = Input.GetAxis("Roll") * rollPower;
        
        
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(transform.right * enginePower);
            
           // Vector3 Lift = Vector3.Project(rb.linearVelocity, transform.right);
           // rb.AddForce(transform.up * Lift.magnitude * liftBooter);
            
        }
        Debug.Log(enginePower);
        
        rb.linearDamping = rb.linearVelocity.magnitude * drag;
        rb.linearDamping = rb.linearVelocity.magnitude * angularDrag;
        
        rb.AddTorque(transform.up * yaw);
        rb.AddTorque(transform.forward * pitch);
        rb.AddTorque(transform.right * roll);
    }
}

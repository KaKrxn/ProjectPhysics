using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PYController : MonoBehaviour
{
    public GameObject Player;
    public int maxHP = 100;
    private int currentHP;
    public float MoveSpeed;
    public float XRange;
    public float YRange; // เพิ่มระยะ Y
    public Transform firePoint;
    public GameObject projectilePrefab;

    private InputAction moveAction;
    private InputAction shootAction;
    private float horizontalInput;
    private float verticalInput;
    private Coroutine shootingCoroutine;

    [SerializeField] Rigidbody RB;

    private void Awake()
    {
        currentHP = maxHP;
        moveAction = InputSystem.actions.FindAction("Move");
        shootAction = InputSystem.actions.FindAction("Shoot");
        RB = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        shootAction.started += StartShooting;
        shootAction.canceled += StopShooting;
    }

    private void OnDisable()
    {
        shootAction.started -= StartShooting;
        shootAction.canceled -= StopShooting;
    }

    void FixedUpdate()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        horizontalInput = moveInput.x;
        verticalInput = moveInput.y;

        RB.AddForce(horizontalInput * MoveSpeed * Time.deltaTime * Vector3.right);
        RB.AddForce(verticalInput * MoveSpeed * Time.deltaTime * Vector3.down);

        RollShip();
        PitchShip();
        FixYRotation();

        if (transform.position.x < -XRange)
        {
            transform.position = new Vector3(-XRange, transform.position.y, transform.position.z);
            RB.linearVelocity = new Vector3(0, RB.linearVelocity.y, RB.linearVelocity.z);
        }
        if (transform.position.x > XRange)
        {
            transform.position = new Vector3(XRange, transform.position.y, transform.position.z);
            RB.linearVelocity = new Vector3(0, RB.linearVelocity.y, RB.linearVelocity.z);
        }

        if (transform.position.y < -YRange)
        {
            transform.position = new Vector3(transform.position.x, -YRange, transform.position.z);
            RB.linearVelocity = new Vector3(RB.linearVelocity.x, 0, RB.linearVelocity.z);
        }
        if (transform.position.y > YRange)
        {
            transform.position = new Vector3(transform.position.x, YRange, transform.position.z);
            RB.linearVelocity = new Vector3(RB.linearVelocity.x, 0, RB.linearVelocity.z);
        }
    }

    private void StartShooting(InputAction.CallbackContext context)
    {
        if (shootingCoroutine == null)
        {
            shootingCoroutine = StartCoroutine(Shooter());
        }
    }

    private void StopShooting(InputAction.CallbackContext context)
    {
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }

    void RollShip()
    {
        float targetXRotation = horizontalInput * 30f;
        Vector3 currentRotation = transform.rotation.eulerAngles;
        float currentX = currentRotation.x;
        if (currentX > 180) currentX -= 360;
        float newX = Mathf.Lerp(currentX, targetXRotation, Time.deltaTime * 5f);
        transform.rotation = Quaternion.Euler(newX, currentRotation.y, currentRotation.z);
    }

    void PitchShip()
    {
        float targetZRotation = verticalInput * 25f;
        Vector3 currentRotation = transform.rotation.eulerAngles;
        float currentZ = currentRotation.z;
        if (currentZ > 180f) currentZ -= 360f;
        float newZ = Mathf.Lerp(currentZ, targetZRotation, Time.deltaTime * 5f);
        transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, newZ);
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        Debug.Log("Player HP: " + currentHP);
        if (currentHP <= 0)
        {
            Debug.Log("Player Died!");
        }
    }

    void FixYRotation()
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(currentRotation.x, 90f, currentRotation.z);
    }

    void OnCollisionEnter(Collision collision)
    {
        RB.linearVelocity = Vector3.zero;
        Vector3 currentRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);
        Debug.Log("Hit Enemy, reset Force and Rotation.");
        Destroy(this.gameObject);
        Instantiate(Player, transform.position, Player.transform.rotation);
    }

    IEnumerator Shooter()
    {
        while (true)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            yield return new WaitForSeconds(3f);
        }
    }
}

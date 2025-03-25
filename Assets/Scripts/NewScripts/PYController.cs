using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class PYController : MonoBehaviour
{
    public GameObject Player;
    public int maxHP = 100;
    private int currentHP;
    public float MoveSpeed;
    public float XRange;
    public float YRange;

    [Header("Missile System")]
    public Transform firePoint;
    public GameObject missilePrefab;
    public float missileCooldown = 3f;
    private bool canShoot = true;
    private Transform lockedTarget;

    private InputAction moveAction;
    private InputAction shootAction;
    private InputAction lockTargetAction;
    private float horizontalInput;
    private float verticalInput;

    [Header("Text")]
    public TextMeshProUGUI scoreText;
    private int score;

    public Slider HealthBar;
    [SerializeField] Rigidbody RB;

    private void Awake()
    {
        currentHP = maxHP;
        HealthBar.maxValue = maxHP;
        HealthBar.value = currentHP;
        moveAction = InputSystem.actions.FindAction("Move");
        shootAction = InputSystem.actions.FindAction("Shoot");
        lockTargetAction = InputSystem.actions.FindAction("LockTarget");
        RB = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        shootAction.started += ShootMissile;
        lockTargetAction.started += LockTarget;
    }

    private void OnDisable()
    {
        shootAction.started -= ShootMissile;
        lockTargetAction.started -= LockTarget;
    }

    void FixedUpdate()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        horizontalInput = moveInput.x;
        verticalInput = moveInput.y;

        RB.AddForce(horizontalInput * MoveSpeed * Time.deltaTime * Vector3.right);
        RB.AddForce(verticalInput * MoveSpeed * Time.deltaTime * Vector3.down);

        RollShip();  // เพิ่มการหมุนเครื่องบิน
        PitchShip(); // เพิ่มการเอียงเครื่องบิน
        FixYRotation(); // แก้ไขการหมุนในแกน Y

        ClampPosition(); // ตรวจสอบตำแหน่ง

    }

    // ฟังก์ชันที่ใช้ควบคุมการหมุนเครื่องบิน (Roll)
    void RollShip()
    {
        float targetXRotation = horizontalInput * 30f;
        Vector3 currentRotation = transform.rotation.eulerAngles;
        float currentX = currentRotation.x;
        if (currentX > 180) currentX -= 360;
        float newX = Mathf.Lerp(currentX, targetXRotation, Time.deltaTime * 5f);
        transform.rotation = Quaternion.Euler(newX, currentRotation.y, currentRotation.z);
    }

    // ฟังก์ชันที่ใช้ควบคุมการเอียงเครื่องบิน (Pitch)
    void PitchShip()
    {
        // ควบคุมการเอียงเครื่องบินขึ้นลง
        float targetZRotation = verticalInput * 25f;  // 25f คือค่าความเร็วในการเอียง (ปรับตามที่ต้องการ)
        Vector3 currentRotation = transform.rotation.eulerAngles;
        float currentZ = currentRotation.z;
        if (currentZ > 180f) currentZ -= 360f;  // ทำให้ค่าหมุนระหว่าง -180 ถึง 180 องศา
        float newZ = Mathf.Lerp(currentZ, targetZRotation, Time.deltaTime * 5f);  // ใช้ Lerp เพื่อทำให้การเอียงเนียนขึ้น
        transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, newZ);
    }

    // ฟังก์ชันการควบคุมตำแหน่งของเครื่องบิน
    void ClampPosition()
    {
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

    private void LockTarget(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, 100f))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                lockedTarget = hit.transform;
                Debug.Log("Target Locked: " + lockedTarget.name);
            }
        }
    }

    private void ShootMissile(InputAction.CallbackContext context)
    {
        if (canShoot)
        {
            GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
            Missilex missileScript = missile.GetComponent<Missilex>();
            missileScript.SetTarget(lockedTarget);
            StartCoroutine(MissileCooldown());
        }
    }

    IEnumerator MissileCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(missileCooldown);
        canShoot = true;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        HealthBar.value = currentHP;
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
    public void UpdateScore(int score)
    {
        this.score += score;
        scoreText.text = this.score.ToString();

    }






}




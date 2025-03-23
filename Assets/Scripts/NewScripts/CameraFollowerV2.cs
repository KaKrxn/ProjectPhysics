using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// CameraFollowerV2 class is responsible for following the player with a certain offset.
/// It supports Camera view switching:
/// - "C" for Rear View (behind the player)
/// - "V" for Front View (in front of the player)
/// </summary>
public class CameraFollowerV2 : MonoBehaviour
{
    public Transform target; // ตัวละครที่กล้องจะติดตาม
    public Vector3 rearOffset = new Vector3(0, 5, -10); // มุมมองจากด้านหลัง
    public Vector3 frontOffset = new Vector3(0, 3, 5);  // มุมมองจากด้านหน้า
    public float smoothSpeed = 5f; // ความเร็วในการตาม

    private Vector3 currentOffset; // เก็บค่า offset ปัจจุบัน
    private Vector3 velocity = Vector3.zero; // ใช้กับ SmoothDamp

    void Start()
    {
        currentOffset = rearOffset; // เริ่มต้นที่มุมมองด้านหลัง
    }

    void LateUpdate()
    {
        if (target == null) return;

        // กด "C" เพื่อเปลี่ยนไป Rear View
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            currentOffset = rearOffset;
        }

        // กด "V" เพื่อเปลี่ยนไป Front View
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            currentOffset = frontOffset;
        }

        // คำนวณตำแหน่งที่ต้องการให้กล้องไปอยู่
        Vector3 desiredPosition = target.position + target.rotation * currentOffset;

        // ใช้ SmoothDamp เพื่อให้กล้องเคลื่อนที่นุ่มนวล
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed * Time.deltaTime);

        // กล้องหันหน้าตาม Player เสมอ
        transform.LookAt(target);
    }
}

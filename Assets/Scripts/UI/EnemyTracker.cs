using UnityEngine;
using UnityEngine.UI;

public class EnemyTracker : MonoBehaviour
{
    public RectTransform trackingFrame; // กรอบสีเขียว
    public Image targetIndicator;       // วงกลมแดง
    public Image lockOnMarker;          // จุดบนหัวศัตรู
    public Transform enemy;             // เป้าหมาย
    public Camera mainCamera;           // กล้องหลัก
    public Canvas canvas;               // Canvas หลักของ UI

    private bool isTracking = false;

    void Update()
    {
        if (enemy == null || mainCamera == null || canvas == null) return;

        // แปลงตำแหน่งศัตรูจากโลกจริงไปเป็นหน้าจอ
        Vector3 screenPos = mainCamera.WorldToScreenPoint(enemy.position);

        // ตรวจสอบว่าศัตรูอยู่ในมุมมองของกล้องหรือไม่
        if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width &&
            screenPos.y > 0 && screenPos.y < Screen.height)
        {
            Vector2 uiPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(), screenPos, mainCamera, out uiPosition);

            targetIndicator.rectTransform.anchoredPosition = uiPosition;

            // ตรวจสอบว่าศัตรูอยู่ในกรอบ Tracking หรือไม่
            Rect rect = trackingFrame.rect;
            isTracking = uiPosition.x > -rect.width / 2 && uiPosition.x < rect.width / 2 &&
                         uiPosition.y > -rect.height / 2 && uiPosition.y < rect.height / 2;
        }
        else
        {
            isTracking = false;
        }

        // เปิด/ปิด Indicator และ Lock-on Marker ตามสถานะการ Tracking
        targetIndicator.enabled = isTracking;
        lockOnMarker.enabled = isTracking;

        if (isTracking)
        {
            // วาง Marker บนหัวศัตรู
            Vector3 headPosition = enemy.position + Vector3.up * 2;
            Vector3 screenHeadPos = mainCamera.WorldToScreenPoint(headPosition);
            Vector2 uiHeadPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(), screenHeadPos, mainCamera, out uiHeadPosition);

            lockOnMarker.rectTransform.anchoredPosition = uiHeadPosition;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class EnemyTracker : MonoBehaviour
{
    public RectTransform trackingFrame; // ��ͺ������
    public Image targetIndicator;       // ǧ���ᴧ
    public Image lockOnMarker;          // �ش������ѵ��
    public Transform enemy;             // �������
    public Camera mainCamera;           // ���ͧ��ѡ
    public Canvas canvas;               // Canvas ��ѡ�ͧ UI

    private bool isTracking = false;

    void Update()
    {
        if (enemy == null || mainCamera == null || canvas == null) return;

        // �ŧ���˹��ѵ�٨ҡ�š��ԧ���˹�Ҩ�
        Vector3 screenPos = mainCamera.WorldToScreenPoint(enemy.position);

        // ��Ǩ�ͺ����ѵ�����������ͧ�ͧ���ͧ�������
        if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width &&
            screenPos.y > 0 && screenPos.y < Screen.height)
        {
            Vector2 uiPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(), screenPos, mainCamera, out uiPosition);

            targetIndicator.rectTransform.anchoredPosition = uiPosition;

            // ��Ǩ�ͺ����ѵ������㹡�ͺ Tracking �������
            Rect rect = trackingFrame.rect;
            isTracking = uiPosition.x > -rect.width / 2 && uiPosition.x < rect.width / 2 &&
                         uiPosition.y > -rect.height / 2 && uiPosition.y < rect.height / 2;
        }
        else
        {
            isTracking = false;
        }

        // �Դ/�Դ Indicator ��� Lock-on Marker ���ʶҹС�� Tracking
        targetIndicator.enabled = isTracking;
        lockOnMarker.enabled = isTracking;

        if (isTracking)
        {
            // �ҧ Marker ������ѵ��
            Vector3 headPosition = enemy.position + Vector3.up * 2;
            Vector3 screenHeadPos = mainCamera.WorldToScreenPoint(headPosition);
            Vector2 uiHeadPosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(), screenHeadPos, mainCamera, out uiHeadPosition);

            lockOnMarker.rectTransform.anchoredPosition = uiHeadPosition;
        }
    }
}

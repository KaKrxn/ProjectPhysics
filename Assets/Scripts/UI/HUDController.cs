using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public RectTransform pitchIndicator; // UI �ʴ� Pitch
    public RectTransform rollIndicator;  // UI �ʴ� Roll
    public Transform aircraft;           // ��ҧ�ԧ����ͧ�Թ

    void Update()
    {
        if (aircraft == null) return;

        // �֧������ Pitch ��� Roll �ͧ����ͧ�Թ
        float pitch = aircraft.eulerAngles.z;
        float roll = aircraft.eulerAngles.x;

        // ��Ѻ�������㹪�ǧ -180 �֧ 180
        if (pitch > 180) pitch -= 360;
        if (roll > 180) roll -= 360;

        // �ѻവ UI
        pitchIndicator.anchoredPosition = new Vector2(0, pitch * 2); // ���-ŧ
        rollIndicator.localRotation = Quaternion.Euler(0, 0, -roll); // ��ع����-���
    }
}
  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    public static float SAFE_AREA_HEIGHT;
    void Awake()
    {
        // �ػ� ���� 
        Screen.SetResolution(720, GetScreenHeight(), true);

        // ������ ���� ����
        RectTransform rt = GetComponent<RectTransform>();
        Rect safeArea = Screen.safeArea;
        Vector2 minAnchor = safeArea.position;
        Vector2 maxAnchor = minAnchor + safeArea.size;

        minAnchor.x = rt.anchorMin.x;
        maxAnchor.x = rt.anchorMax.x;

        minAnchor.y /= Screen.height;
        maxAnchor.y /= Screen.height;

        rt.anchorMin = minAnchor;
        rt.anchorMax = maxAnchor;
        SAFE_AREA_HEIGHT = rt.rect.height;
    }
    static public int GetScreenHeight()
    {
        float ratioWdith = Display.main.systemHeight * 720.0f / Display.main.systemWidth;
        return (int)ratioWdith;
    }

}

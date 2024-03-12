using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
public class UIDefine : MonoBehaviour
{

    public enum Lang_Kind : int
    {

        Kor = 0,
        Eng = 1,
        Count = 2,
    }

/// <summary>
///  ���� ������ ������ ���� �����Ǹ�, �ش� ��ũ��Ʈ�� �̵�
///  ���� �� ���� ���� �������� ��� ��� �ϰ� ���� -> ���� ���� �ɷ� ����� �ܼ� ���׷��̵�� ����, ���� ��ȭ�� ���� ���
/// </summary>
    public enum ItemType : int
    {

        Material = 0, // ������ ���
        SkillCard = 1,
        Potion = 2,// ����
        Count = 3,// 

    }


    static public string POINT_WITH_K_M(int point, bool withComma = true)
    {
        if (point >= 10000000)
        {
            return string.Format("{0}M", GetPoint(Mathf.FloorToInt(point / 1000000f), withComma));
        }
        else if (point >= 10000)
        {
            return string.Format("{0}K", GetPoint(Mathf.FloorToInt(point / 1000f), withComma));
        }
        else
            return GetPoint(point, withComma);
    }

    private static string GetPoint(int point, bool withComma)
    {
        if (withComma)
            return point.ToString("N0");
        else
            return point.ToString();
    }

}

public enum UNIT_STAT_TYPE
{
    None = 0,
    Normal_Slow = 1,
    Normal_Fast = 2,
    Normal_Heavy = 3,
    Middle_Slow = 11,
    Middle_Fast = 12,
    Middle_Heavy = 13,
    Boss_Slow = 21,
    Boss_Fast = 22,
    Boss_Heavy = 23,
}
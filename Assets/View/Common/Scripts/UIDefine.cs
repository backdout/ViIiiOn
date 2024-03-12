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
///  추후 아이템 데이터 관련 정리되면, 해당 스크립트로 이동
///  무기 및 장착 관련 아이템은 잠시 고민 하고 진행 -> 현재 영웅 능력 상승을 단순 업그레이드로 할지, 무기 강화로 갈지 고민
/// </summary>
    public enum ItemType : int
    {

        Material = 0, // 아이템 재료
        SkillCard = 1,
        Potion = 2,// 포션
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
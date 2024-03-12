
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SkillTreeDataScript
{
    //readFile
    //https://docs.google.com/spreadsheets/d/1Dge4oQps4UnvPqQauBC9lFqqSehX_CY7ET_FgWzpe3U/edit?pli=1#gid=265026986
    private static Dictionary<int, SkillTreeDataScript> SKILL_RED_LIST;
    private static Dictionary<int, SkillTreeDataScript> SKILL_BLUE_LIST;
    private static Dictionary<int, SkillTreeDataScript> SKILL_GREEN_LIST;


    public enum SKILL_TREE_TYPE :int
    {
        RED = 0,
        BLUE = 1,
        GREEN = 2,
    }

    public const int MAX_SPAWN = 17;
    public static string GET_SCRIPT_NAME()
    {
        return "SKILL";
    }
    public static bool IS_INIT()
    {
        if (SKILL_RED_LIST == null)
            return false;

        return true;
    }
    public static bool INIT()
    {

        try
        {
            var dataScript = GoogleSheetsManager.Instance.CSVDatas[GoogleSheetsManager.DATA_TYPE.SKILL];

            if (string.IsNullOrEmpty(dataScript))
                throw new Exception("SkillTreeDataScript, CSV DATA NULL");

            SKILL_RED_LIST = new Dictionary<int, SkillTreeDataScript>();
            SKILL_BLUE_LIST = new Dictionary<int, SkillTreeDataScript>();
            SKILL_GREEN_LIST = new Dictionary<int, SkillTreeDataScript>();


            string[] str = ReadScript.readFile(dataScript, true);

            foreach (string target in str)
            {
                if (target.Length <= 0)
                    continue;

                //string[] split = target.Split(',');
                string[] split = CSVUtil.Split(target);

                SkillTreeDataScript Script = new SkillTreeDataScript(split);

                switch (Script.Type)
                {
                    case SKILL_TREE_TYPE.RED:
                        {
                            if (SKILL_RED_LIST.ContainsKey(Script.Id) == true)
                            {
                                SKILL_RED_LIST = null;

                                throw new Exception("SkillTreeDataScript, ContainsKey: " + Script.Id);
                            }

                            SKILL_RED_LIST.Add(Script.Id, Script);
                        }
                        break;
                    case SKILL_TREE_TYPE.BLUE:
                        {
                            if (SKILL_BLUE_LIST.ContainsKey(Script.Id) == true)
                            {
                                SKILL_RED_LIST = null;
                                SKILL_BLUE_LIST = null;

                                throw new Exception("SkillTreeDataScript, ContainsKey: " + Script.Id);
                            }

                            SKILL_BLUE_LIST.Add(Script.Id, Script);
                        }
                        break;
                    case SKILL_TREE_TYPE.GREEN:
                        {
                            if (SKILL_GREEN_LIST.ContainsKey(Script.Id) == true)
                            {
                                SKILL_RED_LIST = null;
                                SKILL_BLUE_LIST = null;
                                SKILL_GREEN_LIST = null;

                                throw new Exception("SkillTreeDataScript, ContainsKey: " + Script.Id);
                            }

                            SKILL_GREEN_LIST.Add(Script.Id, Script);
                        }
                        break;
                }

               

            }


        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError("[Message]\n" + e.Message + "\n[StackTrace]\n" + e.StackTrace);
#endif
            SKILL_RED_LIST = null;
            SKILL_BLUE_LIST = null;
            SKILL_GREEN_LIST = null;
            return false;
        }

        return true;
    }


    public static List<SkillTreeDataScript> GET_DATA_LIST(SKILL_TREE_TYPE type)
    {
        switch(type)
        {
            case SKILL_TREE_TYPE.RED:
                return SKILL_RED_LIST.Values.ToList();
            case SKILL_TREE_TYPE.BLUE:
                return SKILL_BLUE_LIST.Values.ToList();
            case SKILL_TREE_TYPE.GREEN:
                return SKILL_GREEN_LIST.Values.ToList();

             default: return null;
        }
     
    }
    public static SkillTreeDataScript GET_DATA(SKILL_TREE_TYPE type, int ID)
    {

        switch (type)
        {
            case SKILL_TREE_TYPE.RED:
                {
                    if (SKILL_RED_LIST.ContainsKey(ID) == false)
                        return null;

                    return SKILL_RED_LIST[ID];
                }
            case SKILL_TREE_TYPE.BLUE:
                {
                    if (SKILL_BLUE_LIST.ContainsKey(ID) == false)
                        return null;

                    return SKILL_BLUE_LIST[ID];
                }
            case SKILL_TREE_TYPE.GREEN:
                {
                    if (SKILL_GREEN_LIST.ContainsKey(ID) == false)
                        return null;

                    return SKILL_GREEN_LIST[ID];
                }
            default: return null;
        }

    }

    #region 변수 


    /// <summary>
    /// 타입. 레드는 공격관련/ 블루는 방어 이동/ 그린은 부활 체력 관련 버프
    /// </summary>
    public SKILL_TREE_TYPE Type { get; private set; }

    /// <summary>
    ///skill ID 
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// 오픈 조건 아이디 
    /// </summary>
    public int Before_id { get; private set; }

    /// <summary>
    /// 구매 가격
    /// </summary>
    public int Price { get; private set; }
    /// <summary>
    /// 스킬에 적용되는 실제 버프 아이디
    /// </summary>
    public int BuffId { get; private set; }

    /// <summary>
    /// 버프에 적용되는 값
    /// </summary>
    public int Value { get; private set; }


    public string desc_kor { get; private set; }

 
    public string desc_en { get; private set; }


    #endregion


    public SkillTreeDataScript(string[] arr)
    {
        int arrIndex = 0;
        //raw 는 레드 블루 그린 순으로 되어 있음으로 단순 합산으로 처리 
        int raw = SKILL_RED_LIST.Count + SKILL_BLUE_LIST.Count + SKILL_GREEN_LIST.Count;
        ScriptManager.FieldInfo fi = new ScriptManager.FieldInfo(GET_SCRIPT_NAME(), raw, arrIndex);
        var tyep = ScriptManager.ParseString(fi.SetColumn(arrIndex), arr[arrIndex++], 1024);
        Type = tyep == "red" ? SKILL_TREE_TYPE.RED : tyep == "blue" ? SKILL_TREE_TYPE.BLUE : SKILL_TREE_TYPE.GREEN;
        Id = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 9999);
        Before_id = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 999);
        Price = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 9999);

        BuffId = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 9999);
        Value = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 9999);

        desc_kor = ScriptManager.ParseString(fi.SetColumn(arrIndex), arr[arrIndex++], 1024);

        desc_en = ScriptManager.ParseString(fi.SetColumn(arrIndex), arr[arrIndex++], 1024);

    }

  

}

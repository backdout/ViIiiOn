using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMobDataScript 
{
    //readFile
 
    private static Dictionary<int, List<StageMobDataScript>> STAGE_MOB_LIST;

    public const int MAX_SPAWN = 17;
    public static string GET_SCRIPT_NAME()
    {
        return "STAGE_MOB_DATA";
    }
    public static bool IS_INIT()
    {
        if (STAGE_MOB_LIST == null)
            return false;

        return true;
    }
    public static bool INIT()
    {
     
        try
        {
            var dataScript = GoogleSheetsManager.Instance.CSVDatas[GoogleSheetsManager.DATA_TYPE.STAGE_MOB];

            if(string.IsNullOrEmpty(dataScript))
                throw new Exception("StageMobDataScript, CSV DATA NULL");

            STAGE_MOB_LIST = new Dictionary<int, List<StageMobDataScript>>();
              

            string[] str = ReadScript.readFile(dataScript, true);

            foreach (string target in str)
            {
                if (target.Length <= 0)
                    continue;

                //string[] split = target.Split(',');
                string[] split = CSVUtil.Split(target);

                StageMobDataScript Script = new StageMobDataScript(split);

                if (STAGE_MOB_LIST.ContainsKey(Script.stage) == false)
                {
                    STAGE_MOB_LIST.Add(Script.stage, new List<StageMobDataScript>());
                  
                    //STAGE_MOB_LIST = null;

                    //throw new Exception("StageMobDataScript, ContainsKey: " + Script.stage);
                }
          
                STAGE_MOB_LIST[Script.stage].Add(Script);

            
            }
            // 시간 별로 정리 
            foreach (var data in STAGE_MOB_LIST)
            {
                data.Value.Sort((x,y)=> x.time.CompareTo(y.time));  
            }

        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError("[Message]\n" + e.Message + "\n[StackTrace]\n" + e.StackTrace);
#endif
            STAGE_MOB_LIST = null;

            return false;
        }

        return true;
    }

    public static List<StageMobDataScript> GET_DATA(int stageNum)
    {
        if (STAGE_MOB_LIST.ContainsKey(stageNum) == false)
            return null;

        return STAGE_MOB_LIST[stageNum];

    }
    #region 변수 
    

    /// <summary>
    /// 출력될 스테이지
    /// </summary>
    public int stage { get; private set; }
    /// <summary>
    /// 출력될 시간 0.1초 단위
    /// </summary>
    public float time { get; private set; }
    /// <summary>
    /// 동그라미형 : 1/ 네모형 2
    /// </summary>
    public int type { get; private set; }
    /// <summary>
    /// 몬스터가 스폰될 곳/ 0: 미출력 1~ 몬스터 넘버
    /// </summary>
    public int[] spowns { get; private set; }


    #endregion


    public StageMobDataScript(string[] arr)
    {
        int arrIndex = 0;

        ScriptManager.FieldInfo fi = new ScriptManager.FieldInfo(GET_SCRIPT_NAME(), STAGE_MOB_LIST.Count, arrIndex);

        stage = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 9999);
        time = ScriptManager.ParseFloat(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 9999f);
        type = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 9999);

        spowns = new int[MAX_SPAWN];
        for (int i = 0; i < MAX_SPAWN; i++)
            spowns[i] = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 9999);
     
        //spown_16 = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 9999);

    }

}

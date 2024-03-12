using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;



//"https://docs.google.com/spreadsheets/d/1zamPm6LkIy_qrfvou5OKykqD5NaOYg-hrCDB0_IkD4s/export?fomat=csv";
public class GoogleSheetsManager : Singleton<GoogleSheetsManager>
{

    const string GOOGLE_SHEETS_URL = "https://docs.google.com/spreadsheets/d/1Dge4oQps4UnvPqQauBC9lFqqSehX_CY7ET_FgWzpe3U/gviz/tq?tqx=out:csv";
   
    
    const string STAGE_MOB_URL = "&gid=77812390&range=A2:T";
    const string MOB_STAT_URL = "&gid=977385032&range=A2:T";
    const string ITEM_URL = "&gid=2135326456&range=A2:T";
    const string SKILL_URL = "&gid=265026986&range=A2:T";


    private bool IS_GOOGLESHEETS_LOAD = false;
    public bool isGoogleSheetsLoad()
    {

        return IS_GOOGLESHEETS_LOAD;
    }


    public enum DATA_TYPE
    {
        STAGE_MOB = 0,
        MOB_STAT = 1,
        ITEM = 2,
        SKILL = 3,
        COUNT = 4
    }


    private string GetScriptName(DATA_TYPE type)
    {
        switch (type)
        {
            case DATA_TYPE.STAGE_MOB:
                return STAGE_MOB_URL;
            case DATA_TYPE.MOB_STAT:
                return MOB_STAT_URL;
            case DATA_TYPE.ITEM:
                return ITEM_URL;
            case DATA_TYPE.SKILL:
                return SKILL_URL;
            default:
                return "";
        }

    }

    public Dictionary<DATA_TYPE, string> CSVDatas { get; private set; }

    public IEnumerator ALL_REQUEST()
    {
        IS_GOOGLESHEETS_LOAD = false; 
        CSVDatas = new Dictionary<DATA_TYPE,string>();

        for (int i = 0; i < (int) DATA_TYPE.COUNT; i++)
            StartCoroutine(REQUEST((DATA_TYPE)i));

        yield return new WaitUntil(() => CSVDatas.Count >= (int)DATA_TYPE.COUNT);

        IS_GOOGLESHEETS_LOAD = true;
    }



    // 실패일때 예외처리 추후 추가 
    private IEnumerator REQUEST(DATA_TYPE scriptName)
    {
        UnityWebRequest www = UnityWebRequest.Get(GOOGLE_SHEETS_URL + GetScriptName(scriptName));
        yield return www.SendWebRequest();

        
        string data = www.downloadHandler.text;

       // Debug.Log(data);
        CSVDatas.Add(scriptName, data);
    }





}

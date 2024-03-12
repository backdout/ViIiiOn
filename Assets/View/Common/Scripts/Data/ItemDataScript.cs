using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class ItemDataScript
{
    //readFile
    //https://docs.google.com/spreadsheets/d/1Dge4oQps4UnvPqQauBC9lFqqSehX_CY7ET_FgWzpe3U/edit?pli=1#gid=2135326456
    private static Dictionary<int, ItemDataScript> ITEM_LIST;


    public const int MAX_SPAWN = 17;
    public static string GET_SCRIPT_NAME()
    {
        return "ITEM";
    }
    public static bool IS_INIT()
    {
        if (ITEM_LIST == null)
            return false;

        return true;
    }
    public static bool INIT()
    {

        try
        {
            var dataScript = GoogleSheetsManager.Instance.CSVDatas[GoogleSheetsManager.DATA_TYPE.ITEM];

            if (string.IsNullOrEmpty(dataScript))
                throw new Exception("ItemDataScript, CSV DATA NULL");

            ITEM_LIST = new Dictionary<int, ItemDataScript>();
        
            string[] str = ReadScript.readFile(dataScript, true);

            foreach (string target in str)
            {
                if (target.Length <= 0)
                    continue;

                //string[] split = target.Split(',');
                string[] split = CSVUtil.Split(target);

                ItemDataScript Script = new ItemDataScript(split);

                if (ITEM_LIST.ContainsKey(Script.id) == true)
                {
                    ITEM_LIST = null;

                    throw new Exception("ItemDataScript, ContainsKey: " + Script.id);
                }

                ITEM_LIST.Add(Script.id, Script);


            }

        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError("[Message]\n" + e.Message + "\n[StackTrace]\n" + e.StackTrace);
#endif
            ITEM_LIST = null;
      
            return false;
        }

        return true;
    }


    public static List<ItemDataScript> GET_DATA_LIST()
    {
        return ITEM_LIST.Values.ToList();

    }
    public static ItemDataScript GET_DATA(int ID)
    {

        if (ITEM_LIST.ContainsKey(ID) == false)
            return null;

        return ITEM_LIST[ID];
    }

    #region º¯¼ö 

    public int id { get; private set; }
    public UIDefine.ItemType type { get; private set; }

    public int buyPrice { get; private set; }
    public int sellPrice { get; private set; }

    public string namekor { get; private set; }

    public string nameEn { get; private set; }

    public string desckor { get; private set; }

    public string descEn { get; private set; }



    #endregion


    public ItemDataScript(string[] arr)
    {
        int arrIndex = 0;
        
        ScriptManager.FieldInfo fi = new ScriptManager.FieldInfo(GET_SCRIPT_NAME(), ITEM_LIST.Count, arrIndex);
        
        id = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 9999);
        type = (UIDefine.ItemType) ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 9999);
        buyPrice = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 999999);
        sellPrice = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 999999);

        namekor = ScriptManager.ParseString(fi.SetColumn(arrIndex), arr[arrIndex++], 1024);

        nameEn = ScriptManager.ParseString(fi.SetColumn(arrIndex), arr[arrIndex++], 1024);
        desckor = ScriptManager.ParseString(fi.SetColumn(arrIndex), arr[arrIndex++], 1024);

        descEn = ScriptManager.ParseString(fi.SetColumn(arrIndex), arr[arrIndex++], 1024);
    }



}

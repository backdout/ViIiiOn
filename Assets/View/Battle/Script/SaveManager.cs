using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;

public class SaveManager : MonoBehaviour
{



    private const string LoadPath = "Data/"; //
    private const string SavePath = "Assets/A-Assets/Common/Scripts/Resources/Data/";

    // 읽어오는 파일 이름 ===================================
    private const string UnitFileName = "unitstat";
    private const string ItemStatFileName = "itemstat";
    private const string ItemDataFileName = "item";
    private const string ItemOptionDataFileName = "itemoption";
    private const string SkillDataFileName = "skill";
    //==============================================================================
    [Serializable]
    public class UnitStatSaveDataList
    {
        public List<UnitStatSaveData> UnitStatDatas;
    }
    [Serializable]
    public class ItemStatSaveDataList
    {
        public List<ItemStatSaveData> ItemStatDatas;
    }


    //[Serializable]
    //public class ItemDataList
    //{
    //    public List<ItemScript> ItemDatas;
    //}


    //[Serializable]
    //public class OptionDataList
    //{
    //    public List<ItemOptionScript> ItemOptionDatas;
    //}


    [Serializable]
    public class UnitStatSaveData
    {
        public UNIT_STAT_TYPE UnitStatType;
        public int Hp;
        public int Atk;
        public int Def;
        public int Critical;
        public float CriticalDamage;
        public int MoveSpeed;
        public float AtkSpeed;
        public float KnockbackValue;

        public UnitStatSaveData() { }
        public UnitStatSaveData(UNIT_STAT_TYPE _UnitStatType, int _Hp, int _Atk, int _Def, int _Critical, float _CriticalDamage, int _MoveSpeed)
        {
            UnitStatType = _UnitStatType;
            Hp = _Hp;
            Atk = _Atk;
            Def = _Def;
            Critical = _Critical;
            CriticalDamage = _CriticalDamage;
            MoveSpeed = _MoveSpeed;  
        }
    }

    [Serializable]
    public class ItemStatSaveData
    {
        public int ItemStatType;
        public bool IsAttackStat;
        public int MinStat;
        public int MaxStat;

        public ItemStatSaveData() { }
        public ItemStatSaveData(int _ItemStatType, bool _IsAttackStat, int _MinStat, int _MaxStat)
        {
            ItemStatType = _ItemStatType;
            IsAttackStat = _IsAttackStat;
            MinStat = _MinStat;
            MaxStat = _MaxStat;
        }
    }



#if UNITY_EDITOR

    #region Save
    public static void UnitStatSave(UnitStatSaveData saveData)
    {
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);

        }

        UnitStatSaveDataList data = UnitStatLoad();
        List<UnitStatSaveData> UnitStat = new List<UnitStatSaveData>();
        if (data == null)
        {
            data = new UnitStatSaveDataList();
        }
        else
        {
            UnitStat = data.UnitStatDatas;
        }

        bool isAdd = false;
        for (int i = 0; i < UnitStat.Count; i++)
        {
            if (UnitStat[i].UnitStatType == saveData.UnitStatType)
            {
                UnitStat[i] = saveData;
                isAdd = true;
                break;
            }
        }

        if (isAdd == false)
        {
            UnitStat.Add(saveData);
        }

        data.UnitStatDatas = UnitStat;
        string saveJson = JsonUtility.ToJson(data, true);

        string saveFilePath = SavePath + UnitFileName + ".json";
        File.WriteAllText(saveFilePath, saveJson);
        //AssetDatabase.Refresh();
        Debug.Log("Save Success: " + saveFilePath);
    }
    public static void ItemStatSave(ItemStatSaveData saveData)
    {
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);

        }

        ItemStatSaveDataList data = ItemStatLoad();
        List<ItemStatSaveData> ItemStat = new List<ItemStatSaveData>();
        if (data == null)
        {
            data = new ItemStatSaveDataList();
        }
        else
        {
            ItemStat = data.ItemStatDatas;
        }

        bool isAdd = false;
        for (int i = 0; i < ItemStat.Count; i++)
        {
            if (ItemStat[i].ItemStatType == saveData.ItemStatType)
            {
                ItemStat[i] = saveData;
                isAdd = true;
                break;
            }
        }

        if (isAdd == false)
        {
            ItemStat.Add(saveData);
        }

        ItemStat.Sort((x, y) => x.ItemStatType.CompareTo(y.ItemStatType));


        data.ItemStatDatas = ItemStat;
        string saveJson = JsonUtility.ToJson(data, true);

        string saveFilePath = SavePath + ItemStatFileName + ".json";
        File.WriteAllText(saveFilePath, saveJson);
        Debug.Log("Save Success: " + saveFilePath);
        //AssetDatabase.Refresh();
    }


    public static void ItemStatSave(ItemStatSaveData[] saveData)
    {
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }

        ItemStatSaveDataList data = new ItemStatSaveDataList();
        List<ItemStatSaveData> ItemStat = new List<ItemStatSaveData>();
        for (int i = 0; i < saveData.Length; i++)
        {
            if (saveData[i].ItemStatType != 0)
                ItemStat.Add(saveData[i]);
        }
        ItemStat.Sort((x, y) => x.ItemStatType.CompareTo(y.ItemStatType));
        data.ItemStatDatas = ItemStat;
        string saveJson = JsonUtility.ToJson(data, true);

        string saveFilePath = SavePath + ItemStatFileName + ".json";
        File.WriteAllText(saveFilePath, saveJson);
        Debug.Log("Save Success: " + saveFilePath);
        //AssetDatabase.Refresh();
    }


    //public static void ItemDataSave(ItemScript saveData)
    //{
    //    if (!Directory.Exists(SavePath))
    //    {
    //        Directory.CreateDirectory(SavePath);

    //    }

    //    ItemDataList data = ItemDataLoad();
    //    List<ItemScript> ItemDatas = new List<ItemScript>();
    //    if (data == null)
    //    {
    //        data = new ItemDataList();
    //    }
    //    else
    //    {
    //        ItemDatas = data.ItemDatas;
    //    }

    //    bool isAdd = false;
    //    for (int i = 0; i < ItemDatas.Count; i++)
    //    {
    //        if (ItemDatas[i].ID == saveData.ID)
    //        {
    //            ItemDatas[i] = saveData;
    //            isAdd = true;
    //            break;
    //        }
    //    }

    //    if (isAdd == false)
    //    {
    //        ItemDatas.Add(saveData);
    //    }

    //    ItemDatas.Sort((x, y) => x.ID.CompareTo(y.ID));
    //    data.ItemDatas = ItemDatas;
    //    string saveJson = JsonUtility.ToJson(data, true);

    //    string saveFilePath = SavePath + ItemDataFileName + ".json";
    //    File.WriteAllText(saveFilePath, saveJson);
    //    Debug.Log("Save Success: " + saveFilePath);
    //    //AssetDatabase.Refresh();
    //}

    //public static void ItemDataSave(ItemScript[] saveData)
    //{
    //    if (!Directory.Exists(SavePath))
    //    {
    //        Directory.CreateDirectory(SavePath);
    //    }

    //    ItemDataList data = new ItemDataList();
    //    List<ItemScript> ItemDatas = new List<ItemScript>();
    //    for (int i = 0; i < saveData.Length; i++)
    //    {
    //        if (saveData[i].ID != 0)
    //            ItemDatas.Add(saveData[i]);
    //    }

    //    ItemDatas.Sort((x, y) => x.ID.CompareTo(y.ID));
    //    data.ItemDatas = ItemDatas;
    //    string saveJson = JsonUtility.ToJson(data, true);

    //    string saveFilePath = SavePath + ItemDataFileName + ".json";
    //    File.WriteAllText(saveFilePath, saveJson);
    //    Debug.Log("Save Success: " + saveFilePath);
    //    //AssetDatabase.Refresh();

    //}


    //public static void OptionDataSave(ItemOptionScript saveData)
    //{
    //    if (!Directory.Exists(SavePath))
    //    {
    //        Directory.CreateDirectory(SavePath);

    //    }

    //    OptionDataList data = OptionDataLoad();
    //    List<ItemOptionScript> options = new List<ItemOptionScript>();
    //    if (data == null)
    //    {
    //        data = new OptionDataList();
    //    }
    //    else
    //    {
    //        options = data.ItemOptionDatas;
    //    }

    //    bool isAdd = false;
    //    for (int i = 0; i < options.Count; i++)
    //    {
    //        if (options[i].OptionID == saveData.OptionID)
    //        {
    //            options[i] = saveData;
    //            isAdd = true;
    //            break;
    //        }
    //    }

    //    if (isAdd == false)
    //    {
    //        options.Add(saveData);
    //    }
    //    options.Sort((x, y) => x.OptionID.CompareTo(y.OptionID));
    //    data.ItemOptionDatas = options;
    //    string saveJson = JsonUtility.ToJson(data, true);

    //    string saveFilePath = SavePath + ItemOptionDataFileName + ".json";
    //    File.WriteAllText(saveFilePath, saveJson);
    //    Debug.Log("Save Success: " + saveFilePath);
    //    //AssetDatabase.Refresh();
    //}

    //public static void OptionDataSave(ItemOptionScript[] saveData)
    //{
    //    if (!Directory.Exists(SavePath))
    //    {
    //        Directory.CreateDirectory(SavePath);
    //    }

    //    OptionDataList data = new OptionDataList();
    //    List<ItemOptionScript> options = new List<ItemOptionScript>();
    //    for (int i = 0; i < saveData.Length; i++)
    //    {
    //        if (saveData[i].OptionID != 0)
    //            options.Add(saveData[i]);
    //    }
    //    options.Sort((x, y) => x.OptionID.CompareTo(y.OptionID));
    //    data.ItemOptionDatas = options;
    //    string saveJson = JsonUtility.ToJson(data, true);

    //    string saveFilePath = SavePath + ItemOptionDataFileName + ".json";
    //    File.WriteAllText(saveFilePath, saveJson);
    //    Debug.Log("Save Success: " + saveFilePath);
    //    //AssetDatabase.Refresh();
    //}




    #endregion

#endif

    #region Load
    public static UnitStatSaveDataList UnitStatLoad()
    {
        string saveFilePath = LoadPath + UnitFileName;


        var saveFile = Resources.Load<TextAsset>(saveFilePath);
        if (saveFile == null)
        {
            Debug.Log("No such UnitStatLoad exists");
            return null;
        }

        //	string saveFile = File.ReadAllText(saveFilePath);
        UnitStatSaveDataList saveData = JsonUtility.FromJson<UnitStatSaveDataList>(saveFile.ToString());
        return saveData;
    }

    public static ItemStatSaveDataList ItemStatLoad()
    {
        string saveFilePath = LoadPath + ItemStatFileName;
        var saveFile = Resources.Load<TextAsset>(saveFilePath);
        if (saveFile == null)
        {
            Debug.Log("No such ItemStatLoad exists");
            return null;
        }
        ItemStatSaveDataList saveData = JsonUtility.FromJson<ItemStatSaveDataList>(saveFile.ToString());
        return saveData;
    }

    //public static ItemDataList ItemDataLoad()
    //{
    //    string saveFilePath = LoadPath + ItemDataFileName;

    //    var saveFile = Resources.Load<TextAsset>(saveFilePath);
    //    if (saveFile == null)
    //    {
    //        Debug.Log("No such ItemLoad exists");
    //        return null;
    //    }

    //    ItemDataList saveData = JsonUtility.FromJson<ItemDataList>(saveFile.ToString());
    //    return saveData;
    //}
    //public static OptionDataList OptionDataLoad()
    //{
    //    string saveFilePath = LoadPath + ItemOptionDataFileName;

    //    var saveFile = Resources.Load<TextAsset>(saveFilePath);
    //    if (saveFile == null)
    //    {
    //        Debug.Log("No such OptionLoad exists");
    //        return null;
    //    }

    //    OptionDataList saveData = JsonUtility.FromJson<OptionDataList>(saveFile.ToString());
    //    return saveData;
    //}

   

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Security.Cryptography;

public class MobStatDataScript
{
    //readFile
    //https://docs.google.com/spreadsheets/d/1Dge4oQps4UnvPqQauBC9lFqqSehX_CY7ET_FgWzpe3U/edit?pli=1#gid=977385032
    private static Dictionary<int, MobStatDataScript> MOB_STAT_LIST;

    public const int MAX_SPAWN = 17;
    public static string GET_SCRIPT_NAME()
    {
        return "MOB_STAT";
    }
    public static bool IS_INIT()
    {
        if (MOB_STAT_LIST == null)
            return false;

        return true;
    }
    public static bool INIT()
    {

        try
        {
            var dataScript = GoogleSheetsManager.Instance.CSVDatas[GoogleSheetsManager.DATA_TYPE.MOB_STAT];

            if (string.IsNullOrEmpty(dataScript))
                throw new Exception("MobStatDataScript, CSV DATA NULL");

            MOB_STAT_LIST = new Dictionary<int, MobStatDataScript>();


            string[] str = ReadScript.readFile(dataScript, true);

            foreach (string target in str)
            {
                if (target.Length <= 0)
                    continue;

                //string[] split = target.Split(',');
                string[] split = CSVUtil.Split(target);

                MobStatDataScript Script = new MobStatDataScript(split);

                if (MOB_STAT_LIST.ContainsKey(Script.Id)==true)
                {
                    MOB_STAT_LIST = null;

                    throw new Exception("MobStatDataScript, ContainsKey: " + Script.Id);
                }

                MOB_STAT_LIST.Add(Script.Id, Script) ;
            
            }
          
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError("[Message]\n" + e.Message + "\n[StackTrace]\n" + e.StackTrace);
#endif
            MOB_STAT_LIST = null;

            return false;
        }

        return true;
    }


    public static List<MobStatDataScript> GET_DATA_LIST()
    {
        return MOB_STAT_LIST.Values.ToList();
    }

    public static MobStatDataScript GET_DATA(int ID)
    {
        if (MOB_STAT_LIST.ContainsKey(ID) == false)
            return null;

        return MOB_STAT_LIST[ID];
    }
    /// <summary>
    /// é�ͳѹ� + ID  �� ���� ������ �����ö� ���
    /// </summary>
    /// <param name="chapterNum"></param>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static MobStatDataScript GET_DATA(int chapterNum , int ID)
    {
        if (MOB_STAT_LIST.ContainsKey(chapterNum+ID) == false)
            return null;

        return MOB_STAT_LIST[chapterNum + ID];
    }

    #region ���� 


    /// <summary>
    /// �� ID / ���� �ڸ��� é��, 1�� �ڸ��� ���̵�  
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// ü��
    /// </summary>
    public int Hp { get; private set; }
    /// <summary>
    /// ���ݷ�
    /// </summary>
    public int Atk { get; private set; }
    /// <summary>
    /// ����
    /// </summary>
    public int Def { get; private set; }

    /// <summary>
    /// ũ��Ƽ�� Ȯ��
    /// </summary>
    public int Critical { get; private set; }

    /// <summary>
    /// ũ��Ƽ�� ������ �ۼ�Ʈ
    /// </summary>
    public float CriticalDamage { get; private set; }

    /// <summary>
    /// �̵� �ӵ�
    /// </summary>
    public int MoveSpeed { get; private set; }
    /// <summary>
    /// ��� �� Ȯ��
    /// </summary>
    public int[] DropRateGem { get; private set; }

    #endregion


    public MobStatDataScript(string[] arr)
    {
        int arrIndex = 0;

        ScriptManager.FieldInfo fi = new ScriptManager.FieldInfo(GET_SCRIPT_NAME(), MOB_STAT_LIST.Count, arrIndex);

        Id = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 9999);
        Hp = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 999);
        Atk = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 9999);

        Def = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 9999);
        Critical = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 9999);
        CriticalDamage = ScriptManager.ParseFloat(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 10.0f);

        MoveSpeed = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 9999);
        DropRateGem = new int[3];
        
        for( int i = 0; i < DropRateGem.Length; i++)
            DropRateGem[i] = ScriptManager.ParseInt(fi.SetColumn(arrIndex), arr[arrIndex++], 0, 9999);
    }

    /// <summary>
    /// �ش� �Լ��� StatBase���� ����ϱ����� ������
    /// </summary>
    /// <param name="id"></param>
    /// <param name=""></param>
    public MobStatDataScript(int id, int hp, int atk, int def, int critical, float criticalDamage, int moveSpeed, int[] dropRateGem)
    {
        Id = id;
        Hp = hp;
        Atk = atk;
        Def = def;
        Critical = critical; 
        CriticalDamage = criticalDamage;
        MoveSpeed = moveSpeed;

        DropRateGem = dropRateGem;

       
    }

}

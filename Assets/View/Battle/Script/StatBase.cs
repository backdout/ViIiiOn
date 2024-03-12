
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.Text;
using System.Linq;
using System;

public class StatBase : MonoBehaviour
{


    private static Dictionary<int, MobStatDataScript> BASE_STAT;

    [SerializeField, Tooltip("���� ID")]
    private int Id;

    [HideInInspector, SerializeField, Space(20), Range(0, 99f), Header("������ ������")]
    private float Size = 1f;

    /// <summary>
    /// ���� é�ͳ��� ���������� ���� �ɷ�ġ�� ���� �ϱ����� ��
    /// </summary>
    private float StatValue = 1;


    private void SetStatData()
    {
        BASE_STAT = new Dictionary<int, MobStatDataScript>();
        Debug.Log("statdata set");
        var data = MobStatDataScript.GET_DATA_LIST();
        for (int i = 0; i < data.Count; i++)
        {
            if (BASE_STAT.ContainsKey(data[i].Id) == false)
                BASE_STAT.Add(data[i].Id, data[i]);
        }
  
    }

    public MobStatDataScript GetBaseStat()
    {
        string name = this.gameObject.name;
        int.TryParse(name[6].ToString(), out int chaper);
        int.TryParse(name[8].ToString(), out int num);
        Id = chaper * 10 + num;
       // Debug.Log(name[7].ToString() + name[9].ToString() + "ID :" + Id);
        if (BASE_STAT == null)
        {
            SetStatData();
        }

        if (StatValue == 1.0f)
            return BASE_STAT[Id];
        else
        {
            var rtBase = new MobStatDataScript(Id, (int)(BASE_STAT[Id].Hp * StatValue),
                (int)(BASE_STAT[Id].Atk * StatValue),
                (int)(BASE_STAT[Id].Def * StatValue),
                (int)(BASE_STAT[Id].Critical * StatValue),
                (float)(BASE_STAT[Id].CriticalDamage * StatValue),
                (int)(BASE_STAT[Id].MoveSpeed * StatValue),
                BASE_STAT[Id].DropRateGem);
        
            return rtBase;
        }
    }
   
    /// <summary>
    /// ���� é�ͳ��� ���������� ���� �ɷ�ġ�� ���� �ϱ����� ���� ����
    /// </summary>
    /// <param name="value"></param>
    public void SetStatValue(float value = 1.0f)
    {
        StatValue = value;
    }


    public int Hp { get; private set; }
    public int Atk { get; private set; }
    public int Def { get; private set; }
    public int Critical { get; private set; }
    public float CriticalDamage { get; private set; }
    public int MoveSpeed { get; private set; }
    public int[] DropRateGem { get; private set; }
    
    public void SetSize()
    {
        this.gameObject.transform.localScale = new Vector3(Size, Size, Size);
    }

 

    private string _name;
    private string _prefabActor;

}

#if UNITY_EDITOR
[CustomEditor(typeof(StatBase))] //���⿡�� Ŀ���� �����͸� ���̱� ���� ��ũ��Ʈ�� �����մϴ�
[CanEditMultipleObjects]
public class InspectorTest : Editor
{
    //Ŀ�����ϱ� ���� �������� ������ ������ ���� �մϴ�. 
    private SerializedProperty _StatType;
    private SerializedProperty _StatTypeString;
    private SerializedProperty _Size;
    StatBase script;

    private void OnEnable()
    {
        //�ش� �ν�����â��  Ȱ��ȭ�ɶ�, ȣǮ�˴ϴ�.
        script = target as StatBase;
        _StatType = serializedObject.FindProperty("_StatType"); // UnitBase�ǿ��� ���� ������ ã�� �����մϴ�. 
        _StatTypeString = serializedObject.FindProperty("StatInfo");
        _Size = serializedObject.FindProperty("Size");

     
       // script.SetStat();
        script.SetSize();

    }

    public override void OnInspectorGUI()   //OnInspectorGUI �� �������̵� �� �ݴϴ�.
    {

        base.OnInspectorGUI(); // UnitBase�� �ִ� �������� �״�� ����մϴ�. �ش� �κ��� �����ϸ� UnitBase���ִ� �������� ����� �˴ϴ�. 
        EditorGUI.BeginChangeCheck(); // �ش� �Լ��� �ν������� ���� ��ȭ�� �ֱ��� ���� üũ�մϴ�.
      

        EditorGUILayout.PropertyField(_Size);
        serializedObject.ApplyModifiedProperties();
        if (EditorGUI.EndChangeCheck())// �ش� �Լ��� �ν������� ���� ��ȭ�� �ִ��� üũ�մϴ�.
        {
         
           //script.SetStat();
            script.SetSize();

        }


    }
}
#endif
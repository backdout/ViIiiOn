
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.Text;
using System.Linq;
using System;

public class StatBase : MonoBehaviour
{


    private static Dictionary<int, MobStatDataScript> BASE_STAT;

    [SerializeField, Tooltip("고유 ID")]
    private int Id;

    [HideInInspector, SerializeField, Space(20), Range(0, 99f), Header("프리팹 사이즈")]
    private float Size = 1f;

    /// <summary>
    /// 동일 챕터내의 스테이지별 몹의 능력치를 조절 하기위한 값
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
    /// 동일 챕터내의 스테이지별 몹의 능력치를 조절 하기위한 값을 셋팅
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
[CustomEditor(typeof(StatBase))] //여기에서 커스텀 에디터를 붙이기 위한 스크립트를 지정합니다
[CanEditMultipleObjects]
public class InspectorTest : Editor
{
    //커스텀하기 위한 변수들을 관리할 변수를 선언 합니다. 
    private SerializedProperty _StatType;
    private SerializedProperty _StatTypeString;
    private SerializedProperty _Size;
    StatBase script;

    private void OnEnable()
    {
        //해당 인스펙터창이  활성화될때, 호풀됩니다.
        script = target as StatBase;
        _StatType = serializedObject.FindProperty("_StatType"); // UnitBase의에서 관련 변수를 찾아 연결합니다. 
        _StatTypeString = serializedObject.FindProperty("StatInfo");
        _Size = serializedObject.FindProperty("Size");

     
       // script.SetStat();
        script.SetSize();

    }

    public override void OnInspectorGUI()   //OnInspectorGUI 에 오버라이드 해 줍니다.
    {

        base.OnInspectorGUI(); // UnitBase에 있는 변수들을 그대로 출력합니다. 해당 부분을 제거하면 UnitBase에있는 변수들은 미출력 됩니다. 
        EditorGUI.BeginChangeCheck(); // 해당 함수는 인스펙터의 값이 변화가 있기전 값을 체크합니다.
      

        EditorGUILayout.PropertyField(_Size);
        serializedObject.ApplyModifiedProperties();
        if (EditorGUI.EndChangeCheck())// 해당 함수는 인스펙터의 값이 변화가 있는지 체크합니다.
        {
         
           //script.SetStat();
            script.SetSize();

        }


    }
}
#endif
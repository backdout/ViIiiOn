
using System.Collections.Generic;

using UnityEngine;
/*
  SpawnData을 바탕으로 몬스터 스폰을 관리
  SpawnData에서의 몬스터 출력 시간은 0.1초 단위이며 그 이하 단위는 필요에 따라 변경시, 수정이 필요하다.
  spawnSpotCircleList 12시 방향 부터 순서대로 총 12개로 출력
  spawnSpotSquareList눈 한변이 5인 사각형으로 왼쪽 위 모서리부터 순차적으로 으로 있으며 총 17개  
 */
public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnSpotCircle;
    [SerializeField]
    private GameObject spawnSpotSquare;


    private List<SpawnSpot> spawnSpotCircleList;
    private List<SpawnSpot> spawnSpotSquareList;
    List<StageMobDataScript> stageSpawnData;
    //Assets/Resources/SPUM/SPUM_Units/Player.prefab
    private const string mobName = "SPUM/SPUM_Units/stage_";
    private List<GameObject> mobPrefabs;

    //추후에 배틀 매니저에서 타임 가져오기 
    private double spawnTime;
    int stageNum;
    public bool isInit = false;
    // Start is called before the first frame update
    public bool Init(int stagaNumber = 0)
    {

        stageNum = stagaNumber == 0 ? 1 : stagaNumber; 

        // spot세팅 
        spawnSpotCircleList = new List<SpawnSpot>();
        spawnSpotSquareList = new List<SpawnSpot>();

        var spots = spawnSpotCircle.GetComponentsInChildren<SpawnSpot>();

        foreach (var spot in spots)
        {
            spawnSpotCircleList.Add(spot);
        }
        
        spots = spawnSpotSquare.GetComponentsInChildren<SpawnSpot>();
        foreach (var spot in spots)
        {
            spawnSpotSquareList.Add(spot);
        }

        var spawnData = StageMobDataScript.GET_DATA(stageNum);

        stageSpawnData = new List<StageMobDataScript>();
        foreach(var data in spawnData)
        {
            stageSpawnData.Add(data);
        }

        if (stageSpawnData == null || stageSpawnData.Count == 0)
        {
            Debug.LogError("Stage Mob Data Err");
            return false;
        }

        mobPrefabs = new List<GameObject>();
        var MaxValue = GetMobCount(stageNum);
        for (int i = 1; i <= MaxValue; i++)
        {//stage_1_1
            var mob = Resources.Load<GameObject>(mobName + stageNum + "_" + i);
            if (mob == null)
                Debug.Log("mobPrefab is Null  : " + mobName + stageNum + "_" + i);
            else
                mob.SetActive(false);
           
            mobPrefabs.Add(mob);
        }
        isInit = true;
        return isInit;

    }


    public bool ReInit(int stagaNumber = 0)
    {
        
        isInit = false;
        //재 인입이기에, 기존에 있는 몬스터 프리팹 데이터 초기화 
        if(mobPrefabs?.Count > 0)
        {
            mobPrefabs.Clear();
        }

        
        stageNum = stagaNumber == 0 ? stageNum : stagaNumber;

        stageSpawnData = StageMobDataScript.GET_DATA(stageNum);
        mobPrefabs = new List<GameObject>();
        var MaxValue = GetMobCount(stageNum);
        for (int i = 1; i <= MaxValue; i++)
        {//stage_1_1
            var mob = Resources.Load<GameObject>(mobName + stageNum + "_" + i);
            if (mob == null)
                Debug.Log("mobPrefab is Null  : " + mobName + stageNum + "_" + i);
            else
                mob.SetActive(false);

            mobPrefabs.Add(mob);
        }

        isInit = true;
        return isInit;

    }




    // Update is called once per frame
    void Update()
    {
        spawnTime += Time.deltaTime; 


        if (isInit == false)
            return;

        for (int i = 0; i < stageSpawnData.Count; i++)
        {
            if (stageSpawnData[i].time <= spawnTime)
            {
                SpawnUpdate(i);
                stageSpawnData.RemoveAt(i);
                return;
            }

        }

           
    }


    private void SpawnUpdate(int index)
    {
        if (stageSpawnData[index].type == 0 )
            return;

        var list = stageSpawnData[index].type == 1 ? spawnSpotCircleList : spawnSpotSquareList;

        for (int i = 0; i < list.Count; i++)
        {
            if (stageSpawnData[index].spowns[i] == 0)
                continue;

            list[i].SetData(mobPrefabs[stageSpawnData[index].spowns[i] - 1]);
        }

    }

    private int GetMobCount(int stageNum)
    {
        if(stageNum == 0) 
            return 0;
        else if (stageNum < 5)
            return 3;
        else if (stageNum < 10)
            return 3;

        return 0;



    }

}

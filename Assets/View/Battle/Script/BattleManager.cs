
using System.Collections.Generic;

using UnityEngine;

public class BattleManager : MonoBehaviour
{

    public const float CLEAR_TIME = 30f;

    // 추후에 캐릭터 속성 변경시 생성으로 변경; 
    [SerializeField]
    private Rigidbody2D _playerBody;
    public GameObject dropItemGroup;
    private Player _player;
    private double _time;
    private int[] gemCounts;

    bool isPush;
    public bool IsPush 
    { 
        get
        { 
            return isPush; 
        }    
        set 
        {
            isPush = value;
            SetPause();
        }
    }

    public double Playtime
    {
        get { return _time; }   
        private set { _time = value; }
    }

    public int stageNum { get; private set; }

    public Rigidbody2D PlayerBody
    {
        get {
            if (_playerBody == null)
                Debug.Log("null");
            return _playerBody; 
        } 
        private set { _playerBody = value; } 
    }

    public Player Player
    {
        get
        {
            if (_player == null)
                Debug.Log("null");
            return _player;
        }
        private set { _player = value; }
    }


    private List<Monster> MonsterList;
    private List<DropItem> DropItemList;

    public void Init(int _stageNum)
    {
        stageNum = _stageNum == 0 ? 1 : _stageNum;
        if (Player == null)
            Player = _playerBody.transform.GetComponent<Player>();
       
        gemCounts = new int[3];
        ResetTime();

    }
    public void ReInit(int _stageNum)
    {
        stageNum = _stageNum == 0 ? stageNum : _stageNum;
        if (Player == null)
            Player = _playerBody.transform.GetComponent<Player>();
        BattleSceneManager.Instance.battleManager.IsPush = false;
        Player.Init();
        // 몹리스트 정리 
        RemoveAllMonsterList();
        // 드랍 아이템 정리 
        RemoveAllDropItemList();
        gemCounts = new int[3];
        ResetTime();
    }




    private void ResetTime()
    {
        Playtime = 0.0f;
        IsPush = false;
    }

    private void FixedUpdate()
    {
        Playtime += Time.fixedDeltaTime;
        BattleSceneManager.Instance.battleUI.UpdateTime(Playtime);

        if (Playtime >= CLEAR_TIME)
            GameClear();
    }




    public void AddMonsterList(Monster monster)
    {
        if(MonsterList == null)
            MonsterList = new List<Monster>();

        MonsterList.Add(monster);
    }


    public void RemoveMonsterList(Monster monster)
    {
        if (MonsterList == null)
            return;

        MonsterList.Remove(monster);
    }

    public void RemoveAllMonsterList()
    {
        if (MonsterList == null)
            return;

        foreach (Monster monster in MonsterList)
        {
            monster.SetMobDestroy();
        }

        MonsterList.Clear(); 
    }

    public bool HasCloseMob()
    {
        var mob = GetClosestMonster();
        return mob != null && (mob.transform.position - Player.transform.position).sqrMagnitude < 3.5f;
    }


    public UnitHandler GetClosestMonster()
    {
        if (MonsterList != null && MonsterList.Count > 0)
        {
            MonsterList.Sort((a, b) => (a.transform.position - Player.transform.position).sqrMagnitude.CompareTo((b.transform.position - Player.transform.position).sqrMagnitude));

            for (int i = 0; i < MonsterList.Count; i++)
            {
                if (MonsterList[i] != Player && MonsterList[i].IsDie == false)
                {
                    return MonsterList[i];
                }
            }
        }

        return null;
    }

 

    public void AddDropItemList(DropItem dropItem)
    {
        if (DropItemList == null)
            DropItemList = new List<DropItem>();

        DropItemList.Add(dropItem);
    }


    public void RemoveDropItemList(DropItem dropItem)
    {
        if (DropItemList == null)
            return;

        DropItemList.Remove(dropItem);
    }

    /// <summary>
    /// 초기화시 드랍 아이템 모두 제거 할때 사용
    /// </summary>
    /// <param name="dropItem"></param>
    public void RemoveAllDropItemList()
    {
        if (DropItemList == null)
            return;

        foreach (var item in DropItemList)
        {
            item.DeleteDropItem();
        }

        DropItemList.Clear();

    }



    public void SetCloseDropItem()
    {
        if (DropItemList != null && DropItemList.Count > 0)
        {
            for (int i = 0; i < DropItemList.Count; i++)
            {
                if (DropItemList[i].enabled && (DropItemList[i].transform.position - Player.transform.position).sqrMagnitude < 3.5f)
                {
                     DropItemList[i].SetDropItemMove(true);
                }
            }
        }

    }



    public void AddGemCount(int index)
    {
        gemCounts[index] += 1; 

    }

    public int[] GetGemCounts()
    { 
        return gemCounts; 
    }

    public void GameClear()
    {
        //  
        if (IsPush)
            return;

        IsPush = true;

        //드랍 아이템 획득 갱신 
        for (int i = 0; i < gemCounts.Length; i++)
            ItemData.Instance.AddItemValue(1 + i, gemCounts[i]);

      

        // 클리어 스테이지 정보 갱신
        UserData.Instance.LastEnterStage += 1;
        if (stageNum > UserData.Instance.ClearStage)
            UserData.Instance.ClearStage = stageNum;


       // 각종 데이터 저장
        if (GoogleSignInManager.Instance.HasLogin())
        {
            FirestoreDBManager.Instance.SaveUserData();
            FirestoreDBManager.Instance.SaveItemdata();
            Debug.Log("Userdata Save");
        }
        else
        {
            RecordManager.Instance.SetSaveItemDatas(ItemData.Instance.GetItemDatas().ToArray());
            RecordManager.Instance.SetSaveStageData();
        }

        // 클리어 창 출력
        BattleSceneManager.Instance.battleUI.GameClear();  

    }


    public void GameOver()
    {
        IsPush = true;

    }
    public void SetPause()
    {
        if (IsPush)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}

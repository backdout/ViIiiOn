using Google;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;


public class UserData : Singleton<UserData>
{

    public static int XOR = 0x18273645;

    public static float FXOR = 2.123f;
    public static string DATA_CRC = "";

    /* 기본 재화
      
    */
    #region 기본재화
    private int _Ticket;
    public int Ticket
    {
        get
        {
            return _Ticket ^ XOR;
        }

        set
        {
            _Ticket = value ^ XOR;

        }
    }

    private int _Gold;
    public int Gold
    {
        get
        {
            return _Gold ^ XOR;
        }

        set
        {
            _Gold = value ^ XOR;

        }
    }

   

    private int _SkillRedPoint;
    public int SkillRedPoint
    {
        get
        {
            return _SkillRedPoint ^ XOR;
        }

        set
        {
            _SkillRedPoint = value ^ XOR;

        }
    }

    private int _SkillBluePoint;
    public int SkillBluePoint
    {
        get
        {
            return _SkillBluePoint ^ XOR;
        }

        set
        {
            _SkillBluePoint = value ^ XOR;

        }
    }

    private int _SkillGreenPoint;
    public int SkillGreenPoint
    {
        get
        {
            return _SkillGreenPoint ^ XOR;
        }

        set
        {
            _SkillGreenPoint = value ^ XOR;

        }
    }

  

    public string NickName { get; private set; }
    #endregion

    #region  스테이지 클리어 정보 

    /// <summary>
    /// 마지막에 입장한 곳 정보, 클리어한 정보가 아님
    /// </summary>
    public int LastEnterStage = 0;
    public int ClearStage = 0;
    


    private Dictionary<int, List<bool>> _StageClearData;

    public Dictionary<int, List<bool>> StageClearData
    {
        get
        {
            if (_StageClearData == null)
            {
                _StageClearData = new Dictionary<int, List<bool>>();
                var data = RecordManager.Instance.GetLoadStageData(RecordManager.StageData.All);
                for (int i = 1; i < (int)RecordManager.StageData.Count; i++)
                {
                    _StageClearData.Add(i, new List<bool>());             

                    if (data != null && data[i] != null)
                    {
                        foreach (var item in data[i])
                        {

                            _StageClearData[i].Add(item);
                           
                        }
                    }
                    else
                        _StageClearData[i].Add(false);
                }

            }

            return _StageClearData;
        }

        set
        {
            _StageClearData = value;
            RecordManager.Instance.SetSaveStageData(RecordManager.StageData.All);
        }
    }

    #endregion

    
    bool isInit = false;
    public void Init()
    {
        if (isInit)
            return;

        SetUserLoadData();

        isInit = true;
    }

    private void SetUserLoadData()
    {
        if (GoogleSignInManager.Instance.HasLogin())
        {// 파이어 베이스 데이터
            FirestoreDBManager.Instance.LoadUserData();

        }
        else
        {// 로그인 안되어 있을 경우, 클라에 저장된 정보를 가져옴
            SetLoadRecordUserData();
        }

    }


    /// <summary>
    /// 클라에 저장된 레코드 데이터
    /// </summary>
    public void SetLoadRecordUserData()
    {
        var record = RecordManager.Instance;
        NickName = record.GetLoadNickNameData();
        Gold = record.GetLoadUserInfoData(RecordManager.UserDataKey.Gold);
        Ticket = 10;//record.GetLoadUserInfoData(RecordManager.UserDataKey.Ticket);
        SkillRedPoint = record.GetLoadUserInfoData(RecordManager.UserDataKey.SkillRedPoint);
        SkillBluePoint = record.GetLoadUserInfoData(RecordManager.UserDataKey.SkillBluePoint);
        SkillGreenPoint = record.GetLoadUserInfoData(RecordManager.UserDataKey.SkillGreenPoint);
        LastEnterStage = record.GetLoadUserInfoData(RecordManager.UserDataKey.LastEnterStage);
    }


    public void SetDefaultValue(string nickName = "Hero" )
    {
        NickName = nickName;

        Gold = 5000;
        Ticket = 30;
        SkillRedPoint = 0;
        SkillBluePoint = 0;
        SkillGreenPoint = 0;
        LastEnterStage = 0;
    }

    public void SetLoadValue(Dictionary<string, object> data)
    {

        foreach (KeyValuePair<string, object> kvp in data)
        {
            switch(kvp.Key)
            {
                case "NickName":
                    NickName = kvp.Value.ConvertTo<string>();
                    if (string.IsNullOrEmpty(NickName))
                        NickName = "Hero";
                    break;
                case "Ticket":
                    Ticket = 20;//kvp.Value.ConvertTo<int>();
                    break;
                case "Gold":
                    Gold = kvp.Value.ConvertTo<int>();
                    break;
                case "SkillRedPoint":
                    SkillRedPoint = kvp.Value.ConvertTo<int>();
                    break;
                case "SkillBluePoint":
                    SkillBluePoint = kvp.Value.ConvertTo<int>();
                    break;
                case "SkillGreenPoint":
                    SkillGreenPoint = kvp.Value.ConvertTo<int>();
                    break;
                case "LastEnterStage":
                    LastEnterStage = kvp.Value.ConvertTo<int>();
                    break;
                case "ClearStage":
                    ClearStage = kvp.Value.ConvertTo<int>();
                    break;


            }
        }
       


    }
    public void SetSkillTree(SkillTreeDataScript.SKILL_TREE_TYPE skillTreeType, int id)
    {
        switch(skillTreeType) {

            case SkillTreeDataScript.SKILL_TREE_TYPE.RED:
                SkillRedPoint = id;
                break;

            case SkillTreeDataScript.SKILL_TREE_TYPE.BLUE:
                SkillBluePoint = id;             
                break;
            case SkillTreeDataScript.SKILL_TREE_TYPE.GREEN:
                SkillGreenPoint = id;
                break;

        }

       

    }

   


}

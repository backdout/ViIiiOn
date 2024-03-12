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

    /* �⺻ ��ȭ
      
    */
    #region �⺻��ȭ
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

    #region  �������� Ŭ���� ���� 

    /// <summary>
    /// �������� ������ �� ����, Ŭ������ ������ �ƴ�
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
        {// ���̾� ���̽� ������
            FirestoreDBManager.Instance.LoadUserData();

        }
        else
        {// �α��� �ȵǾ� ���� ���, Ŭ�� ����� ������ ������
            SetLoadRecordUserData();
        }

    }


    /// <summary>
    /// Ŭ�� ����� ���ڵ� ������
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

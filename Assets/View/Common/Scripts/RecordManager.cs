using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.InputSystem;

public class RecordManager : Singleton<RecordManager>
{

    private const string encryptKey = "dry1234";// 해당 부분 변경시, 데이터 오류 발생 주의

    #region 옵션데이터
    // 효과음/배경음/진동/조이스틱/구글/언어
    public enum OptionData
    {
        BgmSound,
        EffectSound,
        Vibration,
        Joystick,
        Login,
        Lang,
        All,
    }

    public void SetSaveOptionData(OptionData Key)
    {
        var optionData = OptionDataManager.Instance;
        switch (Key)
        {

            case OptionData.All:
                SetSave(OptionData.EffectSound.ToString(), optionData.EffectSound.ToString());
                SetSave(OptionData.BgmSound.ToString(), optionData.BgmSound.ToString());
                SetSave(OptionData.Vibration.ToString(), optionData.Vibration.ToString());

                SetSave(OptionData.Joystick.ToString(), optionData.Joystick.ToString());
                SetSave(OptionData.Login.ToString(), optionData.GoogleLogin.ToString());
                SetSave(OptionData.Lang.ToString(), optionData.Language.ToString());
                break;

            case OptionData.EffectSound:
                SetSave(OptionData.EffectSound.ToString(), optionData.EffectSound.ToString());
                break;

            case OptionData.BgmSound:
                SetSave(OptionData.BgmSound.ToString(), optionData.BgmSound.ToString());
                break;
            case OptionData.Vibration:
                SetSave(OptionData.Vibration.ToString(), optionData.Vibration.ToString());
                break;
            case OptionData.Login:
                SetSave(OptionData.Login.ToString(), optionData.GoogleLogin.ToString());
                break;
            case OptionData.Joystick:
                SetSave(OptionData.Joystick.ToString(), optionData.Joystick.ToString());
                break;
            case OptionData.Lang:
                SetSave(OptionData.EffectSound.ToString(), optionData.Language.ToString());
                break;

        }

    }


    public string GetLoadOptionData(OptionData Key)
    {
        return GetLoadData(Key.ToString());
    }


    #endregion



    #region 유저 정보 저장
    public enum UserDataKey : int
    {
        All = 0,
        NickName = 1,       
        Ticket = 2,
        Gold = 3,
        SkillRedPoint = 4,
        SkillBluePoint = 5,
        SkillGreenPoint = 6,
        LastEnterStage = 7,
        ClearStage = 8,

        Count = 9,

    }

    public void SetSaveUserData(UserDataKey Key)
    {
        var userdata = UserData.Instance;
        switch (Key)
        {

            case UserDataKey.All:
                {
                    SetSave(UserDataKey.NickName.ToString(), userdata.NickName.ToString());
                  
                    SetSave(UserDataKey.Ticket.ToString(), userdata.Ticket.ToString());
                    SetSave(UserDataKey.Gold.ToString(), userdata.Gold.ToString());
                    SetSave(UserDataKey.SkillRedPoint.ToString(), userdata.SkillRedPoint.ToString());
                    SetSave(UserDataKey.SkillBluePoint.ToString(), userdata.SkillBluePoint.ToString());
                    SetSave(UserDataKey.SkillGreenPoint.ToString(), userdata.SkillGreenPoint.ToString());
                    SetSave(UserDataKey.LastEnterStage.ToString(), userdata.LastEnterStage.ToString());
                    SetSave(UserDataKey.ClearStage.ToString(), userdata.ClearStage.ToString());
                }
                break;

            case UserDataKey.NickName:
                SetSave(UserDataKey.NickName.ToString(), userdata.NickName.ToString());
                break;
            case UserDataKey.Ticket:
                SetSave(UserDataKey.Ticket.ToString(), userdata.Ticket.ToString());
                break;
            case UserDataKey.Gold:
                SetSave(UserDataKey.Gold.ToString(), userdata.Gold.ToString());
                break;
            case UserDataKey.SkillRedPoint:
                SetSave(UserDataKey.SkillRedPoint.ToString(), userdata.SkillRedPoint.ToString());
                break;
            case UserDataKey.SkillBluePoint:
                SetSave(UserDataKey.SkillBluePoint.ToString(), userdata.SkillBluePoint.ToString());
                break;
            case UserDataKey.SkillGreenPoint:
                SetSave(UserDataKey.SkillGreenPoint.ToString(), userdata.SkillGreenPoint.ToString());
                break;
            case UserDataKey.LastEnterStage:
                SetSave(UserDataKey.LastEnterStage.ToString(), userdata.LastEnterStage.ToString());
                break;

        }

    }

    public void SetSaveStageData()
    {
        var userdata = UserData.Instance;
        SetSave(UserDataKey.Ticket.ToString(), userdata.Ticket.ToString());
        SetSave(UserDataKey.Gold.ToString(), userdata.Gold.ToString());
        SetSave(UserDataKey.LastEnterStage.ToString(), userdata.LastEnterStage.ToString());
        SetSave(UserDataKey.ClearStage.ToString(), userdata.ClearStage.ToString());
    }

    public void SetSaveSkillViewData()
    {
        var userdata = UserData.Instance;
        SetSave(UserDataKey.Gold.ToString(), userdata.Gold.ToString());
        SetSave(UserDataKey.SkillRedPoint.ToString(), userdata.SkillRedPoint.ToString());
        SetSave(UserDataKey.SkillBluePoint.ToString(), userdata.SkillBluePoint.ToString());
        SetSave(UserDataKey.SkillGreenPoint.ToString(), userdata.SkillGreenPoint.ToString());
        
    }


    //일반 재화/스킬/마지막 스테이지 
    public int GetLoadUserInfoData(UserDataKey Key)
    {
        string data = GetLoadData(Key.ToString());
        int result = Key == UserDataKey.Gold? 5000 : Key == UserDataKey.Ticket? 3 : 0;

        if (string.IsNullOrEmpty(data) == false)
        {
            int.TryParse(data.ToLower(), out result);
        }

        return result;

    }
    // 닉네임
    public string GetLoadNickNameData()
    {
        string data = GetLoadData(UserDataKey.NickName.ToString());
        return data;
    }

    #endregion

    #region 아이템 저장 

    private const string itemKey = "ItemInfos";// 변경 필요// 유저아이디 값도 필요할 듯/ 
    /// <summary>
    /// 저장 시점/ 신규 획득, 구매 판매, 사용, 
    /// </summary>
    /// <param name="itemInfos"></param>
    public void SetSaveItemDatas(ItemData.ItemInfo[] itemInfos)
    {// 테스트 필요 
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, itemInfos);
        string result = Convert.ToBase64String(ms.GetBuffer());


        SetSave(itemKey, result);
    }
    public ItemData.ItemInfo[] GetLoadItemDatas()
    {
        // 테스트 필요 
        var data = GetLoadData(itemKey);

        if (string.IsNullOrEmpty(data))
            return null;


        var binaryFormatter = new BinaryFormatter();
        var memoryStream = new MemoryStream(Convert.FromBase64String(data));

        ItemData.ItemInfo[] rt = (ItemData.ItemInfo[])binaryFormatter.Deserialize(memoryStream);

        return rt;
    }


    #endregion

    #region 스테이지 정보

    // 1챕터당 총 5스테이지로 구성
    // 챕터당 스테이지 클리어 정보 기록 
    // 챕터가 추가 되시, 이넘 값을 하나씩 추가한다. 
    public enum StageData
    {
        All = 0, // 현재 게임 업데이트상 오픈된 스테이지 정보  
        Chapter1 = 1,// 챕터 1,  
        Chapter2 = 2,
        Chapter3 = 3,

        Count,
    }

    public void SetSaveStageData(StageData Key)
    {
        // 테스트 필요 
        Debug.Log(StageData.Count.ToString());

        var userdata = UserData.Instance;
        Debug.Log(userdata.StageClearData[0].ToString());
        if (Key == StageData.All)
        {
            for (int i = 1; i < (int)StageData.Count; i++)
                SetSave("Chapter" + i, userdata.StageClearData[i].ToString());

        }
        else
        {
            SetSave(Key.ToString(), userdata.StageClearData[(int)(Key - 1)].ToString());
        }
    }


    public Dictionary<int, List<bool>> GetLoadStageData(StageData Key)
    {
        var rt = new Dictionary<int, List<bool>>();
        if (Key == StageData.All)
        {           
          
            for (int i = 1; i < (int)RecordManager.StageData.Count; i++)
            {
                rt.Add(i, new List<bool>());
                var data = GetLoadData(Key.ToString());
                if (string.IsNullOrEmpty(data))
                {
                    for (int k = 0; k < 5; k++)
                        rt[i].Add(false);

                    continue;
                }    
                var binaryFormatter = new BinaryFormatter();
                var memoryStream = new MemoryStream(Convert.FromBase64String(data));

                var clear = (List<bool>)binaryFormatter.Deserialize(memoryStream);

                for (int k = 0; k < 5; k++)
                {
                    if (clear == null || clear.Count <= k)
                        rt[(int)i].Add(false);
                    else
                        rt[(int)i].Add(clear[k]);
                }
            }
           
        }
        else
        {
            rt.Add((int)Key, new List<bool>());
            var data = GetLoadData(Key.ToString());
            
            if (string.IsNullOrEmpty(data))
            {
                for (int k = 0; k < 5; k++)
                    rt[(int)Key].Add(false);
            }

         
            var binaryFormatter = new BinaryFormatter();
            var memoryStream = new MemoryStream(Convert.FromBase64String(data));
            var clear = (List<bool>)binaryFormatter.Deserialize(memoryStream);


            for (int i = 0; i < 5; i++)
            {
                if (clear == null || clear.Count <= i)
                    rt[(int)Key].Add(false);
                else
                    rt[(int)Key].Add(clear[i]);
            }

        }


        return rt;

    }
    #endregion

    private void SetSave(string Key, string value)
    {
        MD5 md5Hash = MD5.Create();
        byte[] hashData = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Key));
        string hashKey = System.Text.Encoding.UTF8.GetString(hashData);
        byte[] secret = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(encryptKey));

        // Encrypt '_value' into a byte array  
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);

        // Eecrypt '_value' with 3DES.  
        TripleDES des = new TripleDESCryptoServiceProvider();
        des.Key = secret;
        des.Mode = CipherMode.ECB;
        ICryptoTransform xform = des.CreateEncryptor();
        byte[] encrypted = xform.TransformFinalBlock(bytes, 0, bytes.Length);

        // Convert encrypted array into a readable string.  
        string encryptedString = Convert.ToBase64String(encrypted);

        // Set the ( key, encrypted value ) pair in regular PlayerPrefs.  
        PlayerPrefs.SetString(hashKey, encryptedString);
    }

    private string GetLoadData(string Key)
    {

        // Hide '_key' string.  
        MD5 md5Hash = MD5.Create();
        byte[] hashData = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Key));
        string hashKey = System.Text.Encoding.UTF8.GetString(hashData);
        byte[] secret = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(encryptKey));

        // Retrieve encrypted '_value' and Base64 decode it.        
        string _value = PlayerPrefs.GetString(hashKey);
        if (string.IsNullOrEmpty(_value))
            return _value;

        byte[] bytes = Convert.FromBase64String(_value);

        // Decrypt '_value' with 3DES.  
        TripleDES des = new TripleDESCryptoServiceProvider();
        des.Key = secret;
        des.Mode = CipherMode.ECB;
        ICryptoTransform xform = des.CreateDecryptor();
        byte[] decrypted = xform.TransformFinalBlock(bytes, 0, bytes.Length);

        // decrypte_value as a proper string.  
        string decryptedString = System.Text.Encoding.UTF8.GetString(decrypted);
        return decryptedString;

    }

    public void DeleteAllData()
    {
        PlayerPrefs.DeleteAll();
    }

    public bool HasUserData()
    {
        //유저 닉넴임은 무조건 기재되어 있음으로, 닉네임 유무로 체크 
        return string.IsNullOrEmpty(GetLoadNickNameData()) == false; 
    }

}

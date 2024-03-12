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

    private const string encryptKey = "dry1234";// �ش� �κ� �����, ������ ���� �߻� ����

    #region �ɼǵ�����
    // ȿ����/�����/����/���̽�ƽ/����/���
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



    #region ���� ���� ����
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


    //�Ϲ� ��ȭ/��ų/������ �������� 
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
    // �г���
    public string GetLoadNickNameData()
    {
        string data = GetLoadData(UserDataKey.NickName.ToString());
        return data;
    }

    #endregion

    #region ������ ���� 

    private const string itemKey = "ItemInfos";// ���� �ʿ�// �������̵� ���� �ʿ��� ��/ 
    /// <summary>
    /// ���� ����/ �ű� ȹ��, ���� �Ǹ�, ���, 
    /// </summary>
    /// <param name="itemInfos"></param>
    public void SetSaveItemDatas(ItemData.ItemInfo[] itemInfos)
    {// �׽�Ʈ �ʿ� 
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, itemInfos);
        string result = Convert.ToBase64String(ms.GetBuffer());


        SetSave(itemKey, result);
    }
    public ItemData.ItemInfo[] GetLoadItemDatas()
    {
        // �׽�Ʈ �ʿ� 
        var data = GetLoadData(itemKey);

        if (string.IsNullOrEmpty(data))
            return null;


        var binaryFormatter = new BinaryFormatter();
        var memoryStream = new MemoryStream(Convert.FromBase64String(data));

        ItemData.ItemInfo[] rt = (ItemData.ItemInfo[])binaryFormatter.Deserialize(memoryStream);

        return rt;
    }


    #endregion

    #region �������� ����

    // 1é�ʹ� �� 5���������� ����
    // é�ʹ� �������� Ŭ���� ���� ��� 
    // é�Ͱ� �߰� �ǽ�, �̳� ���� �ϳ��� �߰��Ѵ�. 
    public enum StageData
    {
        All = 0, // ���� ���� ������Ʈ�� ���µ� �������� ����  
        Chapter1 = 1,// é�� 1,  
        Chapter2 = 2,
        Chapter3 = 3,

        Count,
    }

    public void SetSaveStageData(StageData Key)
    {
        // �׽�Ʈ �ʿ� 
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
        //���� �г����� ������ ����Ǿ� ��������, �г��� ������ üũ 
        return string.IsNullOrEmpty(GetLoadNickNameData()) == false; 
    }

}

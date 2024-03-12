using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using Google;
using System;
using Unity.VisualScripting;
using static ItemData;

public class FirestoreDBManager : Singleton<FirestoreDBManager>
{
    /*
     �ش� DB �� �α��� ���� �����Ͽ��� �ϴ� �κ������� ȣ�� �� GoogleSignInManager�� Awake ��  ����ǰ� ���� �Ǿ�� �Ѵ�. 
     ���� �ĺ����� �������� �����ʹ� ���� �����Ϳ� ������ ������, ���� �����ʹ� ���� ������ ���� ��� �߰��� �߰�
     */
    FirebaseFirestore DB;
    private string uid;
    
    public async void Init()
    {        
        DB = FirebaseFirestore.DefaultInstance;      
        uid = GoogleSignInManager.Instance.currentUser?.UserId;
    }


    public bool IsComplete { get; private set; }


    public void SetUid(string _uid)
    {
        DB = FirebaseFirestore.DefaultInstance;
        uid = _uid;
    }
    public void SaveUserData()
    {
        if (string.IsNullOrEmpty(uid))
        {
            Init();
        }
        var userData = UserData.Instance;

        var user =  new Dictionary<string, object>()
        {
            { "Uid", uid },
            { "NickName", userData.NickName },
            { "Ticket", userData.Ticket },
            { "Gold", userData.Gold },
            { "SkillRedPoint", userData.SkillRedPoint },
            { "SkillBluePoint", userData.SkillBluePoint },
            { "SkillGreenPoint", userData.SkillGreenPoint },
            { "LastEnterStage", userData.LastEnterStage },
            { "ClearStage", userData.ClearStage }
        };


        SaveUserData(user, uid);

    }

    
    public void SaveDefaultUserData()
    {
        if (string.IsNullOrEmpty(uid))
        {
            Init();
        }
        var userData = UserData.Instance;
        UserData.Instance.SetDefaultValue();
        var user = new Dictionary<string, object>()
        {
            { "Uid", uid },
            { "NickName", userData.NickName },
            { "Ticket", userData.Ticket },
            { "Gold", userData.Gold },
            { "SkillRedPoint", userData.SkillRedPoint },
            { "SkillBluePoint", userData.SkillBluePoint },
            { "SkillGreenPoint", userData.SkillGreenPoint },
            { "LastEnterStage", userData.LastEnterStage },
            { "ClearStage", userData.ClearStage }
        };


        SaveUserData(user, uid);

    }


    private void SaveUserData(Dictionary<string, object> userdata, string uid)
    {
        IsComplete = false;

        if (string.IsNullOrEmpty(uid))
        {
            Init();
        }
        
        CollectionReference Userlist = DB.Collection("UserData");

        Userlist.Document(uid).SetAsync(userdata).ContinueWithOnMainThread(task => 
        { 
            if(task.IsCompleted)
            {
                IsComplete = true;
                HasUserData = true;
            }
            else
            {
                if (task.IsFaulted)
                    Debug.Log("USER_DATA_SAVE_FAIL");
                else if (task.IsCanceled)
                    Debug.Log("USER_DATA_SAVE_CANCLED");
            }

            IsComplete = true;
        });

    }


    async public void LoadUserData()
    {
        IsComplete = false;

        if (string.IsNullOrEmpty(uid))
        {
            Init();
        }
       
        DocumentReference data = DB.Collection("UserData").Document(uid);
        await data.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            Dictionary<string, object> userData;
            if (snapshot.Exists)
            {
                userData = snapshot.ToDictionary();
                if (userData?.Count > 0)
                {
                    UserData.Instance.SetLoadValue(userData);
                }
                else
                    UserData.Instance.SetDefaultValue();

            }
            else
            {
                Debug.Log("UserData Load does not exist!");
                  
                UserData.Instance.SetDefaultValue();


            }
            HasUserData = true;
            IsComplete = true;
        });

    }
    /// <summary>
    /// ������ �����ʹ� userdata�� �ٸ��� Dictionary ������ �ƴ� Ŀ���� �������� 
    /// </summary>
    public void SaveItemdata()
    {
        IsComplete = false;
        if (string.IsNullOrEmpty(uid))
        {
            Init();
        }
        CollectionReference Itemlist = DB.Collection("ItemData");

        List<ItemInfo> itemdata = new List<ItemInfo>();
        FBItemIngoList fBItemIngoList = new FBItemIngoList();
        var datas = ItemData.Instance.GetItemDatas();

        for (int i = 0; i < datas.Count; i++)
        {
            itemdata.Add(datas[i]);
        }
        fBItemIngoList.itemdata = itemdata;

        Itemlist.Document(uid).SetAsync(fBItemIngoList).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("ITEM_DATA_SAVE_IsCompleted");
            }
            else
            {
                if (task.IsFaulted)
                    Debug.Log("ITEM_DATA_SAVE_FAIL");
                else if (task.IsCanceled)
                    Debug.Log("ITEM_DATA_SAVE_CANCLED");
            }

            IsComplete = true;
        });

    }



    /// <summary>
    /// ���� �α��ν�, item�� UID Document ����
    /// </summary>
    public void SaveDefaultItemdata()
    {
        IsComplete = false;
        if (string.IsNullOrEmpty(uid))
        {
            Init();
        }
        CollectionReference Itemlist = DB.Collection("ItemData");

        List<ItemInfo> itemdata = new List<ItemInfo>();
        FBItemIngoList fBItemIngoList = new FBItemIngoList();
        Itemlist.Document(uid).SetAsync(fBItemIngoList).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("ITEM_DATA_SAVE_IsCompleted");
            }
            else
            {
                if (task.IsFaulted)
                    Debug.Log("ITEM_DATA_SAVE_FAIL");
                else if (task.IsCanceled)
                    Debug.Log("ITEM_DATA_SAVE_CANCLED");
            }

            IsComplete = true;
        });

    }

    async public void LoadItemData(List<ItemInfo> itemInfos)
    {
        IsComplete = false;
        if (string.IsNullOrEmpty(uid))
        {
            Init();
        }

        DocumentReference  data = DB.Collection("ItemData").Document(uid);
        await data.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            var snapshot = task.Result;
            FBItemIngoList itemdatas = new FBItemIngoList();
            if (snapshot.Exists)
            {
                itemdatas = snapshot.ConvertTo<FBItemIngoList>();
                if (itemdatas?.itemdata?.Count > 0)
                {
                    itemInfos = itemdatas.itemdata;
                   
                }
                foreach (var item in itemInfos)
                {
                    ItemData.Instance.SetItemValue(item.id, item.value, item.isNew);
                }
            }
            else
                Debug.Log("Itemdata Load does not exist!");

           
            IsComplete = true;
        });

    }


    public bool HasUserData { get; private set; }


    async public void CheckUserData()
    {


        IsComplete = false;
        if (string.IsNullOrEmpty(uid))
        {          
            Init();     
        }
            
        DocumentReference data = DB.Collection("UserData").Document(uid);

              
        if (data == null)
            Debug.Log("data null");

        await data.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            
            DocumentSnapshot snapshot = task.Result;
            Dictionary<string, object> userData;
            if (snapshot.Exists)
            {
               
                userData = snapshot.ToDictionary();
                if (userData?.Count > 0)
                {
                    HasUserData = true;
                }
                else
                    HasUserData = false;

            }
            else
            {
                
                HasUserData = false;
            }
            IsComplete = true;
            
        });






    }
















}

using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static ItemData;


public class ItemData : Singleton<ItemData>
{
    /*
     유저가 보유하고 있는 아이템 정보를 관리하는 곳   
     */

    //ItemMaterial
    [Serializable]
    [FirestoreData]
    public class ItemInfo
    {
        [FirestoreProperty]
        public int id { get; set; }
        [FirestoreProperty]
        public int value { get; set; }
        [FirestoreProperty]
        public bool isNew { get; set; }
    }

    [Serializable]
    [FirestoreData]
    public class FBItemIngoList
    {
        [FirestoreProperty]
        public List<ItemInfo> itemdata { get; set; }
    }


    private List<ItemInfo> ItemDatas;

    const int SKILL_CARD_NUM = 101;
    public void Init()
    {
        ItemDatas = new List<ItemInfo>();

        if (GoogleSignInManager.Instance.HasLogin())
        {

            FirestoreDBManager.Instance.LoadItemData(ItemDatas);
        }
        else
        {
            var data = RecordManager.Instance.GetLoadItemDatas();
            if (data != null)
            {
                ItemDatas = data.ToList();

                foreach (var item in ItemDatas)
                {
                    SetItemValue(item.id, item.value, item.isNew);
                }
            }
        }
       
    }

    public void SetItemValue(int id, int value, bool isNew)
    {
        if (ItemDatas == null)
            Init();

        var item = ItemDatas.Find(x => x.id == id);

        if (item == null)
            ItemDatas.Add(new ItemInfo { id = id, value = value, isNew = true });
        else
            item.value = value;
    }

    public void SetItemValue(int id, int value)
    {
        if (ItemDatas == null)
            Init();

        var item = ItemDatas.Find(x => x.id == id);

        if (item == null )
            ItemDatas.Add(new ItemInfo { id = id, value = value, isNew = true });
        else
            item.value = value;
    }

    public void AddItemValue(int id, int value)
    {
        if (ItemDatas == null)
            Init();

        var item = ItemDatas.Find(x => x.id == id);

        if (item == null)
            ItemDatas.Add(new ItemInfo { id = id, value = value, isNew = true });
        else
            item.value += value;
    }


    public void SubItemValue(int id, int value)
    {
        if (ItemDatas == null)
            Init();

        var item = ItemDatas.Find(x => x.id == id);


        if (item == null || item.value < value)
        {
            NoticePopup.SHOW(NoticePopup.ButtonType.Ok, Lang.UI_MSG("NOTICE_NOT_ENOUGHT_ITEM"), null);
            return;
        }
        else
            item.value -= value;
    }



    public int GetSkillCard(SkillTreeDataScript.SKILL_TREE_TYPE skillTreeType)
    {
        if (ItemDatas == null)
            Init();

        int id = SKILL_CARD_NUM + (int)skillTreeType;

        var item = ItemDatas.Find(x => x.id == id);

        if (item == null)
            return 0;
        else
            return item.value;
    }

    public void SubSkillCardValue(SkillTreeDataScript.SKILL_TREE_TYPE skillTreeType, int value)
    {
        if (ItemDatas == null)
            Init();

        int id = SKILL_CARD_NUM + (int)skillTreeType;

        var item = ItemDatas.Find(x => x.id == id);
        item.value -= value;

    }


    public List<ItemInfo> GetItemDatas()
    {
       
        if (ItemDatas == null)
            return null;
        else
            return ItemDatas;
    }

    public void SetIsNew(int id, bool isNew)
    {
        if (ItemDatas == null)
            return;
        else
        {
            var item = ItemDatas.Find(x => x.id == id);
            if (item == null) return;
            item.isNew = isNew;
        }
    }
}

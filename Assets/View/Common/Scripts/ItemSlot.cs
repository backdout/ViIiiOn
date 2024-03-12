using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    [Serializable]
    public class ItemSlotData
    {

        string ItemIconPath = "UISprite/Theme2/07_Resources/Item/";

        public int id;
        public int count; 
        public string name;
        public string desc;
        public UIDefine.ItemType type;
        public string resourceName { get; private set; }
        public int sellPrice { get; private set; }
        public int buyPrice { get; private set; }

        public UnityAction<ItemSlot> OnfirstClickItemAction { get; private set; }
        public UnityAction OnSecondClickItemAction { get; private set; }
        public bool isShop { get; private set; }
        public bool isNew;
        public void SetClickActoion(UnityAction<ItemSlot> first, UnityAction second)
        {
            OnfirstClickItemAction = first;
            OnSecondClickItemAction = second;
        }

        public ItemSlotData(ItemData.ItemInfo itemInfo, bool _isShop = false)
        {
            
            id = itemInfo.id;
            count = itemInfo.value;
            isNew = itemInfo.isNew;
            isShop = _isShop;
          
            ItemDataScript script = ItemDataScript.GET_DATA(itemInfo.id);
            type = script.type;
            name = OptionDataManager.Instance.Language == UIDefine.Lang_Kind.Kor ? script.namekor : script.nameEn;
            desc = OptionDataManager.Instance.Language == UIDefine.Lang_Kind.Kor ? script.desckor : script.descEn;

          
            resourceName = ItemIconPath + "Item" + type.ToString() + id;

            buyPrice = script.buyPrice;
            sellPrice = script.sellPrice;
        }

        public ItemSlotData(ItemDataScript script, bool _isShop = false)
        {

            id = script.id;
            count = 0;
            isNew = false;
            isShop = _isShop;

            type = script.type;
            name = OptionDataManager.Instance.Language == UIDefine.Lang_Kind.Kor ? script.namekor : script.nameEn;
            desc = OptionDataManager.Instance.Language == UIDefine.Lang_Kind.Kor ? script.desckor : script.descEn;


            resourceName = ItemIconPath + "Item" + type.ToString() + id;

            buyPrice = script.buyPrice;
            sellPrice = script.sellPrice;
        }

    }

    public ItemSlotData itemData{ get; private set; }

    [SerializeField]
    private Image BackBoard;
    [SerializeField]
    private Sprite[] BackBoardSprites;

   [SerializeField]
    private GameObject ChoiceIcon;
    [SerializeField]
    private Image Icon;
    [SerializeField]
    private GameObject CountGroup;
    [SerializeField]
    private TextMeshProUGUI countText;

    [SerializeField]
    private GameObject newMark;

    [SerializeField]
    private TextMeshProUGUI NameText;


    // [SerializeField] private text itemSlotData;
   
    bool isClick;
    public void SetData(ItemSlotData data)
    {
        isClick = false;
        itemData = data;
        ChoiceIcon.SetActive(false);

        BackBoard.sprite = BackBoardSprites[(int)itemData.type];
        NameText.text = data.name;
        Icon.sprite = Resources.Load<Sprite>(data.resourceName);
        
        if (itemData.isShop)
        {
            newMark.SetActive(false);
            CountGroup.SetActive(false);
        }
        else
        {
            newMark.SetActive(data.isNew);
            countText.text = UIDefine.POINT_WITH_K_M(data.count);
        }



    }


    public void OnClickItem()
    {
        if (itemData.isShop)
        {
            if (!isClick)
            {
                itemData.OnfirstClickItemAction?.Invoke(this);
                isClick = true;
            }
            else
            {
                itemData.OnSecondClickItemAction?.Invoke();
             
            }

            ChoiceIcon.SetActive(isClick);
        }
        else
        {
            if(itemData.isNew)
            {
                ItemData.Instance.SetIsNew(itemData.id, !itemData.isNew);
                newMark.SetActive(false);
                itemData.isNew = false;
            }
        }
    }


    public void SetChoice(bool isChoice)
    {
        ChoiceIcon.SetActive(isChoice);
        isClick = isChoice;
    }


  


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;


public class ShopPopup : UIPopup
{
    /*
     todo : 

         판매 탭 
            판매시 아이템 감소 및 골드 상승 이팩트 넣기 

        구매 탭
            구매시 골드 감소 이팩트 넣기 
      
        

     */

    [Header("Buy")]
    [SerializeField]
    private GameObject BuyTab;
    [SerializeField]
    private TextMeshProUGUI DescText;
    [SerializeField]
    private TextMeshProUGUI BuyPiceText;
    [SerializeField]
    private Button BuyBtn;
    [SerializeField]
    private LoopVerticalScrollRect BuyScrollRect;

    [Header("Sell")]
    [SerializeField]
    private GameObject SellTab;
    [SerializeField]
    private TextMeshProUGUI SellPiceText;
    [SerializeField]
    private Button SellBtn;
    [SerializeField]
    private LoopVerticalScrollRect SellScrollRect;

    [Header("common")]
    [SerializeField]
    private GameObject[] TabBtn;

    private bool isBuyTab;
    private ItemSlot choiceItemSlot;

    
    public static void SHOW(bool isBuy = true)
    {
        UIPopup popup = PopupManager.Show(PopupKind.Shop, null);
        if (popup != null)
        {
            ((ShopPopup)popup).SetData(isBuy);
        }
    }


    public void SetData(bool isBuy)
    {
        TabBtn[0].SetActive(isBuy);
        TabBtn[1].SetActive(!isBuy);
        isBuyTab = isBuy;
        BuyTab.SetActive(isBuyTab);
        SellTab.SetActive(!isBuyTab);

        if(isBuyTab)
            SetBuyTab();
        else 
            SetSellTab();

    }


    private void SetBuyTab()
    {
        choiceItemSlot = null;
       var ItemSlotData = new List<ItemSlot.ItemSlotData>();
        var scripts = ItemDataScript.GET_DATA_LIST();
        foreach (var script in scripts)
        {
            // 현재는 스킬 카드만 판매 
            if (script.type == UIDefine.ItemType.SkillCard) 
            {
                var item = new ItemSlot.ItemSlotData(script, true);
                item.SetClickActoion(OnClickItem, OnClickBuyBtn);
                ItemSlotData.Add(item);
            }
        }

        // 출력시 아이디 순으로 자동 정렬 되어 출력 하기  
        ItemSlotData.Sort((x, y) => x.id.CompareTo(y.id));

        BuyScrollRect.init(ItemSlotData.ToArray());

        DescText.text = "";
        BuyPiceText.text = "0";
        BuyBtn.interactable = false;
   
    }

    private void SetSellTab()
    {
        choiceItemSlot = null;
        // 보유 아이템 데이터 가져오기 
        var datas = ItemData.Instance.GetItemDatas();
        if (datas == null)
            return;

        // 양식에 맞추어 아이템 슬롯 데이터 넣기 
        var ItemSlotSellData = new List<ItemSlot.ItemSlotData>();
        foreach (var data in datas)
        {
            if (data.value <= 0)
                continue;
            var item = new ItemSlot.ItemSlotData(data, true);
            item.SetClickActoion(OnClickItem, OnClickSellBtn);
            ItemSlotSellData.Add(item);
        }

        // 출력시 아이디 순으로 자동 정렬 되어 출력 하기  
        ItemSlotSellData.Sort((x, y) => x.id.CompareTo(y.id));

        SellScrollRect.init(ItemSlotSellData.ToArray());



        SellPiceText.text = "0";
        SellBtn.interactable = false;
    }



    public void OnClickTabButton()
    {     
        BuyTab.SetActive(!isBuyTab);
        SellTab.SetActive(isBuyTab);
        TabBtn[0].SetActive(!isBuyTab);
        TabBtn[1].SetActive(isBuyTab);

        if (isBuyTab)
        {
            SetSellTab();
        }
        else
        {
            SetBuyTab();
        }

        isBuyTab = !isBuyTab;

    }

   
    
    public void OnClickItem(ItemSlot itemSlot)
    {
        if (choiceItemSlot != null)
            choiceItemSlot.SetChoice(false);

        choiceItemSlot = itemSlot;

        if (isBuyTab)
        {
            DescText.text = itemSlot.itemData.desc;
            BuyPiceText.text = UIDefine.POINT_WITH_K_M(choiceItemSlot.itemData.buyPrice);
            bool hasPrice = UserData.Instance.Gold >= choiceItemSlot.itemData.buyPrice;
            BuyBtn.interactable = hasPrice;
            BuyPiceText.color = hasPrice? Color.white : Color.red;

        }
        else
        {        
            SellPiceText.text = UIDefine.POINT_WITH_K_M(choiceItemSlot.itemData.sellPrice);
            SellBtn.interactable = true;
        }
    }


    public void OnClickBuyBtn()
    {
        string name = choiceItemSlot.itemData.name;
        if(OptionDataManager.Instance.Language == UIDefine.Lang_Kind.Kor)
            name += Lang.Instance.ToHangleConsonant3(choiceItemSlot.name);
        int maxCount = UserData.Instance.Gold / choiceItemSlot.itemData.buyPrice;
        ShopNoticePopup.SHOW( string.Format(Lang.UI_MSG("NOTICE_BUY_ITEM"),name), maxCount, (count) => SetBuyItem(count));

      
    }

    private void SetBuyItem(int count)
    {
        // 아이템 추가
        ItemData.Instance.AddItemValue(choiceItemSlot.itemData.id, count);

        // 골드 차감 
        UserData.Instance.Gold -= (choiceItemSlot.itemData.buyPrice * count);
        MainView.Instance.ShowAddEff(true, false, choiceItemSlot.itemData.buyPrice * count);
        MainView.Instance.SetResourceData();

        SoundManager.Instance.PlayEffectAudioClip(SoundManager.EffSoundKind.Cash);

        choiceItemSlot.SetChoice(false);
        choiceItemSlot = null;
        DescText.text = "";
        BuyPiceText.text = "0";
        BuyBtn.interactable = false;
    }

    public void OnClickSellBtn()
    {
        string name = choiceItemSlot.itemData.name;
        if (OptionDataManager.Instance.Language == UIDefine.Lang_Kind.Kor)
            name += Lang.Instance.ToHangleConsonant3(choiceItemSlot.name);
      
        ShopNoticePopup.SHOW(string.Format(Lang.UI_MSG("NOTICE_SELL_ITEM"), name), choiceItemSlot.itemData.count, (count) => SetSellItem(count));

    }


    private void SetSellItem(int count)
    {
        // 아이템 차감
        ItemData.Instance.SubItemValue(choiceItemSlot.itemData.id, count);
        // 골드 추가
        UserData.Instance.Gold += (choiceItemSlot.itemData.sellPrice * count);
        MainView.Instance.ShowAddEff(true, true, choiceItemSlot.itemData.sellPrice * count);
        MainView.Instance.SetResourceData();
        SetSellTab();

        SoundManager.Instance.PlayEffectAudioClip(SoundManager.EffSoundKind.Cash);

    }






}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;


public class InventoryPopup : UIPopup
{
    /*
     todo : 
       아이템 종류 증가시, 정렬 버튼 추가하기  
       지금 현재는 재료 밖에 없음으로 타입 재로로 고정 중 / 추후에 아이템 추가시 수정 

     */
    [SerializeField]
    private LoopVerticalScrollRect scrollRect;


    public static void SHOW()
    {
        UIPopup popup = PopupManager.Show(PopupKind.Inventory, null);
        if (popup != null)
        {
            ((InventoryPopup)popup).SetData();
        }
    }


    public void SetData()
    {
        // 보유 아이템 데이터 가져오기 
        var datas = ItemData.Instance.GetItemDatas();
        if (datas == null)
            return;

        // 양식에 맞추어 아이템 슬롯 데이터 넣기 

        var ItemSlotData = new List<ItemSlot.ItemSlotData>();
        foreach (var data in datas)
        {

            if (data.value > 0)
                ItemSlotData.Add(new ItemSlot.ItemSlotData(data));
            
        }

        // 출력시 아이디 순으로 자동 정렬 되어 출력 하기  
        ItemSlotData.Sort((x,y) => x.id.CompareTo(y.id));

        scrollRect.init(ItemSlotData.ToArray());
    }









}

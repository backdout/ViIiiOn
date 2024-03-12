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
       ������ ���� ������, ���� ��ư �߰��ϱ�  
       ���� ����� ��� �ۿ� �������� Ÿ�� ��η� ���� �� / ���Ŀ� ������ �߰��� ���� 

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
        // ���� ������ ������ �������� 
        var datas = ItemData.Instance.GetItemDatas();
        if (datas == null)
            return;

        // ��Ŀ� ���߾� ������ ���� ������ �ֱ� 

        var ItemSlotData = new List<ItemSlot.ItemSlotData>();
        foreach (var data in datas)
        {

            if (data.value > 0)
                ItemSlotData.Add(new ItemSlot.ItemSlotData(data));
            
        }

        // ��½� ���̵� ������ �ڵ� ���� �Ǿ� ��� �ϱ�  
        ItemSlotData.Sort((x,y) => x.id.CompareTo(y.id));

        scrollRect.init(ItemSlotData.ToArray());
    }









}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UI;

public enum PopupKind : int
{
    invalid = -1,
    option = 0,
    BattleOption = 1,
    Notice =2,
    Inventory = 3,
    Shop = 4,
    ShopNotice = 5,
    Count
};

/// <summary>
/// ������� :
///   ����
///     1. UIPopupManager.Show
///     2. Stack push
///     3. Instantiate
///   �Ҹ�
///     1. UIPopup Close
///     2. UIPopup Disable
///     3. Statck Pop
/// </summary>
public class PopupManager : Singleton<PopupManager>
{
    const string CanvasName = "PopupCanvas";
    // ��üũ �ϴ� �κ� �߰� 
    public static Transform PopupCanvas;

    public class popupData
    {
        private PopupKind popup;
        private string data;

        public PopupKind POPUP
        {
            get { return popup; }
            private set { popup = value; }
        }
        public string DATA
        {
            get { return data; }
            private set { data = value; }
        }

        public popupData(PopupKind kind, string data)
        {
            this.POPUP = kind;
            this.DATA = data;
        }
    }

    //public static PopupManager Instance;
    // ��Ʈ�� ������ �˾� �̸� �߰���
    // ������(�ν��Ͻ�) �߰� ���, �˾����� �˾��� ��µǾ���� ��� ��� ������ ���������� ����.
    // �˾��� ��Ÿ�ӿ��� �����Ͽ� ���� �ϴ� ������ ����
    [Header("Prefabs")]
    private string[] PopupPrefabs = new string[(int)PopupKind.Count]
    {
      "OptionPopup",
      "BattleOptionPopup",
      "NoticePopup",
      "InventoryPopup",
      "ShopPopup",
      "ShopNoticePopup"
    };
    [Header("Variable")]
    private Stack<popupData> popupDataList = new Stack<popupData>();



    private void Awake()
    {
    
    }



    public static Transform GetTransform()
    {
        if (PopupCanvas == null)
        {
            PopupCanvas = GameObject.Find("Popup").transform;          
        }

        return PopupCanvas;
    }
    public static UIPopup Show(PopupKind kind, string data)
    {
        //if (Instance == null)
        //{
        //    if (LoadingScenePrefab.Instance.IsUIScene)
        //        Instance = GameObject.Find("PopupUI").GetComponent<UIPopupManager>();
        //}
        // PopupPrefabs ���� == Popup.Count ��ġ
        // �ٸ� �� �����̴�, �ش� enum �߰��ÿ� �ݵ�� �ν��������� �����Ұ�
        if (Enum.IsDefined(typeof(PopupKind), kind) == false || kind == PopupKind.Count)
            return null;

        // ���ÿ� �˾������� ����
        popupData popupData = new popupData(kind, data);
        Instance.popupDataList.Push(popupData);

        // ���� ���� �ִ� �˾� ���
        GameObject popup = Instance.MakePopup(Instance.popupDataList.Peek().POPUP);
        if (popup == null)
            return null;

        if (Instance.gameObject != null)
            popup.transform.SetParent(GetTransform());

        UIPopup popupComp = popup.GetComponent<UIPopup>();
        if (popupComp == null)
            return null;


        popupComp.InitPopupData(kind, data);
        popup.SetActive(true);

        return popupComp;
    }

    private GameObject MakePopup(PopupKind kind)
    {
        return Instantiate<GameObject>(Resources.Load<GameObject>(PopupPrefabs[(int)kind]), GetTransform(), false);
    }
    public void Close()
    {
        if (popupDataList.Count > 0)
            popupDataList.Pop();

    }

    public bool HasPopupKind(PopupKind kind)
    {
        bool rt = false;

        foreach (var data in popupDataList)
        {
            if (data.POPUP == kind)
                return true;
        }

        return rt;
    }
    public void AllClose()
    {
        Debug.Log("allCLose");
        if (popupDataList.Count > 0)
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                var child = this.transform.GetChild(i);
                if (child.GetComponent<UIPopup>() != null)
                    child.GetComponent<UIPopup>().Close();
            }

        }
    }

    public int GetPopupCount()
    {
        return popupDataList.Count;
    }

}

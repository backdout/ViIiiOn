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
/// 진행순서 :
///   생성
///     1. UIPopupManager.Show
///     2. Stack push
///     3. Instantiate
///   소멸
///     1. UIPopup Close
///     2. UIPopup Disable
///     3. Statck Pop
/// </summary>
public class PopupManager : Singleton<PopupManager>
{
    const string CanvasName = "PopupCanvas";
    // 씬체크 하는 부분 추가 
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
    // 스트링 값으로 팝업 이름 추가됨
    // 프리팹(인스턴스) 추가 경우, 팝업위에 팝업이 출력되어야할 경우 출력 순서가 뒤집어질수 있음.
    // 팝업을 런타임에서 생성하여 세팅 하는 구도로 변경
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
        // PopupPrefabs 개수 == Popup.Count 수치
        // 다를 시 오류이니, 해당 enum 추가시에 반드시 인스펙터쪽을 수정할것
        if (Enum.IsDefined(typeof(PopupKind), kind) == false || kind == PopupKind.Count)
            return null;

        // 스택에 팝업데이터 스택
        popupData popupData = new popupData(kind, data);
        Instance.popupDataList.Push(popupData);

        // 가장 위에 있는 팝업 출력
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

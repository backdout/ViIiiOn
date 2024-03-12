using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using TMPro;
using static SkillTreeDataScript;
/* 
 ���� : 
    ���� ���� ���۽� �ʿ��� ���� init �� Awake�� ����Ǵµ�, ����, �α��� ȭ�� �߰���, Awake ���ִ� �� �� InIt �̵��� �ʿ��ϴ�. 

    �߰��� ����, ������ �����ϰ�, �������� ������ �̿Ϸ� �Ǿ� ����� ���� �ξ��µ�, ���� ��� ������ �Ϸ�Ǹ� 
    MainUI �ν������� ViewList�� ViewKind �� �°� �ְ�, 
    ViewInit�� �ش� View �� Ŭ�� ������ �ʱ�ȭ�ؾ��ϴ� ���� �ִ´�. 
    ����, SetData ���� �� �� �������� �������� ���� ����ó���� �������ְ� (   ViewButtons[i].LockIcon.SetActive();)
    OnClickViewBtn �Լ����� Ŭ�� �����ϰ� ����ó���� �����Ѵ�. 
 
 
 */


public class MainView : Singleton<MainView>
{
    [Serializable]
    public class ViewBtn
    {
        public RectTransform rectTransform;
        public TextMeshProUGUI BtnText;
        public GameObject LockIcon;
      
    }
    public enum VIEW_KIND : int
    {

        NONE = -1,
        SKILLTREE = 0,
        HOME = 1,
        BATTLE = 2,
     
        COUNT = 3,
    }


    [Header("Top")]
    
    public TextMeshProUGUI TicketText;
    public TextMeshProUGUI GoldText;


    public GameObject MyinfoGroup;
    public TextMeshProUGUI UserName;
    public TextMeshProUGUI RankText;
    public Image RankIcon;
    public Sprite[] RankIconSprites;


    [Header("Mid")]
    public GameObject[] ViewList;


    [Header("Bottom")]
    public ViewBtn[] ViewButtons;
    public Sprite[] ButtonBackSprites;

    [Header("Etc")]
    public CanvasGroup NoticeGroup;
    public Text NoticeText;

    Coroutine[] OffAddEff = new Coroutine[2];
    GameObject ShopEffText;
    private int[] fontSize = { 60, 80 };
    private Vector2[] BtnSize = { new Vector2(220, 100), new Vector2(280, 150) };
    private VIEW_KIND currIndex;
    public bool isInit { get; private set; } = false;

    void Awake()
    {


        Application.targetFrameRate = 60;

        isInit = false;
        // ���� �����ؾ� �ϴ� ��
        LoadingScenePrefab.Instance.IsUIScene = true;


        //=====================================================
        Init();

    }

    public void Init(VIEW_KIND kind = VIEW_KIND.NONE)
    {
    
        //ù ���Խ�, Ȩ ���� 
        if (kind == VIEW_KIND.NONE)
        {
            if (currIndex == VIEW_KIND.NONE || currIndex == VIEW_KIND.SKILLTREE)
                currIndex = VIEW_KIND.HOME;
        }
        else
            currIndex = kind;

        SetData();

        isInit = true;

    }


    private void SetData()
    {
        isChange = false;

       // NoticeGroup.gameObject.SetActive(false);
        SetResourceData();

        SetMyinfoData();

        for (int i = 0; i < ViewButtons.Length; i++)
        {
            if (ViewList[i] != null)
                ViewList[i].SetActive(currIndex == (VIEW_KIND)i);
            SetButtonData(i, i == (int)currIndex);
            // ����� �ϴ� �ּ�
            //ViewButtons[i].LockIcon.SetActive(i == (int)VIEW_KIND.SKILLTREE);
            ViewButtons[i].LockIcon.SetActive(false);
            if ((VIEW_KIND)i == VIEW_KIND.HOME)
            {
               MyinfoGroup.SetActive(currIndex == VIEW_KIND.HOME);

            }
        }
        ViewInit((int)currIndex);

        // ������� üũ 
        //if (RecordManager.Instance.HasBattleRecord)
        //    ShowReplayNotice();
    }

    public void SetTopPanel()
    {
        SetResourceData();
        SetMyinfoData();
    }

    public void SetResourceData()
    {
        var user = UserData.Instance;

        TicketText.text = UIDefine.POINT_WITH_K_M(user.Ticket);
        GoldText.text = UIDefine.POINT_WITH_K_M(user.Gold);
    }

    private void SetMyinfoData()
    {
        var user = UserData.Instance;

        UserName.text = ""; // ���� ���� ���Ӱ��� Ȯ���Ǹ� ���� 
        //RnakText.text = ""
        //RankIcon.sprite =RankIconSprites[0];
    }


    private void SetButtonData(int index, bool isClick)
    {
        int spriteIndex = isClick ? 1 : 0;
        // ���� ��/���� ���� ���� �Ǵ� �κ� : �ؽ�Ʈ ũ��/ �۾� ũ�� 

        ViewButtons[index].rectTransform.sizeDelta = BtnSize[spriteIndex];
        ViewButtons[index].BtnText.fontSize = fontSize[spriteIndex];
      
    }





    public void ShowNotice()
    {
        if (CoShowNotice != null)
            StopCoroutine(CoShowNotice);

        CoShowNotice = StartCoroutine(coShowNotice());
    }



    public void OnClickViewBtn(int index)
    {
        //if (isChange)
        //    return;


        if ((int)currIndex == index)
            return;
        //������ ��� ��ư ���  >������ ���¿� ���� ���� ���� 
        //if (!(index == (int)VIEW_KIND.BATTLE || index == (int)VIEW_KIND.HOME))
        //{
        //   // NoticeText.text = Lang.UI_MSG("NOTICE_MSG_1");
        //   // ShowNotice();
        //    return;
        //}
        //
        //��ų Ʈ������ �̵��ϴ� ���, ��ųƮ������ ���泻�� ����
        if (currIndex == VIEW_KIND.SKILLTREE)
        {
            if(ViewList[0].GetComponent<SkillTreeView>().isBuy)
            {

                if (GoogleSignInManager.Instance.HasLogin())
                {
                    FirestoreDBManager.Instance.SaveUserData();
                    FirestoreDBManager.Instance.SaveItemdata();
                }
                else
                {
                    RecordManager.Instance.SetSaveSkillViewData();
                    RecordManager.Instance.SetSaveItemDatas(ItemData.Instance.GetItemDatas().ToArray());
                }    
            }

        }



        // ����� �������� Ȩ �信���� ��� 
        MyinfoGroup.SetActive((VIEW_KIND)index == VIEW_KIND.HOME);


        ViewInit(index);
       


        //// ���� �� active off
        if (ViewList[(int)currIndex] != null)
            ViewList[(int)currIndex].SetActive(false);
        SetButtonData((int)currIndex, false);




        SetButtonData(index, true);
        if (ViewList[(int)index] != null)
            ViewList[index].SetActive(true);

        // ��ư �ִ� 
        if(CoAniSelectButton!=null)
            StopCoroutine(CoAniSelectButton);

        StartCoroutine(AniSelectButton(index));


    }

    private void ViewInit(int index)
    {
        //    // �� ��� ���� 
        switch ((VIEW_KIND)index)
        {
            case VIEW_KIND.SKILLTREE:
                ViewList[0].SetActive(true);
                ViewList[0].GetComponent<SkillTreeView>().Init();
                break;


            case VIEW_KIND.BATTLE:
                ViewList[2].SetActive(true);
                ViewList[2].GetComponent<StageView>().Init();
                break;

            case VIEW_KIND.HOME:

                break;

        }

    }



    Coroutine CoShowNotice;
    IEnumerator coShowNotice()
    {
        float t = 0;
        NoticeGroup.alpha = 0;
        NoticeGroup.gameObject.SetActive(true);
        while (t < 1)
        {
            if (t < 0.5f)
                NoticeGroup.alpha = t * 2;
            else
                NoticeGroup.alpha = 1f - ((t - 0.5f) * 2f);

            t += Time.deltaTime;

            yield return null;
        }
        NoticeGroup.alpha = 0;
        NoticeGroup.gameObject.SetActive(false);
    }
    Coroutine CoAniSelectButton;
    bool isChange;
    IEnumerator AniSelectButton(int nextIndex)
    {

        isChange = true;

        float t = 0;

        int prevIdex = (int)currIndex;

        while (t <= 0.10000f)
        {

            ViewButtons[prevIdex].rectTransform.sizeDelta = Vector2.Lerp(BtnSize[1], BtnSize[0], t * 10f);
            ViewButtons[nextIndex].rectTransform.sizeDelta = Vector2.Lerp(BtnSize[0], BtnSize[1], t * 10f);


            t += Time.deltaTime;
            yield return null;
        }
        ViewButtons[prevIdex].rectTransform.sizeDelta = BtnSize[0];

        ViewButtons[nextIndex].rectTransform.sizeDelta = BtnSize[1];


        currIndex = (VIEW_KIND)nextIndex;
      
        isChange = false;
    }

    public void OnClickOption()
    {
        OptionPopup.SHOW();
    }

    public void OnClickInventory()
    {
        InventoryPopup.SHOW();
    }


    public void OnClickShop()
    {
        ShopPopup.SHOW();
    }


    public void OnClickTicketPlus()
    {
        // ������ ���ŷ� �̵� 
        ShopPopup.SHOW();
        // ���� ����
        OnClickAddResourse();
    }
    public void OnClickGoldPlus()
    {
        // ������ �Ǹŷ� ����
        ShopPopup.SHOW(false);
        // ���� ����
        OnClickSave();
    }



    public void ShowAddEff(bool IsShowEffGold, bool isAdd, int num)
    {
       
        GameObject parent = IsShowEffGold ? GoldText.gameObject : TicketText.gameObject;

        GameObject AddTextGo = InstantiateShopEffText();
        
        var AddText = AddTextGo.GetComponent<ShopEffFont>();

        if (AddText != null)
        {
            AddText.Show(isAdd, num, parent);
        }

    }


    private GameObject InstantiateShopEffText()
    {
        if (ShopEffText == null)
            ShopEffText = Resources.Load<GameObject>("ShopEffFont");

        GameObject g = ObjectPoolManager.Instance.doInstantiate(ShopEffText);

        g.SetActive(false);

        return g;
    }


    #region ġƮ�� ���� ��������
    // ��ȭ ġƮ��
    public void OnClickAddResourse()
    {
        UserData.Instance.Gold += 10000;
        UserData.Instance.Ticket += 5;
       
    }
    // ���� ���̺�� 
    public void OnClickSave()
    {
        if (GoogleSignInManager.Instance.HasLogin())
        {
            FirestoreDBManager.Instance.SaveUserData();
            FirestoreDBManager.Instance.SaveItemdata();
            Debug.Log("GoogleSign Userdata Save");
        }
        else
        {
            RecordManager.Instance.SetSaveUserData(RecordManager.UserDataKey.All);
            RecordManager.Instance.SetSaveItemDatas(ItemData.Instance.GetItemDatas().ToArray());
            Debug.Log("Userdata Save");
        }
    }

    #endregion





}

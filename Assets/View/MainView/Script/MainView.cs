using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using TMPro;
using static SkillTreeDataScript;
/* 
 참고 : 
    현재 게임 시작시 필요한 각종 init 이 Awake에 싷행되는데, 추후, 로그인 화면 추가시, Awake 에있는 각 종 InIt 이동이 필요하다. 

    추가로 현재, 전투를 제외하고, 컨텐츠가 개발이 미완료 되어 기능을 막아 두었는데, 추후 기능 개발이 완료되면 
    MainUI 인스펙터의 ViewList에 ViewKind 에 맞게 넣고, 
    ViewInit에 해당 View 를 클릭 했을때 초기화해야하는 값을 넣는다. 
    이후, SetData 에서 각 뷰 아이콘의 락아이콘 설정 예외처리를 수정해주고 (   ViewButtons[i].LockIcon.SetActive();)
    OnClickViewBtn 함수에서 클릭 가능하게 예외처리를 수정한다. 
 
 
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
        // 먼저 셋팅해야 하는 값
        LoadingScenePrefab.Instance.IsUIScene = true;


        //=====================================================
        Init();

    }

    public void Init(VIEW_KIND kind = VIEW_KIND.NONE)
    {
    
        //첫 진입시, 홈 고정 
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
            // 락기능 일단 주석
            //ViewButtons[i].LockIcon.SetActive(i == (int)VIEW_KIND.SKILLTREE);
            ViewButtons[i].LockIcon.SetActive(false);
            if ((VIEW_KIND)i == VIEW_KIND.HOME)
            {
               MyinfoGroup.SetActive(currIndex == VIEW_KIND.HOME);

            }
        }
        ViewInit((int)currIndex);

        // 전투기록 체크 
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

        UserName.text = ""; // 추후 유저 네임관련 확정되면 수정 
        //RnakText.text = ""
        //RankIcon.sprite =RankIconSprites[0];
    }


    private void SetButtonData(int index, bool isClick)
    {
        int spriteIndex = isClick ? 1 : 0;
        // 선택 유/무에 따라 변경 되는 부분 : 텍스트 크기/ 글씨 크기 

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
        //지금은 모든 버튼 잠김  >컨텐츠 오픈에 따라 조건 변경 
        //if (!(index == (int)VIEW_KIND.BATTLE || index == (int)VIEW_KIND.HOME))
        //{
        //   // NoticeText.text = Lang.UI_MSG("NOTICE_MSG_1");
        //   // ShowNotice();
        //    return;
        //}
        //
        //스킬 트리에서 이동하는 경우, 스킬트리에서 변경내역 저장
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



        // 상단의 내정보는 홈 뷰에서만 출력 
        MyinfoGroup.SetActive((VIEW_KIND)index == VIEW_KIND.HOME);


        ViewInit(index);
       


        //// 현재 뷰 active off
        if (ViewList[(int)currIndex] != null)
            ViewList[(int)currIndex].SetActive(false);
        SetButtonData((int)currIndex, false);




        SetButtonData(index, true);
        if (ViewList[(int)index] != null)
            ViewList[index].SetActive(true);

        // 버튼 애니 
        if(CoAniSelectButton!=null)
            StopCoroutine(CoAniSelectButton);

        StartCoroutine(AniSelectButton(index));


    }

    private void ViewInit(int index)
    {
        //    // 뷰 출력 셋팅 
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
        // 아이템 구매로 이동 
        ShopPopup.SHOW();
        // 추후 삭제
        OnClickAddResourse();
    }
    public void OnClickGoldPlus()
    {
        // 아이템 판매로 유도
        ShopPopup.SHOW(false);
        // 추후 삭제
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


    #region 치트용 추후 삭제예정
    // 재화 치트용
    public void OnClickAddResourse()
    {
        UserData.Instance.Gold += 10000;
        UserData.Instance.Ticket += 5;
       
    }
    // 수동 세이브용 
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

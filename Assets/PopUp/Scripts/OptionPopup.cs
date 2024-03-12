using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using static UnityEngine.Rendering.DebugUI;
using System.Security.Cryptography;

public class OptionPopup : UIPopup
{

    [Serializable]
    public class OptioinBtn
    {
        public GameObject OnButton;
        public GameObject OffButton;
        private bool _isOn;

        public bool isOn
        {
            get { return _isOn; }
            set
            {
                _isOn = value;
                OnButton.SetActive(value);
                OffButton.SetActive(!value);

            }
        }
    }



    /// <summary>
    /// 언어를 제외한 셋팅 버튼 //RecordManager.OptionData 순
    /// </summary>
    public OptioinBtn[] Buttons;
    public Text LoginText;
    public Text LangText;
    public GameObject DeleteBtn;
    private UIDefine.Lang_Kind lang;

    protected override void OnEnable()
    {
        base.OnEnable();

    }

    protected override void OnDisable()
    {
        base.OnDisable();

    }

    public enum OptionPopupKind
    {
        MainOption,
        BattleOption,

    }


    public static void SHOW(OptionPopupKind kind = OptionPopupKind.MainOption)
    {
        PopupKind popupKind = kind == OptionPopupKind.MainOption ? PopupKind.option : PopupKind.BattleOption;

        UIPopup popup = PopupManager.Show(popupKind, null);
        if (popup != null)
        {
            ((OptionPopup)popup).SetData(kind == OptionPopupKind.MainOption);
        }

    }


    public void SetData(bool IsMainOption)
    {
        CloseAction = AddCloseAction;
        var optionData = OptionDataManager.Instance;
        for (int i = 0; i < Buttons.Length; i++)
            Buttons[i].isOn = optionData.GetOptionData((RecordManager.OptionData)i);

        if (IsMainOption)
        {
            lang = optionData.Language;

            LangText.text = lang.ToString().ToUpper();
            LoginText.text = optionData.GoogleLogin ? "LOGOUT" : "LOGIN";

        }


    }


    public void OnClickOption(int index)
    {
        Buttons[index].isOn = Buttons[index].isOn == true ? false : true;
        
    }


    public void OnClickLogin()
    {
        if (GoogleSignInManager.Instance.HasLogin())
            NoticePopup.SHOW(NoticePopup.ButtonType.YesAndNo, Lang.UI_MSG("QUESTION_LOGOUT"), () => CallLogOut());
        else
            NoticePopup.SHOW(NoticePopup.ButtonType.YesAndNo, Lang.UI_MSG("QUESTION_LOGIN"), () => CallLogin());


    }


    private void CallLogin()
    {
        if (coCallLogin != null)
            StopCoroutine(coCallLogin);

        coCallLogin = StartCoroutine(CoCallLogin());

    }

    Coroutine coCallLogin;
    //IEnumerator
    IEnumerator CoCallLogin()
    {
        GoogleSignInManager.Instance.SignInWithGoogle(true);
        if (!GoogleSignInManager.Instance.isComplete)
        {
            //BackBoard.SetActive(true);
            // GoogleSignInManager.Instance.Awake();
            yield return new WaitUntil(() => GoogleSignInManager.Instance.isComplete);
        }
       

        string rt = GoogleSignInManager.Instance.retuenResult;

        //1. 로그인 성공 여부 
        if (rt == "NOTICE_LOGIN_CONNECT")
        {//1-1 로그인 성공
         // 2. 연결 계정에 데이터 유/무 체크 

            FirestoreDBManager.Instance.CheckUserData();
            yield return new WaitUntil(() => FirestoreDBManager.Instance.IsComplete);

            if (FirestoreDBManager.Instance.HasUserData)
            { //2-1. 계정에 데이터 있음 
              // 2-1-1 클라에 데이터가 있음 -> 덮어 씌울지 문의 
                if (RecordManager.Instance.HasUserData())
                {
                    string notic = Lang.UI_MSG("QUESTION_LOGIN_CONNECT") + "\n\n" + Lang.UI_MSG("NOTICE_LOGIN_WARRNING_1");
                    NoticePopup.SHOW(NoticePopup.ButtonType.YesAndNo, Lang.UI_MSG(notic), () => OptionDataManager.Instance.SetLogin(true), () => OptionDataManager.Instance.SetLogin(false));
                    yield return null;
                }
                else
                {// 2-1-2 클라에 데이터가 없음 -=> 계정에 저장된 데이터 호출 

                    OptionDataManager.Instance.SetLogin(false);
                    yield return null;
                    
                }

            }
            else//2-2 계정에 데이터 없음
            {
                // 클라의 데이터 유무에 따라 데이터 연동 
                OptionDataManager.Instance.SetLogin(RecordManager.Instance.HasUserData(), false);
                NoticePopup.SHOW(NoticePopup.ButtonType.Ok, Lang.UI_MSG(rt), null);
                yield return null;
            }

        }
        else
        { //1-2. 로그인 실패 
            NoticePopup.SHOW(NoticePopup.ButtonType.Ok, Lang.UI_MSG(rt), null);
            yield return null;
        }
     
        
    }

  
    public void CallLogOut()
    {
        //로그아웃 처리 => 통신이 없음 
        var rt = GoogleSignInManager.Instance.SignOutFromGoogle(false);


        // 데이터 재처리는 타이틀 화면에서 진행 




        // 타이틀로 복귀 
        OptionDataManager.Instance.SetOptionData(RecordManager.OptionData.Login, false);
        NoticePopup.SHOW(NoticePopup.ButtonType.Ok, Lang.UI_MSG(rt), () => LoadingScenePrefab.Instance.LoadScene("Title"));
    }



    public void OnClickLang()
    {
        if (lang + 1 >= UIDefine.Lang_Kind.Count)
            lang = 0;
        else
            lang += 1;

        LangText.text = lang.ToString().ToUpper();
    }


    private void OptionSave()
    {
        var optionData = OptionDataManager.Instance;
        for (int i = 0; i < Buttons.Length; i++)
            optionData.SetOptionData((RecordManager.OptionData)i, Buttons[i].isOn);


        // 사운드 
        SoundManager.Instance.SetIsBgmOn(optionData.BgmSound);
        SoundManager.Instance.SetIsEffOn(optionData.EffectSound);
        // 로그인 기능이 없음으로 현재는 무조건 false;
        optionData.SetOptionData(RecordManager.OptionData.Login, false);
        bool isLangChange = lang != optionData.Language;
        optionData.SetOptionData(RecordManager.OptionData.Lang, false, lang);



        RecordManager.Instance.SetSaveOptionData(RecordManager.OptionData.All);
        if (isLangChange)
        {
            optionData.SetLocalLang(lang);
            MainView.Instance.Init();
        }
    }

    public void OnClickSave()
    {
        OptionSave();
        Close();
    }


    public void AddCloseAction()
    {
      //  SoundManager.Instance.Init();
    }


    public void OnClickBattleOptionClose()
    {
        OptionSave();
        BattleSceneManager.Instance.battleUI.SetJoyStick(OptionDataManager.Instance.Joystick);
        Close();
        BattleSceneManager.Instance.battleManager.IsPush = false;
    }
    public void OnClickReturnHome()
    {
       
        NoticePopup.SHOW(NoticePopup.ButtonType.YesAndNo, Lang.UI_MSG("NOTICE_EXIT_STAGE"), () => ChangeScene());
     
    }

    private void ChangeScene()
    {
        LoadingScenePrefab.Instance.LoadScene("Main");
        BattleSceneManager.Instance.battleManager.IsPush = false;

    }

public void OnclickDataDeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
}

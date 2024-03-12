using System.Collections;

using UnityEngine;

using UnityEngine.Localization.Settings;
using System;


public class OptionDataManager : Singleton<OptionDataManager>
{
    public bool IsInit { get; private set; } = false;
    public int getLocal()
    {
        SetDefaultLang();
        return local;
    }

    private void SetDefaultLang()
    {
        local = (int)UIDefine.Lang_Kind.Eng;
        switch (Application.systemLanguage)
        {

            case SystemLanguage.Korean:
                local = (int)UIDefine.Lang_Kind.Kor;
                DEFAULT_LANG = UIDefine.Lang_Kind.Kor;
                break;

            default:
                local = (int)UIDefine.Lang_Kind.Eng;
                DEFAULT_LANG = UIDefine.Lang_Kind.Eng;
                break;
        }
        hasLocal = true;
    }

    
    bool hasLocal;
   

    public bool EffectSound { get; private set; }
    public bool BgmSound { get; private set; }
    public bool Vibration { get; private set; }

    public bool Joystick { get; private set; }
    public UIDefine.Lang_Kind Language { get; private set; }
    public bool GoogleLogin { get; private set; }


    public UIDefine.Lang_Kind DEFAULT_LANG { get; private set; }

    public int local { get; private set; }

   

    public void Init()
    {
        SetLoadData();
        IsInit = true;
    }


    bool isLangChange;
    private IEnumerator ISetLocalLang(int index)
    {
        isLangChange = true;

        yield return LocalizationSettings.InitializationOperation;

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        Language = (UIDefine.Lang_Kind)index;
        isLangChange = false;

    }

    public  void SetLocalLang(UIDefine.Lang_Kind Lang_Kind)
    {
        if (isLangChange)
            return;

        StartCoroutine(ISetLocalLang((int)Lang_Kind));
    }


    /// <summary>
    /// 설정창에서 언어 변경시, 호출되는 함수 
    /// </summary>
    public void OnClikChangeLang(int index)
    {
        SetLocalLang((UIDefine.Lang_Kind)index);
    }

    public void SetLoadData()
    {
        if (!hasLocal)
            SetDefaultLang();

        var record = RecordManager.Instance;
        string value = record.GetLoadOptionData(RecordManager.OptionData.EffectSound);
        EffectSound = string.IsNullOrEmpty(value) ? true : bool.Parse(value.ToLower());

        value = record.GetLoadOptionData(RecordManager.OptionData.BgmSound);
        BgmSound = string.IsNullOrEmpty(value) ? true : bool.Parse(value.ToLower());

        value = record.GetLoadOptionData(RecordManager.OptionData.Vibration);
        Vibration = string.IsNullOrEmpty(value) ? true : bool.Parse(value.ToLower());

        value = record.GetLoadOptionData(RecordManager.OptionData.Login);
        GoogleLogin = string.IsNullOrEmpty(value) ? false : bool.Parse(value.ToLower());

        value = record.GetLoadOptionData(RecordManager.OptionData.Joystick);
        Joystick = string.IsNullOrEmpty(value) ? true : bool.Parse(value.ToLower());

        value = record.GetLoadOptionData(RecordManager.OptionData.Lang);
        Language = string.IsNullOrEmpty(value) ? DEFAULT_LANG : (UIDefine.Lang_Kind)Enum.Parse(typeof(UIDefine.Lang_Kind), value);
        SetLocalLang(Language);


    }

    /// <summary>
    /// 언어를 제외한 옵션데이터 반환
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public bool GetOptionData(RecordManager.OptionData option)
    {
        bool rt = false;
        switch (option)
        {
            case RecordManager.OptionData.EffectSound:
                return EffectSound;
            case RecordManager.OptionData.BgmSound:
                return BgmSound;
            case RecordManager.OptionData.Vibration:
                return Vibration;
            case RecordManager.OptionData.Login:
                return GoogleLogin;
            case RecordManager.OptionData.Joystick:
                return Joystick;

        }

        return rt;

    }

    /// <summary>
    /// 언어를 제외한 옵션데이터 설정
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public void SetOptionData(RecordManager.OptionData option, bool value, UIDefine.Lang_Kind lang_Kind = UIDefine.Lang_Kind.Count)
    {
        switch (option)
        {
            case RecordManager.OptionData.EffectSound:
                EffectSound = value;
                break;
            case RecordManager.OptionData.BgmSound:
                BgmSound = value;
                break;
            case RecordManager.OptionData.Vibration:
                Vibration = value;
                break;
            case RecordManager.OptionData.Joystick:
                Joystick = value;
                break;
            case RecordManager.OptionData.Login:
                GoogleLogin = value;
                break;
            case RecordManager.OptionData.Lang:
                Language = lang_Kind;
                break;
        }

    }

    public void SetLogin(bool HasConnectData, bool hasFBData = true, bool isTitle =false)
    {
        //로그인 처리 
        //  var rt = GoogleSignInManager.Instance.SignInWithGoogle(false);


        // 옵션 데이터 저장 처리 - 추후 파베  RDB 추가시 추가 
        //
        // 클라에 데이터가 있고, 연동 하는 경우, 
        if (HasConnectData)
        {
            UserData.Instance.SetLoadRecordUserData();
            FirestoreDBManager.Instance.SaveUserData();
            FirestoreDBManager.Instance.SaveItemdata();
            RecordManager.Instance.DeleteAllData();
        }
        else 
        {
             // 클라에 데이터가 없고, 연동된 계정에 데이터가 없는 경우,    
            if (!hasFBData)
            {
                FirestoreDBManager.Instance.SaveDefaultUserData();
                FirestoreDBManager.Instance.SaveDefaultItemdata();
                FirestoreDBManager.Instance.SaveItemdata();
            }
        }

        // 로그인 성공시, 타이틀로 복귀 
        SetOptionData(RecordManager.OptionData.Login, true);
        if (isTitle)
            NoticePopup.SHOW(NoticePopup.ButtonType.Ok, Lang.UI_MSG("NOTICE_LOGIN_CONNECT"), () => TitleView.Instance.ReInit());
        else
            NoticePopup.SHOW(NoticePopup.ButtonType.Ok, Lang.UI_MSG("NOTICE_LOGIN_CONNECT"), () => MoveToTitle());

    }


    public void MoveToTitle()
    {
        LoadingScenePrefab.Instance.LoadScene("Title");
    }



}

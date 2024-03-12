using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using static MainView;

public class TitleView : Singleton<TitleView>
{
    /*
     로그인 여부 체크 
    
     각종 데이터 셋팅 (init)

     로그인 여부에 따른 버튼 출력 
            
     */

    public GameObject BackBoard;
    private bool HasLogin;

    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button loginButton;



    public void Init()
    {

        if (!ScriptManager.Instance.IsInit)
            ScriptManager.Instance.Init();

        if (!ObjectPoolManager.Instance.IsInit)
            ObjectPoolManager.Instance.Init();
        
        if (!Lang.Instance.IsInit)
            Lang.Instance.Init();

        if (!OptionDataManager.Instance.IsInit)
            OptionDataManager.Instance.Init();

        if (!SoundManager.Instance.IsInit)
            SoundManager.Instance.Init();

    
    }

    

    private void initData()
    {
        UserData.Instance.Init();
       // HeroData.Instance.Init();
        ItemData.Instance.Init();

    }

    private void Awake()
    {
        Init();
    
    }



    Coroutine coSetButton;
    //IEnumerator
    IEnumerator CoHasLogin()
    {
        if (!GoogleSignInManager.Instance.hasInit)
        {
            //BackBoard.SetActive(true);
           // GoogleSignInManager.Instance.Awake();
            yield return new WaitUntil(() => GoogleSignInManager.Instance.hasInit);
        }
        //BackBoard.SetActive(false);
        loginButton.gameObject.SetActive(GoogleSignInManager.Instance.HasLogin() == false);
        initData();
        yield return null;
    }





    // Start is called before the first frame update
    void Start()
    {
       
        if (coSetButton != null)
            StopCoroutine(coSetButton);

        coSetButton = StartCoroutine(CoHasLogin());
    }


    public void OnClickLogin()
    {

        NoticePopup.SHOW(NoticePopup.ButtonType.YesAndNo, Lang.UI_MSG("QUESTION_LOGIN"), () => CallLogin());

    }

    private void CallLogin()
    {
        if (coCallLogin != null)
            StopCoroutine(coCallLogin);

        coCallLogin = StartCoroutine(CoCallLogin());

        // GoogleSignInManager.Instance.SignInWithGoogle(true);
        //  NoticePopup.SHOW(NoticePopup.ButtonType.Ok, Lang.UI_MSG(rt), () => ReInit()); 
    }


    Coroutine coCallLogin;
    //IEnumerator
    IEnumerator CoCallLogin()
    {
        string rt = "";
        GoogleSignInManager.Instance.SignInWithGoogle(true);
 
        if (!GoogleSignInManager.Instance.isComplete)
        {
            //BackBoard.SetActive(true);
            // GoogleSignInManager.Instance.Awake();
            yield return new WaitUntil(() => GoogleSignInManager.Instance.isComplete);
           
        }
        rt = GoogleSignInManager.Instance.retuenResult;
        
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
                    NoticePopup.SHOW(NoticePopup.ButtonType.YesAndNo, notic,
                   () => //예 버튼
                   {
                       initData();
                       OptionDataManager.Instance.SetLogin(true, true, true);
                    }, 
                      () =>//아니오 버튼
                   OptionDataManager.Instance.SetLogin(false, true, true));
                    yield return null;
                }
                else
                {// 2-1-2 클라에 데이터가 없음 -=> 계정에 저장된 데이터 호출 

                    OptionDataManager.Instance.SetLogin(false, true, true);
                    yield return null;
                }

            }
            else//2-2 계정에 데이터 없음
            {              
                // 클라의 데이터 유무에 따라 데이터 연동 
                OptionDataManager.Instance.SetLogin(RecordManager.Instance.HasUserData(), false, true);    
                yield return null;
            }

        }
        else
        { //1-2. 로그인 실패 
            NoticePopup.SHOW(NoticePopup.ButtonType.Ok, Lang.UI_MSG(rt), null);
            yield return null;
        }

        yield return null;
    }




    public void ReInit()
    {
        initData();
        loginButton.gameObject.SetActive(GoogleSignInManager.Instance.HasLogin() == false);
    }

    public void OnClickStart()
    {
        LoadingScenePrefab.Instance.LoadScene("Main");
    }

    public void OnClickDeletedata()
    {
       RecordManager.Instance.DeleteAllData();
    }


}

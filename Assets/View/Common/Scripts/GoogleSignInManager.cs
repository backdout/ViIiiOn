using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GoogleSignInManager : Singleton<GoogleSignInManager>
{
    private GameObject BackBoard;
    public string webClientId = "681517541863-ol4rm9vg7neu8cj8blscnhtgmj57go5t.apps.googleusercontent.com";

    private FirebaseAuth auth;
    private GoogleSignInConfiguration configuration;

    public FirebaseUser currentUser { get; private set; }
    public string retuenResult = "NOTICE_LOGIN_WARRNING_2"; // 기본 실패로 

    public bool hasInit;
    public bool isComplete;
    private void Awake()
    {
        hasInit = false;
        configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
        CheckFirebaseDependencies();

        
    }

    string currSceneName;

    private void CheckFirebaseDependencies()
    {
        SetBackBoard();
        BackBoard.SetActive(true);
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result == DependencyStatus.Available)
                {
                    auth = FirebaseAuth.DefaultInstance;
                    currentUser = auth.CurrentUser;
                }
                else
                    ShowLoginLog("Could not resolve all Firebase dependencies: " + task.Result.ToString());
            }
            else
            {
                ShowLoginLog("Dependency check was not completed. Error : " + task.Exception.Message);
            }
            BackBoard.SetActive(false);
            hasInit = true;
        });
    }

    private void SetBackBoard()
    {
        if (BackBoard == null)
        {
            BackBoard = Resources.Load<GameObject>("BackBoard");
        }

        var scene = SceneManager.GetActiveScene();
        if (currSceneName != scene.name)
        {
            currSceneName = scene.name;

            var pa = GameObject.Find("Popup");

            if (pa != null)
            {
                //로드된 프리팹의 부모를 바로 바꾸면 에러 발생
                GameObject instance = Instantiate(BackBoard) as GameObject;
                instance.SetParent(pa);
              
            }
        }

    }


    public void SignInWithGoogle(bool _isTitle) 
    {
        isComplete = false;
        isTitle = _isTitle;
#if UNITY_EDITOR
        
        OnSignInMail();
#else
         OnSignIn();
#endif
    }
    public string SignOutFromGoogle(bool _isTitle)
    {
        isTitle = _isTitle;
#if UNITY_EDITOR
        return OnSignOutMail();
#else
        return OnSignOut();
#endif
    }

    bool isTitle;
    private string OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        SetBackBoard();
        BackBoard.SetActive(true);
     
        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(OnAuthenticationFinished);        
        return retuenResult;
    }

    private string OnSignOut()
    {
        ShowLoginLog("Calling SignOut");
 
        GoogleSignIn.DefaultInstance.SignOut();
        SetRetuenResult("NOTICE_LOGOUT");
        return retuenResult;
    }


    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        ShowLoginLog("Sign start");
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    ShowLoginLog("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    ShowLoginLog("Got Unexpected Exception?!?" + task.Exception);
                }
           
                SetRetuenResult("NOTICE_LOGIN_WARRNING_2");
             
            }
            isComplete = true;
        }
        else if (task.IsCanceled)
        {
            ShowLoginLog("Canceled");          
            SetRetuenResult("NOTICE_LOGIN_WARRNING_2");
            isComplete = true;
        }
        else
        {
            SignInWithGoogleOnFirebase(task.Result.IdToken);
                   
        }     
        BackBoard.SetActive(false);
    }

    private void SignInWithGoogleOnFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
      
        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            AggregateException ex = task.Exception;
            if (ex != null)
            {
                if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                    ShowLoginLog("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
                SetRetuenResult("NOTICE_LOGIN_WARRNING_2");
                isComplete = true;
            }
            else
            {
                
                SetRetuenResult("NOTICE_LOGIN_CONNECT");
                if(currentUser == null)
                {
                    if(auth.CurrentUser == null) { ShowLoginLog("currentUser null"); }
                    currentUser = auth.CurrentUser;
                    
                }
                FirestoreDBManager.Instance.SetUid(currentUser.UserId);
                ///configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };              
                isComplete = true;
                ShowLoginLog("Sign In Successful. isComplete");
            }
            
         
        });
    }

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        ShowLoginLog("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWithOnMainThread(OnAuthenticationFinished);
    }

    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        ShowLoginLog("Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(OnAuthenticationFinished);
    }


    private void ShowLoginLog(string str)
    {

        Debug.Log(str);
    }

    private void SetRetuenResult(string str)
    {
        retuenResult = str;
    }

   
    public bool HasLogin()
    {
        if(auth.CurrentUser == null)
            return false;
     
        return string.IsNullOrEmpty(auth.CurrentUser.Email) == false;

    }



#if UNITY_EDITOR
    
    string mail = "tahasa011@gmail.com";
    string pw = "Password";

    private void OnSignInMail()
    {
        SetBackBoard();
        BackBoard.SetActive(true);

        auth.SignInWithEmailAndPasswordAsync(mail, pw).ContinueWith(task =>
        {
            if(task.Result == null || task.IsFaulted )
            {
                ShowLoginLog("Mail IsFaulted");
                OnCreateMail();
            }
           else
           {   
                SetRetuenResult("NOTICE_LOGIN_CONNECT");              
           }
            isComplete = true;
            BackBoard.SetActive(false);
            ShowLoginLog("Calling SignIn");
        });

      
    }

    private void OnCreateMail()
    {
        auth.CreateUserWithEmailAndPasswordAsync(mail, pw).ContinueWith(creat =>
        {

            if (creat.IsFaulted)
                Debug.Log("가입 실패");
            if (creat.IsCanceled)
                Debug.Log("가입 취소");

            //NoticePopup.SHOW(NoticePopup.ButtonType.Ok, Lang.UI_MSG(retuenResult), () => TitleView.Instance.ReInit());

        });


    }


    private string OnSignOutMail()
    {
        ShowLoginLog("Calling SignOut");
        auth.SignOut();
        SetRetuenResult("NOTICE_LOGOUT");
        return retuenResult;
    }





#endif
}

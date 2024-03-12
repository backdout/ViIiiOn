using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static MainView;

public class LoadingScenePrefab : Singleton<LoadingScenePrefab>
{

    [Header("UI")]
    public CanvasGroup canvasGroup;
    //  public GaugeUI progressBar;

    [Header("Variable")]
    //private readonly float fullGaugeSize = 996f;
    private string loadsceneName;

    public bool IsUIScene;
    public MainView.VIEW_KIND currMainView;

    public void LoadScene(string sceneName)
    {
        // this.gameObject.SetActive(true);
        SceneManager.sceneLoaded += OnSceneLoaded;
        loadsceneName = sceneName;
        SceneManager.LoadSceneAsync(loadsceneName);
        // StartCoroutine(LoadSceneProcess());
    }
    public void LoadMainScene()
    {
        LoadScene("Main");
    }

    // 종료 될때, 데이터 저장 
    void OnApplicationQuit()
    {
        if (GoogleSignInManager.Instance.HasLogin())
        {
            FirestoreDBManager.Instance.SaveUserData();
            FirestoreDBManager.Instance.SaveItemdata();
            Debug.Log("Userdata Save");
        }
        else
        {
            RecordManager.Instance.SetSaveUserData(RecordManager.UserDataKey.All);
            RecordManager.Instance.SetSaveItemDatas(ItemData.Instance.GetItemDatas().ToArray());
            Debug.Log("Userdata Save");
        }
    }

    /// <summary>
    /// 씬 전환 후 처리부
    /// </summary>
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        // 씬 이름에 따라 어떻게 초기화를 시킬지 결정
        switch (arg0.name)
        {

            case "Title":
                
                IsUIScene = true;
                SoundManager.Instance.SetBgmSound(true);
                break;

            case "Main":
         
                IsUIScene = true;
                if (currMainView == VIEW_KIND.NONE || currMainView == VIEW_KIND.SKILLTREE)
                {
                    currMainView = VIEW_KIND.HOME;
                }

                MainView.Instance.Init(currMainView);
                SoundManager.Instance.SetBgmSound(true);
                
                break;

            case "Battle":
                IsUIScene = false;
                currMainView = VIEW_KIND.BATTLE;
                BattleSceneManager.Instance.Init();
                SoundManager.Instance.SetBgmSound(true);

                break;
            default:
                break;
        }

        if (arg0.name == loadsceneName)
        {
            //StopAllCoroutines();
            //StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }



    /// <summary>
    /// 임시 연출
    /// </summary>
    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;
        while (timer <= 1f)
        {
            yield return null;
            timer += Time.deltaTime * 3f;
            canvasGroup.alpha = isFadeIn ? Mathf.Lerp(0, 1, timer) : Mathf.Lerp(1, 0, timer);
        }

        if (!isFadeIn)
            this.gameObject.SetActive(false);
    }
}

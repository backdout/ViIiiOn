using System;

using UnityEngine;


using UnityEngine.InputSystem;

using TMPro;

public class BattleUIManager : MonoBehaviour
{
    
    public OnScreenStickCustom JoyStick;
    public GameObject JoyStickGroup;
    public GameClearOrOver GameClearOrOverUI;

    public PlayerInput playerInput;

    [SerializeField]
    private TextMeshProUGUI timeText;
  

    [Space]
    public GameObject DamageTextGroup;  
    public Canvas Canvas;
    public GameObject DamageGroup;


    private void Awake()
    {
        
    }
    private void OnEnable()
    {
      
    }
    private void OnDisable()
    {
        BattleSceneManager.Instance.JoyStickTouchPress.started -= obj => this.StartJoyStick();
        BattleSceneManager.Instance.JoyStickTouchPress.canceled -= obj => this.EndJoyStick();
       
        Debug.Log("clear");
    }


    public void Init()
    {
        gameObject.SetActive(true);
        Canvas = this.gameObject.GetComponent<Canvas>();
        SetJoyStick(OptionDataManager.Instance.Joystick);
        GameClearOrOverUI.gameObject.SetActive(false);    
    }


    public void ReInit()
    {
        GameClearOrOverUI.gameObject.SetActive(false);
        gameObject.SetActive(true);
        timeText.text = new TimeSpan(0, 0, Mathf.RoundToInt((float)0)).ToString(@"mm\:ss");      
    }

    public void SetJoyStick(bool isOn)
    {
        if(JoyStickGroup == null) { Debug.Log("selll"); }
        JoyStickGroup.GetComponent<CanvasGroup>().alpha = isOn ? 1 : 0; 
    }

    public void GameOver()
    {
        GameClearOrOverUI.SetData(false);
        GameClearOrOverUI.gameObject.SetActive(true);
        
    }

    public void GameClear()
    {
        GameClearOrOverUI.SetData(true);
        GameClearOrOverUI.gameObject.SetActive(true);
    }


  

    public void UpdateTime(double sec)
    {
        timeText.text = new TimeSpan(0, 0, Mathf.RoundToInt((float)sec)).ToString(@"mm\:ss"); 
    }



    public void StartJoyStick()
    {
   
        if (BattleSceneManager.Instance.battleManager.IsPush|| JoyStickGroup.gameObject.activeSelf == false|| Time.timeScale == 0)
            return;

        Vector2 first;
   
        first = ConvertPosition(BattleSceneManager.Instance.JoyStickTouch.ReadValue<Vector2>());
        JoyStickGroup.transform.position = first;

    }

    private Vector2 ConvertPosition(Vector2 pos)
    {
        pos.x = pos.x * 720f / Screen.width;
        pos.y = pos.y * 720f / Screen.width;

        return pos;
    }

    public void EndJoyStick()
    {
      
        if (BattleSceneManager.Instance.battleManager.IsPush || Time.timeScale == 0f || JoyStickGroup.gameObject.activeSelf == false)
            return;

        JoyStickGroup.transform.position  = Vector2.right * 360 + Vector2.up * 300;

    }

    public void OnClickPause()
    {
        OptionPopup.SHOW(OptionPopup.OptionPopupKind.BattleOption);
        BattleSceneManager.Instance.battleManager.IsPush = true;
    }

    public void OnClickClear()
    {
        // 기록 저장 후 씬이동
        LoadingScenePrefab.Instance.LoadScene("UIScene");
    }

  

    public void ChangePause()
    {
        //if (Time.timeScale == 0)
        // //   BattlePauseUI.OnClickReturn();
        //else
        //    OnClickPause();
    }

 

}

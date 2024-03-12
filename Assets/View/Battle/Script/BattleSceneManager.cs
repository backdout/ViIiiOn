using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleSceneManager : Singleton<BattleSceneManager>
{
    public BattleManager battleManager { get; private set; }
    public SpawnManager spawnManager { get; private set; }

    //public GameTimeManager timeManager;
    public BattleUIManager battleUI { get; private set; }


    //public ChapterWaveManager chapterManager;


    public InputAction JoyStickTouch;
    public InputAction JoyStickTouchPress;
    private void Awake()
    {
        // Init();
       
       
    }


    public void Init()
    {
        GameObject Manager = GameObject.Find("Manager");

        Manager.transform.position = Vector3.zero;

        ObjectPoolManager.Instance.Init();

        battleManager = Manager.GetComponent<BattleManager>();
        battleManager.Init(UserData.Instance.LastEnterStage);

        battleUI = Manager.GetComponent<BattleUIManager>();
        battleUI.Init();


        spawnManager = Manager.GetComponentInChildren<SpawnManager>();
        spawnManager.Init(UserData.Instance.LastEnterStage);
        spawnManager.gameObject.SetParent(battleManager.Player.gameObject);

        Debug.Log("BattleSceneManager Init stage:" + UserData.Instance.LastEnterStage);

        JoyStickTouch = battleUI.playerInput.actions["TouchStart"];
        JoyStickTouchPress = battleUI.playerInput.actions["TouchPress"];
        JoyStickTouchPress.started += obj => battleUI.StartJoyStick();
        JoyStickTouchPress.canceled += obj => battleUI.EndJoyStick();

    }


    public void ReInit(int stageNum = 0)
    {
        battleManager.ReInit(stageNum);
        battleUI.ReInit();
        spawnManager.ReInit(stageNum);

    }
}

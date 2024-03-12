using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.UI;

public class Player : UnitHandler
{
    public  Vector2 inputVec { get; private set; }  
    private Rigidbody2D playerRigid;
    private BattleManager battleManager;
    [SerializeField]
    private HpBar HpBar; 
 
    private void Start()
    {
        Init();
        BattleSceneManager.Instance.battleUI.JoyStick.onJoystickMove.RemoveAllListeners();
        BattleSceneManager.Instance.battleUI.JoyStick.onJoystickMove.AddListener(SetMoveVector);
        BattleSceneManager.Instance.battleUI.JoyStick.onJoystickRelease.RemoveAllListeners();
        BattleSceneManager.Instance.battleUI.JoyStick.onJoystickRelease.AddListener(SetMoveStop);
    }
    bool isInit = false;
    public override void Init()
    {
        base.Init();
        battleManager = BattleSceneManager.Instance.battleManager;

        playerRigid = GetComponent<Rigidbody2D>();
        CharAni = GetComponent<UnitAniManager>();

        CharRect = transform.GetChild(0).GetComponent<Transform>();
        SetState(STATE.IDLE);
        if (Weapon != null)
            Weapon.Init(this);

        IsHero = true;
     
        if (BattleBuff != null)
            BattleBuff.Init(true, this);

     
        transform.localPosition = Vector3.zero;
        SetMaxHp();
       //SetHP(RecordManager.Instance.HasBattleRecord ? RecordManager.Instance.GetLoadBattleData(RecordManager.BattleData.Hp) : maxHp);
        SetHP(maxHp);
        HpBar.SetHPBar(maxHp, maxHp);

        var heroData = HeroData.Instance;
      
        SetBaseStat(heroData.GetAtk(),heroData.baseDef, heroData.baseCritical, heroData.baseCriticalDamage, heroData.GetMoveSpeed());
        isInit = true;
    }
    public override void SetMaxHp()
    {
        int rtHp = HeroData.Instance.GetMaxHp();
        // 버프 추가 
        float addRate = 0;
        if (BattleBuff != null)
            addRate = BattleBuff.GetBuffStat(BuffManager.BUFF_KIND.BUFF_HP);

        maxHp = (int)(rtHp * (1 + (addRate / 100f)));
    }

    protected override void SetHP(int hp)
    {
        if (IsDie == false)
        {
            if (hp <= 0)
            {
                // 부활 버프 체크 
                var stat = BattleBuff.GetBuffStat(BuffManager.BUFF_KIND.BUFF_REVIVAL);
                if (stat > Random.Range(0f, 100f) )
                {
                    var hpRate = BattleBuff.GetBuffStat(BuffManager.BUFF_KIND.BUFF_REVIVAL_HP);
                    hp = hpRate == 0? (int)(maxHp * BuffRevivalHp.FIX_STAT / 100f) : (int)(maxHp * hpRate / 100f);
                    base.SetHP(hp);
                    HpBar.UpdateHP(hp, maxHp);
                    BattleBuff.BuffRemove(BuffManager.BUFF_KIND.BUFF_REVIVAL);
                    return;
                }
            }

            base.SetHP(hp);
            HpBar.UpdateHP(hp, maxHp);
         

            if (hp <= 0)
            {
                //BattleSceneManager.Instance.battleManager.SetRecordSave(true);
           
                HpBar.UpdateHP(0, maxHp);
                SetDeath();

            }
        }
    }




    public override void SetDeath()
    {
        base.SetDeath();

        Invoke(nameof(ShowGameOver), 0.47f);

    }
    private void ShowGameOver()
    {

        BattleSceneManager.Instance.battleUI.GameOver();
      
        // 게임 오버 되면 돈 획득 처리??? 어떻게 할지 고민하기 


    }

    private void FixedUpdate()
    {
        if (!isInit)
            return;
        Vector2 moveVec = inputVec * GetMoveSpeed() * Time.fixedDeltaTime;
        playerRigid.MovePosition(playerRigid.position + moveVec);
        battleManager.SetCloseDropItem();

        SetPlayerMotion();
    }


    //InputSystem 함수
    private void OnMove(InputValue inputValue)
    {

        SetMoveVector(inputValue.Get<Vector2>());

    }
   

    public void SetMoveVector(Vector2 vector2)
    {
        inputVec = vector2;
    }



    public void SetMoveStop()
    {
        inputVec = Vector2.zero;
    }



    private void SetPlayerMotion()
    {
        if (inputVec.x != 0.0f)
            CharRect.rotation = inputVec.x < 0 ? left : right;

        // 공격 체크
        if (BattleSceneManager.Instance.battleManager.HasCloseMob())
        {
            if (UnitState != STATE.ATTACK)
                SetState(STATE.ATTACK);
        }
        else
        {
            if (inputVec == Vector2.zero)
            {
                if (UnitState != STATE.IDLE)
                    SetState(STATE.IDLE);
            }
            else
            {
                if (UnitState != STATE.RUN)
                    SetState(STATE.RUN);
            }

        }
    }


   
}

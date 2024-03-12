using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
//using static PlayerObj;

public class Monster : UnitHandler
{
    // Start is called before the first frame update
    private Rigidbody2D monsterBody;
    private Rigidbody2D targetBody; 

    public StatBase statBase { get; private set; }
    [SerializeField]
    private CanvasGroup canvasGroup;
    public int[] DropRateGem { get; private set; }

    void Start()
    {
       
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.activeSelf)
            return;
        UnitHandler target = collision.GetComponent<UnitHandler>();
        if (target != null && target.IsHero)
            target.DoDamageEvent(this);

    }

    public override void Init()
    {
        base.Init();
        
        IsHero = false;
        targetBody = BattleSceneManager.Instance.battleManager.PlayerBody;
        if (targetBody == null)
            Debug.Log("player targetBody null");
        UnitState = STATE.IDLE;
        monsterBody = GetComponent<Rigidbody2D>();
        CharAni = GetComponent<UnitAniManager>();
        CharRect = GetComponent<RectTransform>();
        statBase = GetComponent<StatBase>();
      

        //기본 베이스 스탯 셋팅 
        var baseStat = statBase.GetBaseStat();
   
        SetBaseStat(baseStat.Atk, baseStat.Def, baseStat.Critical / 100.0f, baseStat.CriticalDamage / 100.0f, baseStat.MoveSpeed);
        DropRateGem = baseStat.DropRateGem;
        SetHP(baseStat.Hp);

        if (BattleBuff != null)
            BattleBuff.Init(false, this);

        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsDie)
            return; 


        Vector2 dir = targetBody.position - monsterBody.position;
        Vector2 moveVec = dir.normalized * moveSpeed * Time.fixedDeltaTime;
        monsterBody.MovePosition(monsterBody.position + moveVec);
        monsterBody.velocity = Vector2.zero;

        if (moveVec.x == 0.0f && moveVec.y == 0.0f && UnitState != STATE.IDLE)
        {
            SetState(STATE.IDLE);
            return;
        }

        if (UnitState == STATE.IDLE)
            SetState(STATE.RUN);


     
    }
    private void SetDropItem()
    {
        GameObject item = InstantiateDropItem();
        var itemHandler = item.GetComponent<DropItem>();
        itemHandler.Show(this);
        BattleSceneManager.Instance.battleManager.AddDropItemList(itemHandler);
    }
    private void LateUpdate()
    {
        CharRect.rotation = targetBody.position.x < monsterBody.position.x ? left : right;
    }

    public override void SetDeath()
    {
        base.SetDeath();
        Invoke(nameof(SetMobOff), 0.47f);

    }
    private void SetMobOff()
    {
        SetDropItem();
        BattleSceneManager.Instance.battleManager.RemoveMonsterList(this);
        gameObject.SetActive(false);
        ObjectPoolManager.Instance.doDestroy(gameObject);

    }

    public void SetMobDestroy()
    {
        gameObject.SetActive(false);
        ObjectPoolManager.Instance.doDestroy(gameObject);
    }

}

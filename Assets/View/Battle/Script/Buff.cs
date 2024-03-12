using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    // 모든 버프가 상속 받는 클래스 

    public int Id { get; private set; }

    public float Stat { get; private set; }

    //같은 아이디인 경우를 위해 버프를 구분하기 위한 변수(AddType/ AddIndex/AddBuffIndex)
    public BUFF_ADD_TYPE AddType { get; private set; }

    /// <summary>
    /// 몇번째 장착 아이템/ 몇번째 획득 스킬인 구분하는 인덱스 
    /// </summary>
    public int AddIndex { get; private set; }

    /// <summary>
    /// 아이템의 경우, 여러개의 버프를 갖고 있어서, 몇번째 아이템의 몇번째 버프인지 구분하는 용도
    /// </summary>
    public int AddBuffIndex { get; private set; }


    // IsLoop/DurationTime/CoolTime 은 DoBegin에서 1회 정의 
    public bool IsLoop { get; protected set; }

    /// <summary>
    /// 버프 지속 시간
    /// </summary>
    public double DurationTime { get; protected set; }
    /// <summary>
    /// 버프 사용 후 재사용 시간
    /// </summary>
    public double CoolTime { get; protected set; }
    /// <summary>
    /// 해당 버프를 갖고 있는 영웅/ 몹의 버프 핸들러 
    /// </summary>
    public BattleBuffHandler Buffhandler { get; private set; }



    /// <summary>
    /// 중첩 횟수 // 중첩 맥스 횟수가 있음으로 체크라기위해 기록 
    /// </summary>
    public int BuffAddCount { get; protected set; }

    protected int lastDamage = 0;

    public enum BUFF_ADD_TYPE : int
    {
        BATTLE_SKILL = 0,
        ITEM = 1,
        SKILL_TREE = 2,// 스킬 트리
        //==============================
        DEBUFF = 3,// 상대방에게 받은 디버프
        MAX = 4, // 카운트 체크용
    }
    public void Init(int Id, float Stat, BattleBuffHandler Buffhandler, BUFF_ADD_TYPE AddType, int AddIndex, int AddBuffIndex)
    {
        this.Id = Id;
        this.Stat = Stat;
        this.Buffhandler = Buffhandler;
        this.AddType = AddType;
        this.AddIndex = AddIndex;
        this.AddBuffIndex = AddBuffIndex;

    }

    /// <summary>
    /// 스킬의 레벨이 업그레이드나, 특정 조건 만족으로 스탯을 증가 시키는 경우, 해당 함수로 스탯 값을 변경
    /// </summary>
    /// <param name="stat"></param>
    public void ChangeStat(float stat)
    {
        this.Stat = stat;
        BuffAddCount++;
    }
    /// <summary>
    /// 추가시, 초기화 되거나 셋팅 되어야 하는 값들이 있는 경우, override하여 재정의 한다.
    /// </summary>
    public virtual void DoBegin()
    {
        IsLoop = true;
        DurationTime = 0.0f;
        CoolTime = 0.0f;
    }
    /// <summary>
    /// 0.1 초단위로 버프 과정이 체크 되며, 호출시, DurationTime/CoolTime이 감소한다. DurationTime이 0 미만이면, DoEnd 호출한다. 
    /// </summary>
    public virtual void DoProcess()
    {
        DurationTime -= 0.1f;
        CoolTime -= 0.1f;
        if (DurationTime <= 0.00f)
        {
            DoEnd();
        }
    }

    /// <summary>
    /// IsLoop에 따라 제거 되거나, override하여 재정의 하여 루프시 반영될 값을 넣는다.
    /// </summary>
    public virtual void DoEnd()
    {
        if (IsLoop == false)
            Delete();
    }

    public virtual void Delete()
    {

        if (AddType == BUFF_ADD_TYPE.DEBUFF)
        {
            Buffhandler.RemoveDebuff(this);
        }
        else
        {
            Buffhandler.RemoveBuff(this);
        }
        //초기화
        Init(0, 0, null, BUFF_ADD_TYPE.MAX, -1, -1);


    }

    /// <summary>
    /// 지속시간이 특정 상황에 따라 갱신 되는 경우가 있는데, override하여 갱신시, 초기화 하거나 , 값을 재설정한다. 
    /// </summary>
    public virtual void SetDurationTime()
    {
        DurationTime = 0;
    }


    public virtual void SetDurationTime(float time)
    {
        DurationTime = time;
    }

    /// <summary>
    /// 버프추가시, 마지막에 받는 데미지에따라, 버프에 영향을 주는 경우가 있다.(독 데미지 디버프 )
    /// </summary>
    /// <param name="_lastDamage"></param>
    public void SetLastDamage(int _lastDamage)
    {
        lastDamage = _lastDamage;
    }

    /// <summary>
    /// 디버프의 경우 루프되지 않음으로, 디버프 추가시 호출
    /// </summary>
    public void SetDeBuff()
    {
        IsLoop = false;
    }
}

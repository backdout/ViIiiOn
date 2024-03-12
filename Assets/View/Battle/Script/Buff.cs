using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    // ��� ������ ��� �޴� Ŭ���� 

    public int Id { get; private set; }

    public float Stat { get; private set; }

    //���� ���̵��� ��츦 ���� ������ �����ϱ� ���� ����(AddType/ AddIndex/AddBuffIndex)
    public BUFF_ADD_TYPE AddType { get; private set; }

    /// <summary>
    /// ���° ���� ������/ ���° ȹ�� ��ų�� �����ϴ� �ε��� 
    /// </summary>
    public int AddIndex { get; private set; }

    /// <summary>
    /// �������� ���, �������� ������ ���� �־, ���° �������� ���° �������� �����ϴ� �뵵
    /// </summary>
    public int AddBuffIndex { get; private set; }


    // IsLoop/DurationTime/CoolTime �� DoBegin���� 1ȸ ���� 
    public bool IsLoop { get; protected set; }

    /// <summary>
    /// ���� ���� �ð�
    /// </summary>
    public double DurationTime { get; protected set; }
    /// <summary>
    /// ���� ��� �� ���� �ð�
    /// </summary>
    public double CoolTime { get; protected set; }
    /// <summary>
    /// �ش� ������ ���� �ִ� ����/ ���� ���� �ڵ鷯 
    /// </summary>
    public BattleBuffHandler Buffhandler { get; private set; }



    /// <summary>
    /// ��ø Ƚ�� // ��ø �ƽ� Ƚ���� �������� üũ������� ��� 
    /// </summary>
    public int BuffAddCount { get; protected set; }

    protected int lastDamage = 0;

    public enum BUFF_ADD_TYPE : int
    {
        BATTLE_SKILL = 0,
        ITEM = 1,
        SKILL_TREE = 2,// ��ų Ʈ��
        //==============================
        DEBUFF = 3,// ���濡�� ���� �����
        MAX = 4, // ī��Ʈ üũ��
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
    /// ��ų�� ������ ���׷��̵峪, Ư�� ���� �������� ������ ���� ��Ű�� ���, �ش� �Լ��� ���� ���� ����
    /// </summary>
    /// <param name="stat"></param>
    public void ChangeStat(float stat)
    {
        this.Stat = stat;
        BuffAddCount++;
    }
    /// <summary>
    /// �߰���, �ʱ�ȭ �ǰų� ���� �Ǿ�� �ϴ� ������ �ִ� ���, override�Ͽ� ������ �Ѵ�.
    /// </summary>
    public virtual void DoBegin()
    {
        IsLoop = true;
        DurationTime = 0.0f;
        CoolTime = 0.0f;
    }
    /// <summary>
    /// 0.1 �ʴ����� ���� ������ üũ �Ǹ�, ȣ���, DurationTime/CoolTime�� �����Ѵ�. DurationTime�� 0 �̸��̸�, DoEnd ȣ���Ѵ�. 
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
    /// IsLoop�� ���� ���� �ǰų�, override�Ͽ� ������ �Ͽ� ������ �ݿ��� ���� �ִ´�.
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
        //�ʱ�ȭ
        Init(0, 0, null, BUFF_ADD_TYPE.MAX, -1, -1);


    }

    /// <summary>
    /// ���ӽð��� Ư�� ��Ȳ�� ���� ���� �Ǵ� ��찡 �ִµ�, override�Ͽ� ���Ž�, �ʱ�ȭ �ϰų� , ���� �缳���Ѵ�. 
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
    /// �����߰���, �������� �޴� ������������, ������ ������ �ִ� ��찡 �ִ�.(�� ������ ����� )
    /// </summary>
    /// <param name="_lastDamage"></param>
    public void SetLastDamage(int _lastDamage)
    {
        lastDamage = _lastDamage;
    }

    /// <summary>
    /// ������� ��� �������� ��������, ����� �߰��� ȣ��
    /// </summary>
    public void SetDeBuff()
    {
        IsLoop = false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class UnitHandler : MonoBehaviour
{
    public enum STATE
    {
        IDLE,
        RUN,
        ATTACK,
        DEATH,
    }

    public enum ATK_TYPE
    {
        Bow,
        Magic,
        Sword,
    }
   

  
    [SerializeField]
    protected WeaponController Weapon;
    protected BattleBuffHandler BattleBuff;
    protected UnitAniManager CharAni;
    protected Transform CharRect;
    protected Quaternion left = Quaternion.identity;
    protected Quaternion right = new Quaternion(0, 1, 0, 0);


    public bool IsDie { get; private set; }

    public int hp { get; protected set; }
    public int maxHp { get; protected set; }

    public int atk { get; protected set; }
    public int def { get; protected set; }
    public float critical { get; protected set; }
    public float criticalDamage { get; protected set; }
    public float moveSpeed { get; protected set; }


    public bool IsHero = false;

    protected Func<bool> CheckAttackDelay;

    protected bool isPlayingKnockBack;

    private static GameObject DamageText;

    private static GameObject DropItem;

    private Texture mainTex;

    private SkinnedMeshRenderer mesh;

    public STATE UnitState { get; protected set; }
    public ATK_TYPE AtkType { get; protected set; }

    public bool IsPlayHit { get; private set; }
    private CanvasGroup canvasGroup;
    public virtual void Init()
    {
        IsDie = false;
        // �ӽ�, ���� ���� Ÿ�� �������� ����
        AtkType = ATK_TYPE.Sword;
        TurnOffHitEffect();

        if (DamageText == null)
            DamageText = Resources.Load<GameObject>("DamageFont");
        if (canvasGroup == null)
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
        IsPlayHit = false;

        if (BattleBuff == null)
            BattleBuff = gameObject.AddComponent<BattleBuffHandler>();

    }

    protected virtual void SetHP(int hp)
    {
        this.hp = hp;

        if (hp <= 0)
        {
            SetDeath();
        }
    }

    protected void SetAtk(int _atk)
    {
        atk = _atk;
    }

    protected void SetBaseStat(int _atk, int _def, float _critical, float _criticalDamage, float _moveSpeed)
    {
        atk = _atk;
        def = _def;
        critical = _critical;
        criticalDamage = _criticalDamage;
        moveSpeed = _moveSpeed;
       
    }


    protected void UpdateHP(int v)
    {
        SetHP(hp + v);
    }

    public void UpdateHP()
    {
        SetHP(hp);
    }

    public virtual int GetAtk()
    {
        return atk;
    }

    public void GetDamage()
    {
        if (IsDie == true)
            return;

        UpdateHP(-1);
    }

  
    protected virtual void SetState(STATE state)
    {
        UnitState = state;
        if (CharAni != null)
            CharAni.PlayAnimation(state, AtkType);
    }

    public virtual void SetDeath()
    {
        if (IsDie == false)
        {

            IsDie = true;

            if (BattleBuff != null)
                BattleBuff.stopRepeat();

            SetState(STATE.DEATH);
            
        }
    }

    protected bool CheckAttack()
    {
        return CheckAttackDelay != null ? CheckAttackDelay.Invoke() : true;
    }

    private IEnumerator TurnOffEffect(GameObject eff)
    {
        yield return new WaitForSeconds(1f);

        ObjectPoolManager.Instance.doDestroy(eff);
    }

    private void TurnOnHitEffect()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponentInChildren<CanvasGroup>();

        canvasGroup.alpha = 0.3f;
        IsPlayHit = true;
    }

    private void TurnOffHitEffect()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            IsPlayHit = false;
        }
    }

    private void PlayHitEffect()
    {
        TurnOnHitEffect();

        Invoke(nameof(TurnOffHitEffect), 0.05f);
    }




    public void DoDamageEvent(UnitHandler attacker)
    {
        if (IsDie)
            return;

        if (IsHero)
        {
            if (OptionDataManager.Instance.Vibration)
                VibrationManager.Instance.SetVibrate(60);
        }

        if (!IsPlayHit)
            PlayHitEffect();

        // HP ����
        int total = GetTotalDamage(attacker, out bool Cri);
        UpdateHP(-total);

        // ����� �߰� 
        // SetInsertDebuff(attackerSkill, total);

        // ������ ���
        SetDamgeText(total, true, Cri);


        // ������ �Ծ�����, Ư�� ���Ϸ� HP �����Ҷ�, Ư�� ���� ���� �ð� ����
        if (this.BattleBuff != null)
            BattleBuff.SetDurationTimeFromDamage();


        if (hp > 0)
            SoundManager.Instance.PlayEffectAudioClip(SoundManager.EffSoundKind.Hit);

     
    }

    private int GetTotalDamage(UnitHandler _attacker, out bool isCri)
    {
        isCri = false;

        //stage_wave - BattleValue ���� stage_wave �߰��� ���� 
        float chapterValue = 1;//BattleSceneManager.Instance.battleManager.stageNum;
        
        
        // ȸ���� üũ
        if (this.BattleBuff != null)
        {
            float missRate = BattleBuff.GetBuffMiss();
            if (UnityEngine.Random.Range(0.00f, 1.00f) <= missRate)
                return 0;
        }

        var attacker = _attacker;
        var attackerBuff = attacker.BattleBuff;
      


        // ���� ������ ����
        // �⺻ ������ = (���ݵ����� - ���)
        // �⺻ ������ = �⺻ ������ * ġ��Ÿ ������  
        // ���� ������ =  �⺻ ������ * �߰� ������ 

        float addAtkRate = 0;
        float addDefRate = 0;
        float addDmgRate = 0;
      

        // ���� ��������
        if (attackerBuff != null)
            addAtkRate = attackerBuff.GetBuffAtk(this);

        // ��� 
        // �޴� ������ ���� �ۼ�Ʈ 
        if (this.BattleBuff != null)
        {
            addDefRate = BattleBuff.GetBuffDef();
        }


        // ���� ������ 
        float atkDmg = attacker.GetAtk() * (1 + addAtkRate) * chapterValue;

        // ��� 
        float defDmg = this.def * (1 + addDefRate) * chapterValue;

        // �⺻ ������ - �ּҰ� 1;
        //attacker_(AttackDamage x SkillDamgeRate) - defender_Defence         
        float pureDmg = Math.Max(1, atkDmg  - defDmg);



        // ũ��Ƽ�� Ȯ�� 
        float criRate = attacker.critical;
        if (attackerBuff != null)
        {
            criRate += attackerBuff.GetBuffCriticalRate();
        }

        // ũ��Ƽ�� ������
        float criDamageRate = 0;
        
        if (UnityEngine.Random.Range(0.00f, 1.00f) < criRate)
        {

            if (attackerBuff != null)
            {
                criDamageRate = attacker.criticalDamage + attackerBuff.GetBuffCriticalDamage();
            }
            isCri = true;
            pureDmg = pureDmg * (1 + criDamageRate);
        }


        // �߰� ������ 
        // LastDamage x (1 + AddDamage / 100)
        if (attackerBuff != null)
            addDmgRate = attackerBuff.GetBuffAddDamege(this);

        int lastDmg = (int)(pureDmg * (1 + addDmgRate));


        return Math.Max(1, lastDmg);
    }


    public virtual void SetMaxHp()
    {

    }


    public void AddBuffRegenHP(float rate)
    {
        if (BattleBuff != null)
        {
            if (hp < maxHp)
            {
                int AddHp = (int)(maxHp * rate);
                if (AddHp > 0)
                {
                    SetHP(Math.Min(maxHp, hp + AddHp));
                    UpdateHP();
                    // �� �ؽ�Ʈ ���
                    SetDamgeText(AddHp, false);
                }
            }

        }
    }

    public void SetDebuffDamageHP(int Dmg)
    {
        if (BattleBuff != null)
        {
            if (hp > 0)
            {
                SetHP(Math.Max(0, hp - Dmg));
                UpdateHP();
                // �������� �ؽ�Ʈ ���
                SetDamgeText(Dmg, true);
            }

        }
    }
    public void SetRotation(Quaternion q)
    {
        transform.rotation = q;
    }



   

    public float GetMoveSpeed()
    {
        float AddRate = 0.0f;
        if (BattleBuff != null)
            AddRate = BattleBuff.GetBuffMoveSpeed();

        return moveSpeed * (1 + AddRate);
    }


    private void SetDamgeText(int dmgNum, bool isDamage, bool isCri = false)
    {
        if (isDamage && IsHero)
            return;

        GameObject DmgText = InstantiateDamageText();
        var DamageFontHandler = DmgText.GetComponent<DamageFontHandler>();
        DamageFontHandler.Show(dmgNum, this.gameObject, isDamage, isCri);

    }
    /// <summary>
    /// ������ �ǰݽ�, ������ ��ȯ�϶�, ��µǴ� �������� 
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="dmgNum"></param>
    private void SetReturnDamageText(GameObject attacker, int dmgNum)
    {
        GameObject DmgText = InstantiateDamageText();
        var DamageFontHandler = DmgText.GetComponent<DamageFontHandler>();
        DamageFontHandler.Show(dmgNum, attacker, true, false);

    }

    private GameObject InstantiateDamageText()
    {
        if (DamageText == null)
            DamageText = Resources.Load<GameObject>("DamageFont");

        GameObject g = ObjectPoolManager.Instance.doInstantiate(DamageText);

        g.SetActive(false);

        return g;
    }
  

    protected GameObject InstantiateDropItem()
    {
        if (DropItem == null)
            DropItem = Resources.Load<GameObject>("DropItem");

        GameObject g = ObjectPoolManager.Instance.doInstantiate(DropItem);

        g.SetActive(false);

        return g;
    }


    public virtual bool IsAttackable()
    {
        if (IsDie)
            return false;

        return true;
    }

    public virtual UnitHandler GetTarget()
    {
        return null;
    }

    public virtual UnitHandler GetNextTransitionTarget(float distance, List<UnitHandler> targets)
    {
        return null;
    }

 

    public UnitAniManager GetCharAni()
    {
        return CharAni; 
    }

}

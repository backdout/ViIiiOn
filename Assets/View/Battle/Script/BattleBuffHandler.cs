using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleBuffHandler : MonoBehaviour
{
    // 전투시, 유닛별 각각 갖고 있는 버프를 관리 하는 곳 

    // 전투시작/ 공격/ 피격 등의조건에 따른 버프 ADD 를 이곳에서 처리 

    // 보유 버프를 바탕으로 예외처리를 이곳에서 처리 

    public bool IsHero { get; private set; }
    public UnitHandler UnitHandler { get; private set; }

    // 디버프를 제외한 모든 버프 리스트(아이템, 액티브, 패시브 스킬에 있는 옵션들 + 추후 스킬트리 추가시, 스킬 트리도 포함)
    public List<Buff> Buffs { get; private set; }

    /// <summary>
    /// 상대의 공격으로 받은 버프
    /// </summary>
    public List<Buff> DeBuffs { get; private set; }



    public void Init(bool isHero, UnitHandler unitHandler)
    {
        // 인입시, 다 초기화 // 
        RegenHpBuffStat = 0;
        IsHero = isHero;
        UnitHandler = unitHandler;
        Buffs = new List<Buff>();
        DeBuffs = new List<Buff>();

        // 영웅일 경우, heroData 에서 아이템 및 스킬 트리 버프정보 셋팅 
        if (isHero)
        {
            //AddItemBuff();
            AddSkillTreeBuff();

        }
        else
        {   // 보스 몹일경우,
            // 액티브 스킬 셋팅 

        }

     
        startRepeat();
    }

    public void startRepeat()
    {
        if (IsInvoking(nameof(BuffRepeat)) == false)
            InvokeRepeating(nameof(BuffRepeat), 0, 0.1f);
    }
    /// <summary>
    /// 전투 종료로 버프 호출 종료시, 모든 버프 제거
    /// </summary>
    public void stopRepeat()
    {
        CancelInvoke(nameof(BuffRepeat));
        AllDeleteBuff();
    }

    bool hasRegenHpBuff;
    float RegenHpBuffStat;
    /// <summary>
    /// 0.1 초단위로 모든 버프의 DoProcess(); 호출
    /// </summary>
    void BuffRepeat()
    {
        if (Buffs == null)
            return;


        for (int i = 0; i < Buffs.Count; i++)
        {
            if (Buffs[i].Id == (int)BuffManager.BUFF_KIND.BUFF_REGEN_TIME && Buffs[i].DurationTime <= 0.1)
            {
                //RegenHpBuffStat += Buffs[i].Stat;
                hasRegenHpBuff = true;
            }

            if (Buffs[i].DurationTime > 0.0f || Buffs[i].CoolTime > 0.0f)
            {
                Buffs[i].DoProcess();
            }
        }

        // 아이템 회복 버프는 합산하여 적용 해야 함으로 따로 처리 
        if (hasRegenHpBuff == true)
        {
            hasRegenHpBuff = false;

            if (RegenHpBuffStat == 0)
                RegenHpBuffStat = BuffRegenHP.FIX_STAT;

            UnitHandler.AddBuffRegenHP(RegenHpBuffStat / 100f);
            
        }



        if (DeBuffs == null)
            return;

        for (int i = 0; i < DeBuffs.Count; i++)
        {
            if (DeBuffs[i].DurationTime > 0.0f || DeBuffs[i].CoolTime > 0.0f)
            {
                DeBuffs[i].DoProcess();
            }
        }
    }


    #region Buff_Add
    /// <summary>
    /// 배틀중 스킬 선택한 버프 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="stat"></param>
    /// <param name="AddIndex"></param>
    /// <param name="Addbuffindex"></param>
    /// <param name="time"></param>
    public void AddBattleSkillBuff(int id, float stat, int AddIndex, int Addbuffindex, float time = -1)
    {
        if (id == 0)
            return;
        // 이미 가지고 있는 버프 일 경우, 스탯만 변경 
        if (ChangeBuffStat(id, stat, Buff.BUFF_ADD_TYPE.BATTLE_SKILL, AddIndex, Addbuffindex, time) == false)
        {  //없으면 신규 추가 
            AddBuff(id, stat, Buff.BUFF_ADD_TYPE.BATTLE_SKILL, AddIndex, Addbuffindex, time);
        }
        // 배틀 스킬은 합산되어야 함으로+=
        if ((int)BuffManager.BUFF_KIND.BUFF_REGEN_HP == id)
        {
            RegenHpBuffStat += stat;
        }

       // 추가되는 버프가 최대 HP 관련이라면, 추가와 동시 즉시 갱신 
        if ((int)BuffManager.BUFF_KIND.BUFF_HP == id)
        {
            UnitHandler.SetMaxHp();
            UnitHandler.UpdateHP();
        }
    }
  
    /// <summary>
    /// 몹으로 부터 공격등으로 디버프를 받게 되는 경우 추가 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="stat"></param>
    /// <param name="time"></param>
    /// <param name="lastDamage"></param>
    public void AddDeBuff(int id, float stat, float time = -1, int lastDamage = 0)
    {
        if (id == 0)
            return;
        //디버프는 ID 만 같으면 같은 버프로 인식하고 스탯만 갱신 
        if (ChangeBuffStat(id, stat, Buff.BUFF_ADD_TYPE.DEBUFF, 0, 0, time) == false)
        { //없으면 신규 추가 
            AddBuff(id, stat, Buff.BUFF_ADD_TYPE.DEBUFF, 0, 0, time);
        }
        // 추가되는 버프가 최대 HP 관련이라면, 추가와 동시 즉시 갱신 
        if ((int)BuffManager.BUFF_KIND.BUFF_HP == id)
        {
            UnitHandler.SetMaxHp();
            UnitHandler.UpdateHP();
        }

    }

    private void AddSkillTreeBuff()
    {
        var user = UserData.Instance;

        var redData = SkillTreeDataScript.GET_DATA_LIST(SkillTreeDataScript.SKILL_TREE_TYPE.RED);
        var blueData = SkillTreeDataScript.GET_DATA_LIST(SkillTreeDataScript.SKILL_TREE_TYPE.BLUE);
        var greenData = SkillTreeDataScript.GET_DATA_LIST(SkillTreeDataScript.SKILL_TREE_TYPE.GREEN);


        for (int i = 3; i >= 0; i--)
        {
            if(user.SkillRedPoint - i >= 0)
                 AddSkillTreeBuff(redData[user.SkillRedPoint - i].BuffId,(float)redData[user.SkillRedPoint - i].Value);

            if (user.SkillBluePoint - (100 + i) >= 0)
                AddSkillTreeBuff(blueData[user.SkillBluePoint - (100 + i)].BuffId, (float)blueData[user.SkillBluePoint - (100 + i)].Value);

            if (user.SkillGreenPoint - (200 + i) >= 0)
                AddSkillTreeBuff(greenData[user.SkillGreenPoint - (200 + i)].BuffId, (float)greenData[user.SkillGreenPoint - (200 + i)].Value);
        }

    }

    private void AddSkillTreeBuff(int id, float stat)
    {

        // 이미 가지고 있는 버프 일 경우, 스탯만 변경 
        if (ChangeBuffStat(id, stat, Buff.BUFF_ADD_TYPE.SKILL_TREE) == false)
        {  //없으면 신규 추가 
            AddBuff(id, stat, Buff.BUFF_ADD_TYPE.BATTLE_SKILL);
        }

        // 스킬 트리 버프는 전투진행시, 인입시 1회 추가 및  BUFF_REGEN_HP 는 1개만 추가 함으로 추가가 아니라 해당 값으로 대체
        if ((int)BuffManager.BUFF_KIND.BUFF_REGEN_HP == id)
        {
            RegenHpBuffStat = stat;
        }

        // 추가되는 버프가 최대 HP 관련이라면, 추가와 동시 즉시 갱신 
        if ((int)BuffManager.BUFF_KIND.BUFF_HP == id)
        {
            UnitHandler.SetMaxHp();
            UnitHandler.UpdateHP();
        }
    }


    /// <summary>
    /// 해당 버프는 아이템 생성시 랜덤하게 갖는 고유 옵션들을 추가
    /// </summary>
    /*private void AddItemBuff()
    {
        var EquipItems = HeroData.Instance.GetEquipDeck();
        if (EquipItems != null)
        {
            for (int i = 0; i < EquipItems.Length; i++)
            {
                if (EquipItems[i] != null)
                {
                    for (int k = 0; k < EquipItems[i].optionId.Length; k++)
                    {
                        int id = EquipItems[i].optionId[k];
                        if (id > 0)
                        {
                            AddBuff(id, EquipItems[i].optionValue[k], Buff.BUFF_ADD_TYPE.ITEM, i, k, 0);

                        }
                    }
                }
                else
                {
                    if (i == (int)HeroData.EquipType.WEAPON)
                    { // 무기 미장착인경우, 

                        var weapon = HeroData.Instance.GetWeaponData();

                        for (int k = 0; k < weapon.optionId.Length; k++)
                        {
                            int id = weapon.optionId[k];
                            if (id > 0)
                            {
                                AddBuff(id, weapon.optionValue[k], Buff.BUFF_ADD_TYPE.ITEM, i, k, 0);
                            }
                        }

                    }
                }

            }
        }


    }

    */


    private void AddBuff(int id, float stat, Buff.BUFF_ADD_TYPE buffType, int AddIndex = 0, int Addbuffindex = 0, float time = 0, int lastDamage = 0)
    {
        // 합산 불가의 경우, 이전에 등록된 케이스 모두 제거 후 등록 
        if (IsPossibleSumStat((BuffManager.BUFF_KIND)id) == false)
        {
            if (buffType != Buff.BUFF_ADD_TYPE.DEBUFF)
            {
                for (int i = Buffs.Count - 1; i >= 0; i--)
                {
                    if (Buffs[i].Id == id)
                        Buffs[i].Delete();
                }
            }

        }

        //BuffManager를 통하여 신규 버프 생성
        var addBuff = BuffManager.GetNewBuff((BuffManager.BUFF_KIND)id);

        if (addBuff == null)
        {
            Debug.Log("None Buff ID : " + id);
            return;
        }
        // 버프 Init 및 각종토기에 셋팅해야 하는 값들 설정 
        addBuff.Init(id, stat, this, buffType, AddIndex, Addbuffindex);

        addBuff.DoBegin();
        if (time > 0)
            addBuff.SetDurationTime(time);

        if (lastDamage > 0)
            addBuff.SetLastDamage(lastDamage);

        // 같은 스킬로 들어온 버프가 아니더라도, 같은 유형이라면, 지속 시간은 뒤늦게 들어온 버프시간에 맞추어서 동작 
        if ((BuffManager.BUFF_KIND)id == BuffManager.BUFF_KIND.BUFF_REGEN_HP)
        {
            for (int i = Buffs.Count - 1; i >= 0; i--)
            {
                if (Buffs[i].Id == id)
                    Buffs[i].SetDurationTime();
            }
            addBuff.SetDurationTime();
        }

        if (buffType == Buff.BUFF_ADD_TYPE.DEBUFF)
        {
            // 디버프는 루프 없음
            addBuff.SetDeBuff();
            DeBuffs.Add(addBuff);
        }
        else
            Buffs.Add(addBuff);
    }
    private bool ChangeBuffStat(int id, float stat, Buff.BUFF_ADD_TYPE buffType, int AddIndex = 0, int Addbuffindex = 0, float time = 0, int lastDamage = 0)
    {
        bool hasBuff = false;
        if (buffType != Buff.BUFF_ADD_TYPE.DEBUFF)
        {
            for (int i = 0; i < Buffs.Count; i++)
            {
                if (Buffs[i].Id == id)
                {
                    if (Buffs[i].AddType == buffType && Buffs[i].AddIndex == AddIndex && Buffs[i].AddBuffIndex == Addbuffindex)
                    {
                        Buffs[i].ChangeStat(stat);
                        if (time > 0)
                            Buffs[i].SetDurationTime(time);
                        hasBuff = true;
                        break;
                    }


                }
            }
        }
        else
        {
            //DeBuff는 ID 만 동일 하다면, 갱신  
            for (int i = 0; i < DeBuffs.Count; i++)
            {
                if (DeBuffs[i].Id == id)
                {
                    DeBuffs[i].ChangeStat(stat);
                    if (time > 0)
                        DeBuffs[i].SetDurationTime(time);
                    if (lastDamage > 0)
                        DeBuffs[i].SetLastDamage(lastDamage);
                    hasBuff = true;
                    break;
                }
            }
        }
        return hasBuff;
    }

    #endregion


    public void RemoveBuff(Buff buff)
    {
        if (Buffs.Contains(buff))
            Buffs.Remove(buff);
    }

    public void RemoveDebuff(Buff buff)
    {
        if (DeBuffs.Contains(buff))
            DeBuffs.Remove(buff);
    }



    #region Get_Buff 버프 밸류 값을 가져 오는 곳
    public float GetBuffAtk(UnitHandler target = null)
    {// 공격력
        float stat = 0;

        for (int i = 0; i < Buffs.Count; i++)
        {
            switch ((BuffManager.BUFF_KIND)Buffs[i].Id)
            {
                case BuffManager.BUFF_KIND.BUFF_ATK:
                    stat += Buffs[i].Stat;
                    break;

            }
        }

        return stat / 100.0f;
    }
   


    public float GetBuffDef()
    {//방어력
        float stat = 0;

        for (int i = 0; i < Buffs.Count; i++)
        {
            switch ((BuffManager.BUFF_KIND)Buffs[i].Id)
            {
                case BuffManager.BUFF_KIND.BUFF_DEF:
                    stat += Buffs[i].Stat;
                    break;

            }
        }

        return stat / 100.0f;
    }

    public float GetBuffAddDamege(UnitHandler target = null)
    {//추가 데미지
        float stat = 0;
     
        for (int i = 0; i < Buffs.Count; i++)
        {
            switch ((BuffManager.BUFF_KIND)Buffs[i].Id)
            {
                case BuffManager.BUFF_KIND.BUFF_ADD_DAMAGE:
                    stat += Buffs[i].Stat;
                    break;

            }
        }

        return stat / 100.0f;
    }




    public float GetBuffCriticalRate()
    {// 치명타 확률
        float stat = 0;

        for (int i = 0; i < Buffs.Count; i++)
        {
           if((BuffManager.BUFF_KIND)Buffs[i].Id == BuffManager.BUFF_KIND.BUFF_CRITICAL)
                stat += Buffs[i].Stat; 
        }

        return stat / 100;

    }
 

    public float GetBuffCriticalDamage()
    {//치명타 데미지
        float stat = 0;

        for (int i = 0; i < Buffs.Count; i++)
        {
            if ((BuffManager.BUFF_KIND)Buffs[i].Id == BuffManager.BUFF_KIND.BUFF_CRITICAL_DAMAGE)
                stat += Buffs[i].Stat;
        }

        return stat / 100.0f;

    }

    public float GetBuffMiss()
    {//치명타 데미지
        float stat = 0;

        for (int i = 0; i < Buffs.Count; i++)
        {
            if ((BuffManager.BUFF_KIND)Buffs[i].Id == BuffManager.BUFF_KIND.BUFF_MISS_PER)
                stat += Buffs[i].Stat;
        }

        return stat / 100.0f;

    }



    /// <summary>
    /// 이속 버프 => Debuff  값도 합산하여 반환
    /// </summary>
    /// <returns></returns>
    public float GetBuffMoveSpeed()
    {// 
        float stat = 0;
        if (Buffs != null)
        {
            for (int i = 0; i < Buffs.Count; i++)
            {
                switch ((BuffManager.BUFF_KIND)Buffs[i].Id)
                {
                    case BuffManager.BUFF_KIND.BUFF_MOVE_SPEED:
                        stat += Buffs[i].Stat;
                        break;

                }
            }
        }

        if (DeBuffs != null)
        {
            for (int i = 0; i < DeBuffs.Count; i++)
            {
                switch ((BuffManager.BUFF_KIND)DeBuffs[i].Id)
                {
                    case BuffManager.BUFF_KIND.BUFF_MOVE_SPEED:
                        stat -= DeBuffs[i].Stat;
                        break;

                }
            }
        }
        return stat / 100.0f;
    }

   
    /// <summary>
    /// 해당 함수는 100으로 나눈 값이 아닌 원본 값을 보냄
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    public float GetBuffStat(BuffManager.BUFF_KIND kind)
    {
        float stat = 0;

        if (Buffs != null)
        {
            for (int i = 0; i < Buffs.Count; i++)
            {
                if (Buffs[i].Id == (int)kind)
                {
                    stat += Buffs[i].Stat;

                    if (IsPossibleSumStat(kind) == false)
                        return stat;
                }
            }
        }


        return stat;
    }
    /// <summary>
    /// 현재 디버프 내역, 이속/ 독 데미지 추후 추가 
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    public float GetDebuffStat(BuffManager.BUFF_KIND kind)
    {
        float stat = 0;

        if (DeBuffs != null)
        {
            for (int i = 0; i < DeBuffs.Count; i++)
            {
                if (DeBuffs[i].Id == (int)kind)
                {
                    stat += DeBuffs[i].Stat;

                    if (IsPossibleSumStat(kind) == false)
                        return stat;
                }
            }
        }

        return stat;
    }
    /// <summary>
    /// 무적 여부 판별
    /// </summary>
    /// <returns></returns>
    public bool HasSuperDef()
    {

        if (Buffs != null)
        {

            for (int i = 0; i < Buffs.Count; i++)
            {
                switch ((BuffManager.BUFF_KIND)Buffs[i].Id)
                {
                    case BuffManager.BUFF_KIND.BUFF_FIRST_SUPER_DEF:
                        if (Buffs[i].DurationTime > 0.0f)
                            return true;
                        break;
                    case BuffManager.BUFF_KIND.BUFF_SECOND_SUPER_DEF:
                        if (Buffs[i].DurationTime > 0.0f)
                            return true;
                        break;
                    case BuffManager.BUFF_KIND.BUFF_WHEN_HIT_GET_SUPER_DEF:
                        if (Buffs[i].DurationTime > 0.0f)
                            return true;
                        break;

                }
            }

        }


        return false;


    }

    #endregion;

    #region Set_Time /Duration 시간 갱신 
    /// <summary>
    /// 데미지 입었을때, 특정 이하로 HP 감소할때,  지속 시간 갱신 /
    /// </summary>
    public void SetDurationTimeFromDamage()
    {
        if (Buffs != null)
        {
            for (int i = 0; i < Buffs.Count; i++)
            {

                switch ((BuffManager.BUFF_KIND)Buffs[i].Id)
                {
                  
                    case BuffManager.BUFF_KIND.BUFF_WHEN_HIT_GET_SUPER_DEF:
                        Buffs[i].SetDurationTime();
                        break;

                    case BuffManager.BUFF_KIND.BUFF_FIRST_SUPER_DEF:
                        //체력이 50%미만일 때UnitHandler
                        if (IsHero)
                        {
                            if ((float)UnitHandler.hp / (float)UnitHandler.maxHp <= 0.50f)
                                Buffs[i].SetDurationTime();
                        }
                        break;
                    case BuffManager.BUFF_KIND.BUFF_SECOND_SUPER_DEF:
                        //체력이 20%미만일 때
                        if (IsHero)
                        {
                            if ((float)UnitHandler.hp / (float)UnitHandler.maxHp <= 0.20f)
                                Buffs[i].SetDurationTime();
                        }
                        break;

                }


            }
        }

    }


    #endregion;


    /// <summary>
    /// 합산 여부 판단(무적과 같은 옵션들은 합산 불가)
    /// 해당 옵션들의 경우, 전투 중 동일 옵션 획득시 해당 옵션으로 갱신됨.
    /// </summary>
    public bool IsPossibleSumStat(BuffManager.BUFF_KIND kind)
    {
        bool isPossible = true;
        switch (kind)
        {
            case BuffManager.BUFF_KIND.BUFF_FIRST_SUPER_DEF:
            case BuffManager.BUFF_KIND.BUFF_SECOND_SUPER_DEF:
            case BuffManager.BUFF_KIND.BUFF_WHEN_HIT_GET_SUPER_DEF:
           

                isPossible = false;
                break;
        }

        return isPossible;

    }


    public void BuffRemove(BuffManager.BUFF_KIND kind)
    {
        if (Buffs != null)
        {
            for (int i = Buffs.Count - 1; i >= 0; i--)
            {
                if ((BuffManager.BUFF_KIND)Buffs[i].Id == kind)
                    Buffs[i].Delete();
            }
        }
    }

    public void AllDeleteBuff()
    {
        if (Buffs != null)
        {
            for (int i = Buffs.Count - 1; i >= 0; i--)
            {
                Buffs[i].Delete();
            }
        }

        if (DeBuffs != null)
        {
            for (int i = DeBuffs.Count - 1; i >= 0; i--)
            {
                DeBuffs[i].Delete();
            }
        }


    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleBuffHandler : MonoBehaviour
{
    // ������, ���ֺ� ���� ���� �ִ� ������ ���� �ϴ� �� 

    // ��������/ ����/ �ǰ� �������ǿ� ���� ���� ADD �� �̰����� ó�� 

    // ���� ������ �������� ����ó���� �̰����� ó�� 

    public bool IsHero { get; private set; }
    public UnitHandler UnitHandler { get; private set; }

    // ������� ������ ��� ���� ����Ʈ(������, ��Ƽ��, �нú� ��ų�� �ִ� �ɼǵ� + ���� ��ųƮ�� �߰���, ��ų Ʈ���� ����)
    public List<Buff> Buffs { get; private set; }

    /// <summary>
    /// ����� �������� ���� ����
    /// </summary>
    public List<Buff> DeBuffs { get; private set; }



    public void Init(bool isHero, UnitHandler unitHandler)
    {
        // ���Խ�, �� �ʱ�ȭ // 
        RegenHpBuffStat = 0;
        IsHero = isHero;
        UnitHandler = unitHandler;
        Buffs = new List<Buff>();
        DeBuffs = new List<Buff>();

        // ������ ���, heroData ���� ������ �� ��ų Ʈ�� �������� ���� 
        if (isHero)
        {
            //AddItemBuff();
            AddSkillTreeBuff();

        }
        else
        {   // ���� ���ϰ��,
            // ��Ƽ�� ��ų ���� 

        }

     
        startRepeat();
    }

    public void startRepeat()
    {
        if (IsInvoking(nameof(BuffRepeat)) == false)
            InvokeRepeating(nameof(BuffRepeat), 0, 0.1f);
    }
    /// <summary>
    /// ���� ����� ���� ȣ�� �����, ��� ���� ����
    /// </summary>
    public void stopRepeat()
    {
        CancelInvoke(nameof(BuffRepeat));
        AllDeleteBuff();
    }

    bool hasRegenHpBuff;
    float RegenHpBuffStat;
    /// <summary>
    /// 0.1 �ʴ����� ��� ������ DoProcess(); ȣ��
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

        // ������ ȸ�� ������ �ջ��Ͽ� ���� �ؾ� ������ ���� ó�� 
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
    /// ��Ʋ�� ��ų ������ ���� 
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
        // �̹� ������ �ִ� ���� �� ���, ���ȸ� ���� 
        if (ChangeBuffStat(id, stat, Buff.BUFF_ADD_TYPE.BATTLE_SKILL, AddIndex, Addbuffindex, time) == false)
        {  //������ �ű� �߰� 
            AddBuff(id, stat, Buff.BUFF_ADD_TYPE.BATTLE_SKILL, AddIndex, Addbuffindex, time);
        }
        // ��Ʋ ��ų�� �ջ�Ǿ�� ������+=
        if ((int)BuffManager.BUFF_KIND.BUFF_REGEN_HP == id)
        {
            RegenHpBuffStat += stat;
        }

       // �߰��Ǵ� ������ �ִ� HP �����̶��, �߰��� ���� ��� ���� 
        if ((int)BuffManager.BUFF_KIND.BUFF_HP == id)
        {
            UnitHandler.SetMaxHp();
            UnitHandler.UpdateHP();
        }
    }
  
    /// <summary>
    /// ������ ���� ���ݵ����� ������� �ް� �Ǵ� ��� �߰� 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="stat"></param>
    /// <param name="time"></param>
    /// <param name="lastDamage"></param>
    public void AddDeBuff(int id, float stat, float time = -1, int lastDamage = 0)
    {
        if (id == 0)
            return;
        //������� ID �� ������ ���� ������ �ν��ϰ� ���ȸ� ���� 
        if (ChangeBuffStat(id, stat, Buff.BUFF_ADD_TYPE.DEBUFF, 0, 0, time) == false)
        { //������ �ű� �߰� 
            AddBuff(id, stat, Buff.BUFF_ADD_TYPE.DEBUFF, 0, 0, time);
        }
        // �߰��Ǵ� ������ �ִ� HP �����̶��, �߰��� ���� ��� ���� 
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

        // �̹� ������ �ִ� ���� �� ���, ���ȸ� ���� 
        if (ChangeBuffStat(id, stat, Buff.BUFF_ADD_TYPE.SKILL_TREE) == false)
        {  //������ �ű� �߰� 
            AddBuff(id, stat, Buff.BUFF_ADD_TYPE.BATTLE_SKILL);
        }

        // ��ų Ʈ�� ������ ���������, ���Խ� 1ȸ �߰� ��  BUFF_REGEN_HP �� 1���� �߰� ������ �߰��� �ƴ϶� �ش� ������ ��ü
        if ((int)BuffManager.BUFF_KIND.BUFF_REGEN_HP == id)
        {
            RegenHpBuffStat = stat;
        }

        // �߰��Ǵ� ������ �ִ� HP �����̶��, �߰��� ���� ��� ���� 
        if ((int)BuffManager.BUFF_KIND.BUFF_HP == id)
        {
            UnitHandler.SetMaxHp();
            UnitHandler.UpdateHP();
        }
    }


    /// <summary>
    /// �ش� ������ ������ ������ �����ϰ� ���� ���� �ɼǵ��� �߰�
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
                    { // ���� �������ΰ��, 

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
        // �ջ� �Ұ��� ���, ������ ��ϵ� ���̽� ��� ���� �� ��� 
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

        //BuffManager�� ���Ͽ� �ű� ���� ����
        var addBuff = BuffManager.GetNewBuff((BuffManager.BUFF_KIND)id);

        if (addBuff == null)
        {
            Debug.Log("None Buff ID : " + id);
            return;
        }
        // ���� Init �� ������⿡ �����ؾ� �ϴ� ���� ���� 
        addBuff.Init(id, stat, this, buffType, AddIndex, Addbuffindex);

        addBuff.DoBegin();
        if (time > 0)
            addBuff.SetDurationTime(time);

        if (lastDamage > 0)
            addBuff.SetLastDamage(lastDamage);

        // ���� ��ų�� ���� ������ �ƴϴ���, ���� �����̶��, ���� �ð��� �ڴʰ� ���� �����ð��� ���߾ ���� 
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
            // ������� ���� ����
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
            //DeBuff�� ID �� ���� �ϴٸ�, ����  
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



    #region Get_Buff ���� ��� ���� ���� ���� ��
    public float GetBuffAtk(UnitHandler target = null)
    {// ���ݷ�
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
    {//����
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
    {//�߰� ������
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
    {// ġ��Ÿ Ȯ��
        float stat = 0;

        for (int i = 0; i < Buffs.Count; i++)
        {
           if((BuffManager.BUFF_KIND)Buffs[i].Id == BuffManager.BUFF_KIND.BUFF_CRITICAL)
                stat += Buffs[i].Stat; 
        }

        return stat / 100;

    }
 

    public float GetBuffCriticalDamage()
    {//ġ��Ÿ ������
        float stat = 0;

        for (int i = 0; i < Buffs.Count; i++)
        {
            if ((BuffManager.BUFF_KIND)Buffs[i].Id == BuffManager.BUFF_KIND.BUFF_CRITICAL_DAMAGE)
                stat += Buffs[i].Stat;
        }

        return stat / 100.0f;

    }

    public float GetBuffMiss()
    {//ġ��Ÿ ������
        float stat = 0;

        for (int i = 0; i < Buffs.Count; i++)
        {
            if ((BuffManager.BUFF_KIND)Buffs[i].Id == BuffManager.BUFF_KIND.BUFF_MISS_PER)
                stat += Buffs[i].Stat;
        }

        return stat / 100.0f;

    }



    /// <summary>
    /// �̼� ���� => Debuff  ���� �ջ��Ͽ� ��ȯ
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
    /// �ش� �Լ��� 100���� ���� ���� �ƴ� ���� ���� ����
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
    /// ���� ����� ����, �̼�/ �� ������ ���� �߰� 
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
    /// ���� ���� �Ǻ�
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

    #region Set_Time /Duration �ð� ���� 
    /// <summary>
    /// ������ �Ծ�����, Ư�� ���Ϸ� HP �����Ҷ�,  ���� �ð� ���� /
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
                        //ü���� 50%�̸��� ��UnitHandler
                        if (IsHero)
                        {
                            if ((float)UnitHandler.hp / (float)UnitHandler.maxHp <= 0.50f)
                                Buffs[i].SetDurationTime();
                        }
                        break;
                    case BuffManager.BUFF_KIND.BUFF_SECOND_SUPER_DEF:
                        //ü���� 20%�̸��� ��
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
    /// �ջ� ���� �Ǵ�(������ ���� �ɼǵ��� �ջ� �Ұ�)
    /// �ش� �ɼǵ��� ���, ���� �� ���� �ɼ� ȹ��� �ش� �ɼ����� ���ŵ�.
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

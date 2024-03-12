using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager
{
    //���� ���� ���� �� �ܼ� ���� ������ �Ŵ��� 
    // ��� ������ ���� �Ŵ����� ���� ����

    /*
        ���� �߰� �� ���� 
        1. �ű� ������ �ְų�, ���ŵ� ������ �ִٸ� �ϴ��� BUFF_KIND �� �߰� �� ����
        2. Buff �� ��� �޴� ���� ��ũ��Ʈ ���� �Ǵ� ���� 
        3. BuffManager.GetNewBuff() �Լ���  �ش� ���̽� �߰� �� ���� 
        =======================================
        ��� ������ ���� ������, GetNewBuff �Լ��� ȣ���Ͽ� ����
     */



    //BUFF_KIND�� GODS-option ���������� ������ �����ϸ�,  int ���� �ɼ� ���̵� ���� �ִ´�. 
    public enum BUFF_KIND : int
    {
        BUFF_ATK = 1,                           //���� +{0}%			
        BUFF_CRITICAL = 2,                       //ġ��Ÿ Ȯ�� +{0}% BUFCRITICAL
        BUFF_CRITICAL_DAMAGE = 3,              //ġ��Ÿ ������ +{0}%
        BUFF_ADD_DAMAGE = 4,                       //�߰� ���ط� +{0}%


        BUFF_DEF = 11,                          //��� +{0} %
        BUFF_MOVE_SPEED = 12,                   //�̵� �ӵ� +{0}%
        BUFF_MISS_PER = 13,                     // ���� ȸ���� +{} % => MISS  ó�� 
        BUFF_HP = 14,                           //ü�� +{0}%

       
        BUFF_REGEN_TIME =21,                    // {0}�ʴ� ü�� ȸ�� 
        BUFF_REGEN_HP = 22,                     // ü�� ȸ���� ��ü HP�� {0}% 
        BUFF_REVIVAL = 23,                      // {0}%�� ��Ȱ
        BUFF_REVIVAL_HP = 24,                   //��Ȱ��, HP {0}%�� ��Ȱ


        // ���Ŀ� �߰�
        BUFF_FIRST_SUPER_DEF = 221,             //ü���� 50%�̸��� �� {0}�ʰ� ����
        BUFF_SECOND_SUPER_DEF = 222,            //ü���� 20%�̸��� �� {0}�ʰ� ����
        BUFF_WHEN_HIT_GET_SUPER_DEF = 223,      //������ ������ {0}%Ȯ����  2�ʰ� ����.

    }

    public static Buff GetNewBuff(BUFF_KIND buffKind)
    {

        switch (buffKind)
        {
            case BUFF_KIND.BUFF_ATK:
                return BuffAtk.pool.Get();

            case BUFF_KIND.BUFF_CRITICAL:
                return BuffCritical.pool.Get();

            case BUFF_KIND.BUFF_CRITICAL_DAMAGE:
                return BuffCriticalDamage.pool.Get();

            case BUFF_KIND.BUFF_ADD_DAMAGE:
                return BuffDamage.pool.Get();



            case BUFF_KIND.BUFF_DEF:
                return BuffDef.pool.Get();

            case BUFF_KIND.BUFF_MOVE_SPEED:
                return BuffMoveSpeed.pool.Get();

            case BUFF_KIND.BUFF_MISS_PER:
                return BuffMissPer.pool.Get();
            case BUFF_KIND.BUFF_HP:
                return BuffHp.pool.Get();



            case BUFF_KIND.BUFF_REGEN_TIME:
                return BuffRegenTime.pool.Get();
            case BUFF_KIND.BUFF_REGEN_HP:
                return BuffRegenHP.pool.Get();

            case BUFF_KIND.BUFF_REVIVAL:
                return BuffRevival.pool.Get();

            case BUFF_KIND.BUFF_REVIVAL_HP:
                return BuffRevivalHp.pool.Get();

                //case BUFF_KIND.BUFF_FIRST_SUPER_DEF:
                //    return BuffFirstSuperDef.pool.Get();

                //case BUFF_KIND.BUFF_SECOND_SUPER_DEF:
                //    return BuffSecondSuperDef.pool.Get();

                //case BUFF_KIND.BUFF_WHEN_HIT_GET_SUPER_DEF:
                //    return BuffWhenHitGetSuperDef.pool.Get();



        }

        return null;

    }


}

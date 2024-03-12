using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager
{
    //버프 종류 관리 및 단순 버프 생성용 매니저 
    // 모든 버프는 버프 매니저를 통해 생성

    /*
        버프 추가 및 제거 
        1. 신규 버프가 있거나, 제거된 버프가 있다면 하단의 BUFF_KIND 에 추가 및 삭제
        2. Buff 를 상속 받는 버프 스크립트 생성 또는 삭제 
        3. BuffManager.GetNewBuff() 함수에  해당 케이스 추가 및 삭제 
        =======================================
        모든 버프는 버프 생성시, GetNewBuff 함수를 호출하여 생성
     */



    //BUFF_KIND는 GODS-option 스프레드의 내용을 참조하며,  int 값은 옵션 아이디 값을 넣는다. 
    public enum BUFF_KIND : int
    {
        BUFF_ATK = 1,                           //공격 +{0}%			
        BUFF_CRITICAL = 2,                       //치명타 확률 +{0}% BUFCRITICAL
        BUFF_CRITICAL_DAMAGE = 3,              //치명타 데미지 +{0}%
        BUFF_ADD_DAMAGE = 4,                       //추가 피해량 +{0}%


        BUFF_DEF = 11,                          //방어 +{0} %
        BUFF_MOVE_SPEED = 12,                   //이동 속도 +{0}%
        BUFF_MISS_PER = 13,                     // 공격 회피율 +{} % => MISS  처리 
        BUFF_HP = 14,                           //체력 +{0}%

       
        BUFF_REGEN_TIME =21,                    // {0}초당 체력 회복 
        BUFF_REGEN_HP = 22,                     // 체력 회복량 전체 HP의 {0}% 
        BUFF_REVIVAL = 23,                      // {0}%로 부활
        BUFF_REVIVAL_HP = 24,                   //부활시, HP {0}%로 부활


        // 추후에 추가
        BUFF_FIRST_SUPER_DEF = 221,             //체력이 50%미만일 때 {0}초간 무적
        BUFF_SECOND_SUPER_DEF = 222,            //체력이 20%미만일 때 {0}초간 무적
        BUFF_WHEN_HIT_GET_SUPER_DEF = 223,      //공격을 받으면 {0}%확률로  2초간 무적.

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

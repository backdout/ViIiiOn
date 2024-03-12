using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HeroData : Singleton<HeroData>
{

    public void Init()
    {

    }
 
    // 기본 영웅 능력치 및 기본 무기 값 
    private const int baseHp = 20;
    private const int baseAtk = 2;
    public readonly int baseDef = 1;
    public readonly float baseCritical = 0.05f;
    public readonly float baseCriticalDamage = 0.2f;


    private const int baseMoveSpeed = 3;

    public int GetMaxHp()
    {
        int addHp = 0;
      
        return (int)(baseHp + addHp);

    }

    public int GetAtk()
    {
        int addAtk = 0;
       
        return (int)(baseAtk + addAtk);
    }

    public int GetMoveSpeed()
    {
     
        int addMoveSpeed = 0;
        return (int)(baseMoveSpeed + addMoveSpeed); 

    }





}
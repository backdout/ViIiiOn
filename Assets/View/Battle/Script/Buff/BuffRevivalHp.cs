using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BuffRevivalHp : Buff
{
    public static ObjectPool<BuffRevivalHp> pool = new ObjectPool<BuffRevivalHp>(() => new BuffRevivalHp());
    public const float FIX_STAT = 10f;


    public override void Delete()
    {
        base.Delete();
        pool.Release(this);
    }

    public override void DoBegin()
    {
        base.DoBegin();

    }




}

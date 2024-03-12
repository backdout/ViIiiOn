using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BuffCriticalDamage : Buff
{
    public static ObjectPool<BuffCriticalDamage> pool = new ObjectPool<BuffCriticalDamage>(() => new BuffCriticalDamage());

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


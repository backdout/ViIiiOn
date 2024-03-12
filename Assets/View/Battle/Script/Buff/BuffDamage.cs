using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BuffDamage : Buff
{
    public static ObjectPool<BuffDamage> pool = new ObjectPool<BuffDamage>(() => new BuffDamage());

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


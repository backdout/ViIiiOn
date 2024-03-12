using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BuffCritical : Buff
{
    public static ObjectPool<BuffCritical> pool = new ObjectPool<BuffCritical>(() => new BuffCritical());

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


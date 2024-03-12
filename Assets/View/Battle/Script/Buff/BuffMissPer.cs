using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BuffMissPer : Buff
{
    public static ObjectPool<BuffMissPer> pool = new ObjectPool<BuffMissPer>(() => new BuffMissPer());

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

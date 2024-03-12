using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BuffRevival : Buff
{
    public static ObjectPool<BuffRevival> pool = new ObjectPool<BuffRevival>(() => new BuffRevival());

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

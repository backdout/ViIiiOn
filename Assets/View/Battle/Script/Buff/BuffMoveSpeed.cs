using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BuffMoveSpeed : Buff
{
    public static ObjectPool<BuffMoveSpeed> pool = new ObjectPool<BuffMoveSpeed>(() => new BuffMoveSpeed());

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BuffHp : Buff
{
    public static ObjectPool<BuffHp> pool = new ObjectPool<BuffHp>(() => new BuffHp());

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

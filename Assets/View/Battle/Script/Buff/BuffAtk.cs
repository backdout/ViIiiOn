using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BuffAtk : Buff
{
    public static ObjectPool<BuffAtk> pool = new ObjectPool<BuffAtk>(() => new BuffAtk());

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

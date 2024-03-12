using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BuffDef : Buff
{
    public static ObjectPool<BuffDef> pool = new ObjectPool<BuffDef>(() => new BuffDef());

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


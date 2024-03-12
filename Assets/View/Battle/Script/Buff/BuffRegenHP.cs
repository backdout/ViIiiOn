using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class BuffRegenHP : Buff
{
    public static ObjectPool<BuffRegenHP> pool = new ObjectPool<BuffRegenHP>(() => new BuffRegenHP());

    public const float FIX_STAT = 0.05f;
    

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

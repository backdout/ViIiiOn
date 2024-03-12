using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BuffRegenTime : Buff
{
    public static ObjectPool<BuffRegenTime> pool = new ObjectPool<BuffRegenTime>(() => new BuffRegenTime());

    public override void Delete()
    {
        base.Delete();
        pool.Release(this);
    }

    public override void DoBegin()
    {
        base.DoBegin();

    }

    public override void SetDurationTime()
    {
        DurationTime = Stat;
    }

    public override void DoEnd()
    {
        DurationTime = Stat;
        //  Buffhandler.UnitHandler.AddBuffRegenHP(Stat / 100f);
    }


}

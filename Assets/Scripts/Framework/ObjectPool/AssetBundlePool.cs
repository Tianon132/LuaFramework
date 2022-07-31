using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundlePool : Poolbase
{
    protected override void Release()
    {
        base.Release();
        foreach (var item in m_Objects)
        {
            if (System.DateTime.Now.Ticks - item.lastUseTime.Ticks >= m_ReleaseTime * 10000000)
            {
                m_Objects.Remove(item);
                Manager.Resource.UnLoadBundle(item.Object);

                Release();
                return;
            }
        }
    }

    public override Object Spwan(string name)
    {
        return base.Spwan(name);
    }

    public override void UnSpwan(string name, Object obj)
    {
        base.UnSpwan(name, obj);
    }
}

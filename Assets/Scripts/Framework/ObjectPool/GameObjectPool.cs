using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : Poolbase
{
    /// <summary>
    /// 释放对象
    /// </summary>
    protected override void Release()
    {
        base.Release();
        foreach (var item in m_Objects)
        {
            if (System.DateTime.Now.Ticks - item.lastUseTime.Ticks >= m_ReleaseTime * 10000000 )
            {
                Manager.Resource.MinusBundleCount(item.name);    //减少该bundle所被使用的
                m_Objects.Remove(item);
                Destroy(item.Object);

                Release();//这里重新调用，因为foreach是不允许在遍历的同时对集合进行改动
                return;
            }
        }
    }

    /// <summary>
    /// 拿出
    /// </summary>
    public override Object Spwan(string name)
    {
        Object obj = base.Spwan(name); 
        if (obj == null)        //添加判断！！！！！！！！！！
            return null;

        GameObject go = obj as GameObject;
        go.SetActive(true);
        return obj;     //返回obj！！！！！！！！！！！
    }

    /// <summary>
    /// 收回
    /// </summary>
    public override void UnSpwan(string name, Object obj)
    {
        GameObject go = obj as GameObject;
        go.SetActive(false);
        go.transform.SetParent(this.transform, false);  //不会随着父节点改变而改变！！！！！！！！！！！！！

        base.UnSpwan(name, obj);    //最后调用！！！！！！！！！！！！！！
    }
}

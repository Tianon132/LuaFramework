using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poolbase : MonoBehaviour
{
    protected List<PoolObject> m_Objects;   //列表形式

    /*************************   释放部分    **************************/

    //释放时间间隔
    protected float m_ReleaseTime = 0f;
    //最近一次施放时间
    protected long m_LastReleaseTime;

    private void Start()
    {
        m_LastReleaseTime = System.DateTime.Now.Ticks;
    }

    public void Init(float releaseTime)
    {
        m_ReleaseTime = releaseTime;
        m_Objects = new List<PoolObject>();
    }

    private void Update()
    {
        if(System.DateTime.Now.Ticks - m_LastReleaseTime >= m_ReleaseTime * 10000000)
        {
            //超出预定时间，开始释放
            m_LastReleaseTime = System.DateTime.Now.Ticks;
            Release();
        }
    }

    /// <summary>
    /// 释放
    /// </summary>
    protected virtual void Release()
    {

    }


    /*************************   操作部分    **************************/

    //拿出
    public virtual UnityEngine.Object Spwan(string name)
    {
        foreach (PoolObject plObj in m_Objects)
        {
            if(plObj.name == name)
            {
                m_Objects.Remove(plObj);
                return plObj.Object;
            }
        }
        return null;

    }

    //回收
    public virtual void UnSpwan(string name, UnityEngine.Object obj)
    {
        PoolObject plObj = new PoolObject(name, obj);
        m_Objects.Add(plObj);
    }
}

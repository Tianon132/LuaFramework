using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private Transform m_Parent;

    private Dictionary<string, Poolbase> m_Pools = new Dictionary<string, Poolbase>();

    private void Awake()
    {
        m_Parent = this.transform.parent.Find("Pool");
    }

    /************************      创建对象池     **************************/
    private void CreatePool<T>(string poolName, float releaseTime) where T : Poolbase
    {
        if(!m_Pools.TryGetValue(poolName, out Poolbase pool))
        {
            GameObject go = new GameObject(poolName);
            go.transform.SetParent(m_Parent);

            pool = go.AddComponent<T>();   //用这个方式添加脚本
            pool.Init(releaseTime);
            m_Pools.Add(poolName, pool);
        }
    }

    //GameObject对象池 创建
    public void CreateGameObjectPool(string poolName, float releaseTime)
    {
        CreatePool<GameObjectPool>(poolName, releaseTime);
    }

    //AssetBundle对象池 创建
    public void CreateAssetBundlePool(string poolName, float releaseTime)
    {
        CreatePool<AssetBundlePool>(poolName, releaseTime);
    }

    //************************      取出和回收对象     **************************/
    //取出
    public Object Spwan(string poolName, string assetName)
    {
        if (m_Pools.TryGetValue(poolName, out Poolbase pool))
        {
            return pool.Spwan(assetName);
        }
        return null;
    }

    //回收
    public void UnSpwan(string poolName, string assetName, Object obj)
    {
        if (m_Pools.TryGetValue(poolName, out Poolbase pool))
        {
            pool.UnSpwan(assetName, obj);
        }
    }
}

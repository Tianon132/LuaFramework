using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PoolObject
{
    //对象名字
    public string name;

    //具体对象
    public UnityEngine.Object Object;

    //最近一次使用
    public DateTime lastUseTime;

    public PoolObject(string name, UnityEngine.Object obj)
    {
        this.name = name;
        this.Object = obj;
        lastUseTime = DateTime.Now;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class GameStart : MonoBehaviour
{
    public GameMode GameMode;

    private void Start()
    {
        AppConst.GameMode = GameMode;
        DontDestroyOnLoad(this);

        Manager.Resource.ParseVersionFile();
        Manager.Event.Subscribe(10000, OnLuaInit);
        Manager.Lua.Init();//(() =>
        //{
            //Manager.Lua.StratLua("main");

            //LuaFunction func = Manager.Lua.LuaEnv.Global.Get<LuaFunction>("Main");  //这里的Main是文件里面的方法，上面的main是文件名
            //func.Call();
        //});
        
    }

    private void OnLuaInit(object args)
    {
        Manager.Lua.StratLua("main");

        Manager.Pool.CreateGameObjectPool("UIPool", 10);
        Manager.Pool.CreateGameObjectPool("EffectPool", 20);
        Manager.Pool.CreateGameObjectPool("ModelPool", 30);
        Manager.Pool.CreateAssetBundlePool("AssetBundle", 10);

    }

    private void OnApplicationQuit()
    {
        Manager.Event.UnSubscribe(10000, OnLuaInit);
    }
}

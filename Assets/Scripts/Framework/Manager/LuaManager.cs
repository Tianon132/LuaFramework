using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System.IO;

public class LuaManager : MonoBehaviour
{
    //lua名称
    public List<string> luaNames = new List<string>();

    //lua脚本内容
    public Dictionary<string, byte[]> m_luaScripts = new Dictionary<string, byte[]>();

    //xlua虚拟机
    public LuaEnv LuaEnv;

    public void Init()
    {
        LuaEnv = new LuaEnv();
        LuaEnv.AddLoader(Loader);   //新的虚拟机添加自定义Loader

#if UNITY_EDITOR
        if (AppConst.GameMode == GameMode.EditorMode)
            EditorLoadLuaScript();
        else
#endif
            LoadLuaScript();
    }

    public void StratLua(string name)
    {
        LuaEnv.DoString(string.Format("require '{0}'", name));
    }

    byte[] Loader(ref string name)  //自定义Loader
    {
        return GetLuaScript(name);
    }

    /// <summary>
    /// 【查询】目标lua脚本的内容
    /// </summary>
    /// <param name="name">lua名称，从而得到assetName</param>
    /// <returns></returns>
    public byte[] GetLuaScript(string name)
    {
        name = name.Replace('.', '/');   //先将输入的点换成斜杠
        string assetName = PathUtil.GetLuaPath(name);

        byte[] luaScript = null;
        if (!m_luaScripts.TryGetValue(assetName, out luaScript))    //查询
            Debug.LogError("lua open error : " + assetName);
        return luaScript;
    }


/*************************       以下三个方法是获得lua脚本内容的方法       ****************************/
    /// <summary>
    /// 添加lua脚本内容
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="luaInfo"></param>
    void AddLuaScript(string assetName, byte[] luaInfo)
    {
        m_luaScripts[assetName] = luaInfo;
    }

#if UNITY_EDITOR
    /// <summary>
    /// 编辑模式 直接读取lua脚本内容（因为版本文件没更新，信息不全）
    /// 【只能通过遍历所有文件夹】
    /// </summary>
    void EditorLoadLuaScript()
    {
        string[] files = Directory.GetFiles(PathUtil.luaScript, "*.bytes", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            string name = PathUtil.GetStandardPath(files[i]);   //因为是读取文件夹，所以第一件事就是处理路径
            byte[] luaScript = File.ReadAllBytes(name);
            AddLuaScript(PathUtil.GetUnityPath(name), luaScript);
        }
        //InitOk?.Invoke();
        Manager.Event.Fire(10000);
    }
#endif

    /// <summary>
    /// 除编辑模式外的两种模式 来 读取lua脚本内容
    /// 【可以用版本文件直接找】
    /// </summary>
    void LoadLuaScript()
    {
        foreach (var name in luaNames)
        {
            Manager.Resource.LoadLua(name, (UnityEngine.Object obj) =>  //loadLua是为了得到lua文件的内容
            {
                AddLuaScript(name, (obj as TextAsset).bytes);// TextAsset.bytes，可以直接获取相对路径下该文件的内容
                if(m_luaScripts.Count >= luaNames.Count)
                {   //此时说明所有lua文件加载完成，可以回调了
                    //InitOk?.Invoke();
                    Manager.Event.Fire(10000);

                    luaNames.Clear();
                    luaNames = null;//清楚GC
                }
            });
        }
    }

    /*****************************           ********************************/


    private void Update()
    {
        if(LuaEnv != null)
        {
            LuaEnv.Tick();
        }
    }

    private void OnDestroy()
    {
        if(LuaEnv != null)
        {
            LuaEnv.Dispose();
            LuaEnv = null;
        }
    }
}

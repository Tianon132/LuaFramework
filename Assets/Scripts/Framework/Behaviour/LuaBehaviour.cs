using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System;

/// <summary>
/// lua 的 绑定
/// </summary>
public class LuaBehaviour : MonoBehaviour
{
    public LuaEnv m_LuaEnv = Manager.Lua.LuaEnv;

    public LuaTable m_scriptEnv;

    private Action m_luaInit;
    private Action m_luaUpdate;
    //private Action m_luaDestroy;

    private void Awake()
    {
        //设置脚本环境
        m_scriptEnv = m_LuaEnv.NewTable();

        //设置元表
        LuaTable meta = m_LuaEnv.NewTable();
        meta.Set("__index", m_LuaEnv.Global);
        m_scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        //设置变量
        m_scriptEnv.Set("self", this);
    }

    public virtual void Init(string luaName)
    {
        //连接Lua文件与CSharp文件连接在一起（XLua环境下）
        m_LuaEnv.DoString(Manager.Lua.GetLuaScript(luaName), luaName, m_scriptEnv);

        //绑定方法
        m_scriptEnv.Get("OnInit", out m_luaInit);
        m_scriptEnv.Get("Update", out m_luaUpdate);
        //m_luaDestroy = m_scriptEnv.Get<Action>("OnDestroy");

        m_luaInit?.Invoke();
    }

    private void Update()
    {
        m_luaUpdate?.Invoke();
    }

    protected virtual void Clear()
    {
        m_luaInit = null;
        m_luaUpdate = null;
        //m_luaDestroy = null;
    }

    private void OnDestroy()
    {
        //m_luaDestroy?.Invoke();
        Clear();
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}

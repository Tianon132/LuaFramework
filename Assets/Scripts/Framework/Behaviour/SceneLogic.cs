using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SceneLogic : LuaBehaviour
{
    public string sceneName;  //供Lua使用，进入场景需记载该场景的名字

    Action m_LuaActive;
    Action m_LuaInActive;
    Action m_LuaEnter;
    Action m_LuaQuit;

    public override void Init(string luaName)
    {
        base.Init(luaName);
        m_scriptEnv.Get("OnActive", out m_LuaActive);
        m_scriptEnv.Get("OnInActive", out m_LuaInActive);
        m_scriptEnv.Get("OnEnter", out m_LuaEnter);
        m_scriptEnv.Get("OnQuit", out m_LuaQuit);
    }

    public void OnActive()
    {
        m_LuaActive.Invoke();
    }

    public void OnInActive()
    {
        m_LuaInActive.Invoke();
    }

    public void OnEnter()
    {
        m_LuaEnter.Invoke();
    }

    public void OnQuit()
    {
        m_LuaQuit.Invoke();
    }

    protected override void Clear()
    {
        base.Clear();
        m_LuaActive = null;
        m_LuaInActive = null;
        m_LuaEnter = null;
        m_LuaQuit = null;
    }
}

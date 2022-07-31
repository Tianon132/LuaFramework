using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UILogic : LuaBehaviour
{
    public string AssetPath;

    public Action m_OnOpen;
    public Action m_OnClose;

    public override void Init(string luaName)
    {
        base.Init(luaName);
        m_scriptEnv.Get("OnOpen", out m_OnOpen);
        m_OnClose = m_scriptEnv.Get<Action>("OnClose");
    }

    protected override void Clear()
    {
        base.Clear();
        m_OnOpen = null;
        m_OnClose = null;
    }
    
    public void OnOpen()
    {
        m_OnOpen?.Invoke();
    }

    public void OnClose()
    {
        m_OnClose?.Invoke();
        Manager.Pool.UnSpwan("UIPool", AssetPath, this.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EntityLogic : LuaBehaviour
{
    private Action m_OnShow;
    private Action m_OnClose;

    public override void Init(string luaName)
    {
        base.Init(luaName);
        m_scriptEnv.Get("OnShow", out m_OnShow);
        m_OnClose = m_scriptEnv.Get<Action>("OnClose");
    }

    public void OnShow()
    {
        m_OnShow?.Invoke();
    }

    public void OnClose()
    {
        m_OnClose?.Invoke();
    }

    protected override void Clear()
    {
        base.Clear();
        m_OnShow = null;
        m_OnClose = null;
    }
}

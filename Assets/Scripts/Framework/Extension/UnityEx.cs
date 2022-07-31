using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[XLua.LuaCallCSharp]    //注意要添加标签   lua调用C#
public static class UnityEx
{
    public static void OnClickSet(this Button button, object callback)
    {
        XLua.LuaFunction func = callback as XLua.LuaFunction;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            func?.Call();
        });
    }

    public static void OnValueChangedSet(this Slider slider, object callback)
    {
        XLua.LuaFunction func = callback as XLua.LuaFunction;
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener((float value) =>      //需要添加参数
        {
            func?.Call(value);      //每次滑动音量导致结果声音为0，原因是没传入参数
        });
    }
}

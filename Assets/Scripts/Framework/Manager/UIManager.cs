using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //缓存UI
    //private Dictionary<string, GameObject> m_UI = new Dictionary<string, GameObject>();  现在直接缓存在对象池中

    private Dictionary<string, Transform> m_UIGroups = new Dictionary<string, Transform>();

    private Transform m_UIParents;  //实例化父节点

    private void Awake()
    {
        m_UIParents = this.transform.parent.Find("UI");
    }

    public void SetUIGroup(List<string> group)
    {
        for (int i = 0; i < group.Count; i++)
        {
            GameObject go = new GameObject("Group-" + group[i]);
            go.transform.SetParent(m_UIParents, false);
            m_UIGroups.Add(group[i], go.transform);
        }
    }

    public Transform GetUIGroup(string group)
    {
        if (!m_UIGroups.ContainsKey(group))
            Debug.LogError("group is not exist");
        return m_UIGroups[group];
    }

    public void OpenUI(string uiName, string group, string luaName)
    {
        GameObject ui = null;

        //先在对象池中查找看有没有
        string uiPath = PathUtil.GetUIPath(uiName);
        Object uiobj = Manager.Pool.Spwan("UIPool", uiPath);
        if(uiobj != null)
        {
            ui = uiobj as GameObject;
            UILogic uILogic = ui.GetComponent<UILogic>();

            Transform m_Parent = GetUIGroup(group);
            ui.transform.SetParent(m_Parent);

            uILogic.OnOpen();
            return;
        }

        //需要重新创建
        Manager.Resource.LoadUI(uiName, (UnityEngine.Object obj) =>
        {
            ui = Instantiate(obj) as GameObject;

            //*******添加设置父节点部分
            Transform parent = GetUIGroup(group);
            ui.transform.SetParent(parent, false);//false就是不保持世界坐标，跟着父节点走

            UILogic uILogic = ui.GetComponent<UILogic>();
            uILogic.Init(luaName);
            uILogic.AssetPath = uiPath;
            uILogic.m_OnOpen();
        });

    }
}

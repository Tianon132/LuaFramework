using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    //缓存Entity对象
    public Dictionary<string, GameObject> m_Entities = new Dictionary<string, GameObject>();

    //记录Entity分组
    public Dictionary<string, Transform> m_Groups = new Dictionary<string, Transform>();

    //父节点
    public Transform m_EntityParent;

    private void Awake()
    {
        m_EntityParent = this.transform.parent.Find("Entity");
    }

    public void SetEntityGroup(List<string> group)
    {
        for (int i = 0; i < group.Count; i++)
        {
            GameObject go = new GameObject("Group-" + group[i]);
            go.transform.SetParent(m_EntityParent);
            m_Groups.Add(group[i], go.transform);
        }
    }

    public Transform GetEntityGroup(string group)
    {
        if (!m_Groups.ContainsKey(group))
        {
            Debug.LogError("Entity Group is not exist : " + group);
            return null;
        }
        return m_Groups[group];
    }

    public void OpenEntity(string entityName, string group, string luaName)
    {
        GameObject entity = null;
        if (m_Entities.TryGetValue(entityName, out entity))
        {
            EntityLogic entityLogic = entity.GetComponent<EntityLogic>();
            entityLogic.OnShow();
            return;
        }

        Manager.Resource.LoadPrefab(entityName, (UnityEngine.Object obj) =>
        {
            entity = Instantiate(obj) as GameObject;
            entity.transform.SetParent(GetEntityGroup(group));
            m_Entities.Add(entityName, entity);

            EntityLogic entityLogic = entity.AddComponent<EntityLogic>();
            entityLogic.Init(luaName);
            entityLogic.OnShow();
        });
    }
}

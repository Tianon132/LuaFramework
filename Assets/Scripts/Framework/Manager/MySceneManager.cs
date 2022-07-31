using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    string m_LogicName = "[SceneLogic]";

    //注意：ResourceManager中，场景资源不能通过Asset Bundle模式直接读取，但是Editor模式下的AssetDatabase.LoadAssetAtPath()可以

    private void Awake()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }
    
    /// <summary>
    /// 场景加载的回调
    /// </summary>
    private void OnActiveSceneChanged(Scene scene1, Scene scene2)
    {
        if (!scene1.isLoaded || !scene2.isLoaded)    //只要有一个没加载，就无需调用此回调方法
            return;

        SceneLogic logic1 = GetSceneLogic(scene1);
        SceneLogic logic2 = GetSceneLogic(scene2);

        logic1?.OnInActive();
        logic2?.OnActive();
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    public IEnumerator StartLoadScene(string sceneName, string luaName, LoadSceneMode mode)     //养成习惯，如果是携程加上Start，好区分
    {
        //若已经打开就无需加载了
        if(IsLoadScene(sceneName))
        {
            Debug.LogError("scene is Loaded : " + sceneName);
            yield break;
        }

        //1. 异步加载场景
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, mode);//异步加载
        async.allowSceneActivation = true;  //true是指加载之后自动跳转
        yield return async;

        //2. 构建lua对象 ： 因为场景不能直接添加脚本，故需要间接构建lua对象
        Scene scene = SceneManager.GetSceneByName(sceneName);       //问题：之前不小心用了name，导致出问题
        GameObject go = new GameObject(m_LogicName);
        SceneManager.MoveGameObjectToScene(go, scene);      //移动对象到场景中

        //3. 绑定C#与lua脚本
        SceneLogic sceneLogic = go.AddComponent<SceneLogic>();
        sceneLogic.Init(luaName);
        sceneLogic.sceneName = sceneName;
        sceneLogic.OnEnter();
    }

    /// <summary>
    /// 关闭场景
    /// </summary>
    public IEnumerator StartUnLoadScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if(scene.isLoaded)
        {
            Debug.LogError("Scene is Not Loaded : " + sceneName);
            yield break;
        }

        SceneLogic sceneLogic = GetSceneLogic(scene);
        sceneLogic?.OnQuit();       //若有该脚本对象就调用方法
        AsyncOperation async = SceneManager.UnloadSceneAsync(scene);
        yield return async;
    }

    /// <summary>
    /// 场景是否打开（得到Scene判断）
    /// </summary>
    private bool IsLoadScene(string name)
    {
        Scene scene = SceneManager.GetSceneByName(name);
        return scene.isLoaded;
    }

    /// <summary>
    /// 找到 场景中的Logic
    /// </summary>
    private SceneLogic GetSceneLogic(Scene scene)
    {
        GameObject[] gameObjects = scene.GetRootGameObjects();  //GetRootGameObjects 获取场景中所有的根节点
        foreach (var gameObject in gameObjects)
        {
            if (gameObject.name.CompareTo(m_LogicName) == 0)
            {
                SceneLogic sceneLogic = gameObject.GetComponent<SceneLogic>();
                return sceneLogic;
            }
        }
        return null;
    }

    /***************************************     向Lua提供的接口      ***************************************/
    /// <summary>
    /// 叠加  加载场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="luaName"></param>
    public void LoadScene(string sceneName, string luaName)
    {
        Manager.Resource.LoadScene(sceneName, (UnityEngine.Object obj) =>
        {
            StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Additive));
        });
    }

    /// <summary>
    /// 切换场景  不叠加
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="luaName"></param>
    public void ChangeScene(string sceneName, string luaName)
    {
        Manager.Resource.LoadScene(sceneName, (UnityEngine.Object obj) =>
        {
            StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Single));
        });
    }

    /// <summary>
    /// 叠加  卸载场景
    /// </summary>
    /// <param name="sceneName"></param>
    public void UnLoadScene(string sceneName)
    {
        StartCoroutine(StartUnLoadScene(sceneName));
    }

    /// <summary>
    /// 叠加  激活场景
    /// 【SceneManager.SetActiveScene】
    /// </summary>
    public void ActiveScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
    }
}

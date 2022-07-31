using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UObject = UnityEngine.Object;

public class ResourceManager : MonoBehaviour
{
    public class BundleInfo
    {
        public string AssetName;
        public string BundleName;
        public List<string> bundleInfos;
    }

    public class BundleData
    {
        //public string Name; //资源名称（这里采用BundleName，因为是要缓存bundle）       //在下面的字典中保存bundle名字
        public int Count;   //次数
        public AssetBundle AssetBundle;    //资源bundle

        public BundleData(AssetBundle bundle)
        {
            Count = 1;
            AssetBundle = bundle;
        }
    }

    //版本信息
    Dictionary<string, BundleInfo> m_BundleInfos = new Dictionary<string, BundleInfo>();

    //缓存已读取的bundle
    Dictionary<string, BundleData> m_AssetBundles = new Dictionary<string, BundleData>();

    /// <summary>
    /// 解析版本文件
    /// </summary>
    public void ParseVersionFile()
    {
        //首先得到文件地址
        string filePath = Path.Combine(PathUtil.BundleResourcePath, AppConst.fileListName);
        string[] files = File.ReadAllLines(filePath);

        //依次拆解每行信息
        for(int i=0; i<files.Length; i++)
        {
            string[] infos = files[i].Split('|');

            BundleInfo bundleInfo = new BundleInfo();
            bundleInfo.AssetName = infos[0];
            bundleInfo.BundleName = infos[1];

            List<string> bundleRes = new List<string>(infos.Length - 2);
            for(int j=2; j<infos.Length; j++)
            {
                bundleRes.Add(infos[j]);
            }
            bundleInfo.bundleInfos = bundleRes;

            m_BundleInfos.Add(bundleInfo.AssetName, bundleInfo);

            if (infos[0].IndexOf("LuaScript") > 0)
                Manager.Lua.luaNames.Add(infos[0]);//如果是lua文件夹下的，那么就添加
        }
    }

    /***********************        加载资源（编辑模式和bundle模式）        *****************************/
    /// <summary>
    /// 加载依赖bundle
    /// </summary>
    /// <param name="assetName">文件名</param>
    /// <param name="action">回调</param>
    public IEnumerator LoadBundleAsync(string assetName, Action<UObject> action = null)
    {
        Debug.Log("this is bundleLoadAsset");
        //先得到自身bundle路径
        string bundleName = m_BundleInfos[assetName].BundleName;
        string bundlePath = Path.Combine(PathUtil.BundleResourcePath, bundleName);

        //再得到依赖Bundle[1.先加载依赖]
        List<string> bundleInfos = m_BundleInfos[assetName].bundleInfos;
        if (bundleInfos != null && bundleInfos.Count > 0)
        {
            for (int i = 0; i < bundleInfos.Count; i++)
            {
                StartCoroutine(LoadBundleAsync(bundleInfos[i]));//依赖，不需要回调
            }
        }

        //2.再加载自身bundle [bundle不能重复加载]
        BundleData bundle = GetBundle(bundleName);
        if(bundle == null)
        {
            UObject obj = Manager.Pool.Spwan("AssetBundle", bundleName);
            if (obj != null)
            {
                AssetBundle ab = obj as AssetBundle;
                bundle = new BundleData(ab);
            }
            else
            {
                AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath);
                yield return bundleRequest;

                bundle = new BundleData(bundleRequest.assetBundle);
            }

            m_AssetBundles.Add(bundleName, bundle);
        }


        //3.最终实例化
        //回调为空，表示该资源是依赖，不需要实例化
        if (action == null)
        {
            yield break;    
        }

        //检查是否是场景，场景不用实例化
        if (assetName.EndsWith(".unity"))
        {
            action?.Invoke(null);       //因为加载场景，无需Obj参数
            yield break;        //若是场景文件，则只需加载ab包即可
        }

        AssetBundleRequest request = bundle.AssetBundle.LoadAssetAsync(assetName);
        yield return request;

        action?.Invoke(request.asset);//action打开文件实例化，依赖文件只需要加载bundle，不需要实例化
    }

    private BundleData GetBundle(string bundleName)
    {
        BundleData bundle = null;
        if(m_AssetBundles.TryGetValue(bundleName, out bundle))
        {
            bundle.Count++;
            return bundle;
        }
        return null;
    }

#if UNITY_EDITOR
    //编辑模式下的打开bundle
    void LoadEditorAsset(string assetName, Action<UObject> action = null)
    {
        Debug.Log("this is EditorLoadAsset");
        UObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetName, typeof(UObject));
        if (obj == null)
            Debug.LogError("asset name is not exist: " + assetName);
        action?.Invoke(obj);
    }
#endif

    //选择打开资源的方式
    void LoadAsset(string assetName, Action<UObject> action = null)
    {
#if UNITY_EDITOR
        if (AppConst.GameMode == GameMode.EditorMode)
            LoadEditorAsset(assetName, action);
        else
#endif
            StartCoroutine(LoadBundleAsync(assetName, action));
    }

    //规划具体路径
    public void LoadUI(string assetName, Action<UObject> action = null)
    {
        LoadAsset(PathUtil.GetUIPath(assetName), action);
    }

    public void LoadMusic(string assetName, Action<UObject> action = null)
    {
        LoadAsset(PathUtil.GetMusicPath(assetName), action);
    }

    public void LoadSound(string assetName, Action<UObject> action = null)
    {
        LoadAsset(PathUtil.GetSoundPath(assetName), action);
    }

    public void LoadScene(string assetName, Action<UObject> action = null)
    {
        LoadAsset(PathUtil.GetScenePath(assetName), action);
    }

    public void LoadEffect(string assetName, Action<UObject> action = null)
    {
        LoadAsset(PathUtil.GetEffectPath(assetName), action);
    }

    public void LoadLua(string assetName, Action<UObject> action = null)
    {
        LoadAsset(assetName, action);
    }

    public void LoadPrefab(string assetName, Action<UObject> action = null)
    {
        LoadAsset(assetName, action);
    }

    /***************************          卸载资源           *******************************/
    //卸载bundle
    public void UnLoadBundle(UObject obj)
    {
        AssetBundle ab = obj as AssetBundle;
        ab.Unload(true);
    }

    //减少多个bundle使用次数
    public void MinusBundleCount(string assetName)
    {
        //先解除自身bundle
        string bundleName = m_BundleInfos[assetName].BundleName;//再加上自身bundle
        MinusOneBundleCount(bundleName);

        //在接触依赖bundle
        List<string> dependences = m_BundleInfos[assetName].bundleInfos;//先得到依赖
        if(dependences != null)
        {
            foreach (string item in dependences)
            {
                string name = m_BundleInfos[item].BundleName;
                MinusOneBundleCount(name);
            }
        }
    }

    //减少单个bundle使用次数
    public void MinusOneBundleCount(string bundleName)
    {
        if (m_AssetBundles.TryGetValue(bundleName, out BundleData bundle))
        {
            if(bundle.Count > 0)
            {
                bundle.Count--;
                Debug.Log("bundle引用计数 : " + bundleName + " count : " + bundle.Count);
            }
                

            if (bundle.Count == 0)
            {
                Debug.Log("放入bundle对象池 : " + bundleName);
                Manager.Pool.UnSpwan("AssetBundle", bundleName, bundle.AssetBundle);
                m_AssetBundles.Remove(bundleName);
            }
        }
    }
}

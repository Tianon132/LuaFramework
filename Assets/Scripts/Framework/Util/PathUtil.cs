using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUtil : MonoBehaviour
{
    //根目录
    public static readonly string AssetPath = Application.dataPath;

    //资源打包目录
    public static readonly string ResourcesBuildPath = AssetPath + "/BuildResources/";

    //资源输出目录
    public static readonly string StreamingAssetsPath = Application.streamingAssetsPath;

    //只读目录
    public static readonly string ReadPath = Application.streamingAssetsPath;

    //可读写目录
    public static readonly string ReadWritePath = Application.persistentDataPath;

    //luaScript目录
    public static readonly string luaScript = "Assets/BuildResources/LuaScript";

    public static string BundleResourcePath//bundle资源路径
    {
        get
        {
            if (AppConst.GameMode == GameMode.UpdateMode)
                return ReadWritePath;
            return ReadPath;
        }
    }

    /// <summary>
    /// 得到Assets下的相对目录
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetUnityPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;
        return path.Substring(path.IndexOf("Assets"));
    }

    /// <summary>
    /// 重置反斜杠
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetStandardPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;
        return path.Replace("\\", "/");
    }

    public static string GetUIPath(string name)
    {
        return string.Format("Assets/BuildResources/UI/Prefabs/{0}.prefab", name);
    }

    public static string GetLuaPath(string name)
    {
        return string.Format("Assets/BuildResources/LuaScript/{0}.bytes", name);
    }

    public static string GetMusicPath(string name)
    {
        return string.Format("Assets/BuildResources/Audio/Music/{0}", name);//因为音乐有多种格式，所以不加后缀
    }

    public static string GetSoundPath(string name)
    {
        return string.Format("Assets/BuildResources/Audio/Sound/{0}", name);
    }

    public static string GetScenePath(string name)
    {
        return string.Format("Assets/BuildResources/Scene/{0}.unity", name);
    }

    public static string GetEffectPath(string name)
    {
        return string.Format("Assets/BuildResources/Effect/Prefabs/{0}.prefab", name);
    }

    public static string GetPrefabPath(string name)
    {
        return string.Format("Assets/BuildResources/Model/Prefabs/{0}.prefab", name);
    }
}

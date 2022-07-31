using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum GameMode
{
    EditorMode,     //编辑模式
    PakeageMode,    //ab包模式
    UpdateMode      //热更新模式
}

public class AppConst
{
    public static readonly string bundleSuffix = ".ab";
    public static readonly string fileListName =  "filelist.txt";
    public static readonly string ResourcesUrl = "http://127.0.0.1/AssetBundlesTest";

    public static GameMode GameMode = GameMode.EditorMode;
}
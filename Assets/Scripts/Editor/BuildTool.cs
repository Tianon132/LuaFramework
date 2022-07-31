using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class BuildTool : Editor
{
    [MenuItem("Tools/Build Window Bundle")]
    static void BuildWindowBundle()
    {
        Build(BuildTarget.StandaloneWindows);
    }

    [MenuItem("Tools/Build Android Bundle")]
    static void BuildAndroidBundle()
    {
        Build(BuildTarget.Android);
    }

    [MenuItem("Tools/Build Ios Bundle")]
    static void BuildIosBundle()
    {
        Build(BuildTarget.iOS);
    }

    static void Build(BuildTarget target)
    {
        //要建立的Bundle信息
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
        //依赖文件的信息
        List<string> BundleInfos = new List<string>();

        //具体要被检查的资源文件
        string[] files = Directory.GetFiles(PathUtil.ResourcesBuildPath, "*", SearchOption.AllDirectories);

        for (int i = 0; i < files.Length; i++)
        {
            //1. 排除meta文件
            if (files[i].EndsWith(".meta"))
                continue;

            //2. 构建bundle信息
            AssetBundleBuild assetBundle = new AssetBundleBuild();
            string fileName = PathUtil.GetStandardPath(files[i]);
            Debug.LogFormat("fileBuild : {0}", fileName);

            string assetName = PathUtil.GetUnityPath(fileName);
            assetBundle.assetNames = new string[] { assetName };    //文件原地址名

            string bundleName = fileName.Replace(PathUtil.ResourcesBuildPath, "").ToLower();    //fileName.Replace错误写成assetName.Replace，而ResourcesBuildPath是全路径。assetName是相对路径，故不能使用这个
            assetBundle.assetBundleName = bundleName + ".ab";       //文件依赖名

            assetBundleBuilds.Add(assetBundle);


            //3. 构建依赖信息
            List<string> dependences = GetDependencs(assetName);
            string info = assetName + '|' + bundleName + AppConst.bundleSuffix;  //自身地址|自身bundle|依赖信息

            if(dependences.Count > 0)
                info = info + '|' + string.Join("|", dependences);
            BundleInfos.Add(info);
        }

        //创建文件夹 
        if (Directory.Exists(PathUtil.StreamingAssetsPath))
            Directory.Delete(PathUtil.StreamingAssetsPath, true);
        Directory.CreateDirectory(PathUtil.StreamingAssetsPath);

        //写入文件夹将依赖信息
        File.WriteAllLines(Path.Combine(PathUtil.StreamingAssetsPath, AppConst.fileListName), BundleInfos);

        //构建Bundle
        BuildPipeline.BuildAssetBundles(PathUtil.StreamingAssetsPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, target);

        AssetDatabase.Refresh();
    }

    static List<string> GetDependencs(string curFile)
    {
        List<string> dependences = new List<string>();
        string[] files = AssetDatabase.GetDependencies(curFile);
        dependences = files.Where(file => !file.EndsWith(".cs") && !file.Equals(curFile)).ToList();
        return dependences;
    }
}

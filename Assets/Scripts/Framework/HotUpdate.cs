using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;

public class HotUpdate : MonoBehaviour
{
    byte[] m_DownLoadReadPathFileList;
    byte[] m_DownLoadServerPathFileList;

    public class DownFileInfo
    {
        public string fileName;
        public string url;
        public DownloadHandler fileData;
    }

    /// <summary>
    /// 下载单个文件
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    IEnumerator DownLoadFile(DownFileInfo fileInfo, Action<DownFileInfo> action)
    {
        //UnityWebRequest request = new UnityWebRequest(fileInfo.url);
        UnityWebRequest request = UnityWebRequest.Get(fileInfo.url);        //使用UnityWebRequest.Get方法得到网络
        yield return request.SendWebRequest();

        if(request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("request is down error : " + fileInfo.url);
            yield break;//终止运行，跳出循环
        }

        fileInfo.fileData = request.downloadHandler;
        action?.Invoke(fileInfo);
        request.Dispose();
    }

    /// <summary>
    /// 下载多个文件
    /// </summary>
    /// <param name="fileInfos"></param>
    /// <param name="action"></param>
    /// <param name="allAction"></param>
    /// <returns></returns>
    IEnumerator DownLoadFile(List<DownFileInfo> fileInfos, Action<DownFileInfo> action, Action allAction)
    {
        foreach (var info in fileInfos)
        {
            yield return DownLoadFile(info, action);
        }
        allAction?.Invoke();    
    }

    /// <summary>
    /// 获取文件信息内容【这里一般用于查看 版本文件】
    /// </summary>
    /// <param name="fileData"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    private List<DownFileInfo> GetFileInfo(string fileData, string path)
    {
        string content = fileData.Trim().Replace("\r", "");
        string[] files = content.Split('\n');
        List<DownFileInfo> fileInfos = new List<DownFileInfo>();
        foreach(var file in files)
        {
            string[] infos = file.Split('|');
            DownFileInfo fileInfo = new DownFileInfo();
            fileInfo.fileName = infos[1];
            fileInfo.url = Path.Combine(path, infos[1]);
            fileInfos.Add(fileInfo);
        }
        return fileInfos;
    }

    private void Start()
    {
        //if (AppConst.GameMode == GameMode.UpdateMode)
        //{
        //    if (IsFirstInstall())
        //        ReleaseResources();
        //    else
        //        CheckUpdate();
        //} 
        //else
        //{
        //    EnterGame();
        //}
        if (IsFirstInstall())
            ReleaseResources();
        else
            CheckUpdate();
    }

    private bool IsFirstInstall()       //检查版本文件是否存在即可【只有整包模式下需要检查是否初次安装】
    {
        bool isExistReadPathFileList = FileUtil.IsExist(Path.Combine(PathUtil.ReadPath, AppConst.fileListName));

        bool isExistReadWritePathFileList = FileUtil.IsExist(Path.Combine(PathUtil.ReadWritePath, AppConst.fileListName));

        return isExistReadPathFileList && !isExistReadWritePathFileList;
    }

    /**************************    检查初次安装板块     *************************/
    private void ReleaseResources()     //找到版本文件地址，下载获取信息
    {
        string url = Path.Combine(PathUtil.ReadPath, AppConst.fileListName);
        DownFileInfo fileInfo = new DownFileInfo();
        fileInfo.url = url;
        StartCoroutine(DownLoadFile(fileInfo, OnDownLoadReadPathAllFiles));
    }

    private void OnDownLoadReadPathAllFiles(DownFileInfo info)      //下载所有文件
    {
        m_DownLoadReadPathFileList = info.fileData.data;
        List<DownFileInfo> infos = GetFileInfo(info.fileData.text, PathUtil.ReadPath);
        StartCoroutine(DownLoadFile(infos, OnDownLoadReadPathFileFinish, OnDownLoadReadPathAllFilesFinish));
    }

    private void OnDownLoadReadPathFileFinish(DownFileInfo info)    //每个文件下载完写入
    {
        Debug.Log("OnDownLoadReadPathFileFinish" + info.url);
        string path = Path.Combine(PathUtil.ReadWritePath, info.fileName);
        FileUtil.WriteFile(path, info.fileData.data);
    }

    private void OnDownLoadReadPathAllFilesFinish()                 //最后写入版本文件
    {
        string path = Path.Combine(PathUtil.ReadWritePath, AppConst.fileListName);
        FileUtil.WriteFile(path, m_DownLoadReadPathFileList);
        CheckUpdate();
    }

    /**************************    检查更新板块     *************************/
    private void CheckUpdate()
    {
        string url = Path.Combine(AppConst.ResourcesUrl, AppConst.fileListName);
        DownFileInfo fileInfo = new DownFileInfo();
        fileInfo.url = url;
        StartCoroutine(DownLoadFile(fileInfo, OnDownLoadServerAllFiles));
    }

    private void OnDownLoadServerAllFiles(DownFileInfo info)
    {
        m_DownLoadServerPathFileList = info.fileData.data;
        List<DownFileInfo> fileInfos = GetFileInfo(info.fileData.text, AppConst.ResourcesUrl);
        List<DownFileInfo> downFiles = new List<DownFileInfo>();

        for (int i = 0; i < fileInfos.Count; i++)
        {
            string localPath = Path.Combine(PathUtil.ReadWritePath, fileInfos[i].fileName);
            if(!FileUtil.IsExist(localPath))    //修改1*************应该是不存在
            {                   //下面是最大的问题，没有赋予url地址
                fileInfos[i].url = Path.Combine(AppConst.ResourcesUrl, fileInfos[i].fileName);  //这里应改为要下载的地址
                downFiles.Add(fileInfos[i]);
            }
        }

        //修改2************************添加判断，若是有才下载
        if (downFiles.Count > 0)
            StartCoroutine(DownLoadFile(downFiles, OnDownLoadServerFileFInish, OnDownLoadServerAllFilesFinish));
        else
            EnterGame();
    }

    private void OnDownLoadServerFileFInish(DownFileInfo info)
    {
        Debug.Log("OnDownLoadServerFileFInish : " + info.url);
        string path = Path.Combine(PathUtil.ReadWritePath, info.fileName);
        FileUtil.WriteFile(path, info.fileData.data);
    }

    private void OnDownLoadServerAllFilesFinish()
    {
        string path = Path.Combine(PathUtil.ReadWritePath, AppConst.fileListName);
        FileUtil.WriteFile(path, m_DownLoadServerPathFileList);
        EnterGame();
    }

    private void EnterGame()
    {
        //if(AppConst.GameMode != GameMode.EditorMode)
        //    Manager.Resource.ParseVersionFile();
        Manager.Resource.ParseVersionFile();
        Manager.Resource.LoadUI("Login/LoginUI", OnCreate);
    }

    public void OnCreate(UnityEngine.Object obj)
    {
        GameObject go = Instantiate(obj) as GameObject;
        go.transform.SetParent(this.transform);
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
    }
}

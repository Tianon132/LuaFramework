using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileUtil : MonoBehaviour
{
    /// <summary>
    /// 文件是否存在
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsExist(string path)
    {
        FileInfo file = new FileInfo(path);
        return file.Exists;
    }

    /// <summary>
    /// 写入文件
    /// </summary>
    /// <param name="path">包括文件名在内的完整路径</param>
    /// <param name="data"></param>
    public static void WriteFile(string path, byte[] data)
    {
        //1.检查文件夹
        path = PathUtil.GetStandardPath(path);
        string dir = path.Substring(0, path.LastIndexOf('/'));
        if (!Directory.Exists(dir))         //问题1：是文件夹不存在的时候创建，忘记加！号
            Directory.CreateDirectory(dir);

        //2.检查文件
        FileInfo info = new FileInfo(path);
        //if (IsExist(path))    //问题2：这个方法也是使用FileInfo，因为这边要使用删除，就不用这个了，否则会创建两次
        if (info.Exists)
            info.Delete();//若文件存在就删除
        try
        {
            using(FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Write(data, 0, data.Length);
                fs.Close();
            }
        }
        catch (IOException e)
        {
            Debug.LogError(e.Message);
        }
    }
}

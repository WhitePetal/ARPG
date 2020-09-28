/*********************************************************
	文件：SaveSerializeXML
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/23 9:39:15
	功能：将XML序列化，并存储至文件
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class SaveSerializeXML : Editor
{
    [MenuItem("Tools/SaveSerializeXML")]
    static void Save()
    {
        string path = EditorUtility.OpenFilePanelWithFilters("XML", "", new string[]{ "xml file", "xml" });
        if (string.IsNullOrEmpty(path)) return;
        using (FileStream readFs = File.OpenRead(path))
        {
            byte[] buffer = new byte[1024];
            int len = 0;

            using (FileStream writeFs = File.Create(path.Replace(".xml", ".bytes")))
            {
                using (GZipStream gzs = new GZipStream(writeFs, CompressionMode.Compress, true))
                {
                    while ((len = readFs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        gzs.Write(buffer, 0, len);
                    }
                }
            }
        }
    }

    [MenuItem("Tools/DeSerializeXML")]
    static void DeSerializeXML()
    {
        string path = EditorUtility.OpenFilePanelWithFilters("Bytes", "", new string[] { "byte file", "bytes" });
        if (string.IsNullOrEmpty(path)) return;
        using (FileStream readFs = File.OpenRead(path))
        {
            using (FileStream writeFs = File.Create(path.Replace(".bytes", ".xml")))
            {
                using (GZipStream gzs = new GZipStream(readFs, CompressionMode.Decompress, true))
                {
                    byte[] buffer = new byte[1024];
                    int len = 0;
                    while((len = gzs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        writeFs.Write(buffer, 0, len);
                    }
                }
            }

        }
    }
}

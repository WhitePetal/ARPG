/*********************************************************
	文件：CreateSqlWindow
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/19 14:14:09
	功能：根据脚本写入该脚本的Sql操作方法
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.IO;
using System.Text;

public class CreateSqlWindow : Editor
{
    private static CreateSqlWindow window;

    static string path;

    [MenuItem("Tools/CreateSql")]
    static void ShowWindow()
    {
        path = EditorUtility.OpenFilePanelWithFilters("选择脚本", "", new string[] { "cs file", "cs", "all file", "*" });
        if (path != null && path != "") CreateSql(path);
    }

    private static void CreateSql(string path)
    {
        FileStream stream = File.OpenWrite(path);

        string className = Path.GetFileNameWithoutExtension(path);
        Type cls = Type.GetType(className);

        WriteInsertSql(cls, stream);
        WriteUpdateSql(cls, stream);

        stream.Close();
    }
    private static void WriteInsertSql(Type cls, FileStream stream)
    {
        string lowerClsName = cls.Name.ToLower();
        // 组装 Sql 语句
        StringBuilder sb = new StringBuilder();
        sb.Append("insert into #table set ");
        foreach(FieldInfo field in cls.GetFields())
        {
            sb.Append(field.Name);
            sb.Append("=@");
            sb.Append(field.Name);
            sb.Append(',');
        }
        sb.Remove(sb.Length - 1, 1);
        string sql = sb.ToString();
        sb.Clear();
        // 组装 function
        sb.Append("\nprivate bool Insert");
        sb.Append(cls.Name);
        sb.Append('(');
        sb.Append(cls.Name);
        sb.Append(" ");
        sb.Append(lowerClsName);
        sb.Append(")\n{\n");
        sb.Append("\ttry\n\t{\n");
        sb.Append("\t\tMysqlCommand cmd = new MysqlCommand(");
        sb.Append('"');
        sb.Append(sql);
        sb.Append('"');
        sb.Append(", conn);\n");
        foreach (FieldInfo field in cls.GetFields())
        {
            sb.Append("\t\tcmd.Parameters.AddWithValue(\"");
            sb.Append(field.Name);
            sb.Append("\", ");
            sb.Append(lowerClsName);
            sb.Append('.');
            sb.Append(field.Name);
            sb.Append(");\n");
        }
        sb.Append("\t\tcmd.ExecuteNonQuery();\n\t}\n\tcatch(Exception e)\n\t{\n\t\tNETCommon.Log(\"DB Insert ");
        sb.Append(cls.Name);
        sb.Append(" Data Error\" + e,  NETLogLevel.Error);\n\t\treturn false;\n\t}\n\treturn true;\n}");

        byte[] btyes = Encoding.UTF8.GetBytes(sb.ToString());
        stream.Seek(1, SeekOrigin.End);
        stream.Write(btyes, 0, btyes.Length);
        stream.Flush();
    }
    private static void WriteUpdateSql(Type cls, FileStream stream)
    {
        string lowerClsName = cls.Name.ToLower();
        StringBuilder sb = new StringBuilder();
        // 组装Sql
        sb.Append("update #table set ");
        foreach (FieldInfo field in cls.GetFields())
        {
            sb.Append(field.Name);
            sb.Append("=@");
            sb.Append(field.Name);
            sb.Append(',');
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append("where id=@id");
        string sql = sb.ToString();
        sb.Clear();
        // 组装 function
        sb.Append("\nprivate bool Update#cls(#cls #lowercls)\n{\n\ttry\n\t{\n\t\tMysqlCommand cmd = new MysqlCommand(\"");
        sb.Append(sql);
        sb.Append("\", conn):\n\t\t");
        string template = "cmd.Parameters.AddWithValue(\"#field\", #lowercls.#field);\n\t\t";
        foreach (FieldInfo field in cls.GetFields()) sb.Append(template.Replace("#field", field.Name));
        sb.Append("cmd.ExecuteNonQuery();\n\tcatch(Exception e)\n\t{\n\t\tNETCommon.Log(\"Update #clsData Error: \" + e, NETLogLevel.Error);\n\t\treturn false;\n\t}\n\treturn true;\n}");
        //string function = sb.ToString().Replace("#cls", cls.Name).Replace("#lowercls", lowerClsName);
        string function = Replace(sb.ToString(), "#cls", cls.Name);
        function = Replace(function, "#lowercls", lowerClsName);
        byte[] btyes = Encoding.UTF8.GetBytes(function);
        stream.Seek(1, SeekOrigin.End);
        stream.Write(btyes, 0, btyes.Length);
        stream.Flush();
    }

    private static string Replace(string str, string oldStr, string newStr)
    {
        if (str == null || str.Length == 0) return null;
        int end = str.Length - 1;
        int head = str.Length - oldStr.Length;
        List<char> list = new List<char>();
        while(head >= 0)
        {
            if(str[head] == oldStr[0])
            {
                if(ChecChildStr(str,oldStr, head, end))
                {
                    for (int i = newStr.Length - 1; i >= 0; i--) list.Add(newStr[i]);
                    end -= oldStr.Length;
                    head -= oldStr.Length;
                    continue;
                }
            }
            list.Add(str[end]);
            end--;
            head--;
        }
        for(int i = end; i >= 0; i--)
        {
            list.Add(str[i]);
        }
        StringBuilder sb = new StringBuilder();
        for(int i = list.Count - 1; i >= 0; i--)
        {
            sb.Append(list[i]);
        }
        return sb.ToString();
    }
    private static bool ChecChildStr(string str,string oldStr, int head, int end)
    {
        int index = oldStr.Length - 1;
        for(int i = end; i >= head; i--)
        {
            if (str[i] != oldStr[index]) return false;
            index--;
        }

        return true;
    }
}

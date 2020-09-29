/*********************************************************
	文件：Utility
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/22 9:50:57
	功能：工具类
***********************************************************/
using PENet;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace Protocol
{
    public class Utility
    {
        public static byte[] SerializeToObjectByte<T>(T data)
        {
            byte[] buffer = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, data);
                    buffer = ms.ToArray();
                }
            }
            catch(Exception e)
            {
                NETCommon.Log("序列化失败: " + e, NETLogLevel.Error);
            }
            return buffer;
        }
        public static T DeSerializeToObject<T>(byte[] buffer)
        {
            T data = default(T);
            using (MemoryStream rs = new MemoryStream(buffer))
            {
                BinaryFormatter bf = new BinaryFormatter();
                try
                {
                    data = (T)bf.Deserialize(rs);
                }
                catch (Exception e)
                {
                    NETCommon.Log("反序列化失败: " + e, NETLogLevel.Error);
                }
            }

            return data;
        }

        public static byte[] SerializeToObjectByte_GZip<T>(T data)
        {
            try
            {
                byte[] buffer = SerializeToObjectByte(data);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (GZipStream gzs = new GZipStream(ms, CompressionMode.Compress, true))
                    {
                        gzs.Write(buffer, 0, buffer.Length);
                        gzs.Close();
                        return ms.ToArray();
                    }
                }
            }
            catch(Exception e)
            {
                NETCommon.Log("序列化失败: " + e, NETLogLevel.Error);
                return null;
            }
        }
        public static T DeSerizlizeToObject_GZip<T>(byte[] buffer)
        {
            try
            {
                byte[] deCompressBuffer = null;
                using (MemoryStream inputMS = new MemoryStream(buffer))
                {
                    using (MemoryStream outputMS = new MemoryStream())
                    {
                        using (GZipStream gzs = new GZipStream(inputMS, CompressionMode.Decompress, true))
                        {
                            byte[] bytes = new byte[1024];
                            int len = 0;
                            while ((len = gzs.Read(bytes, 0, bytes.Length)) > 0)
                            {
                                outputMS.Write(bytes, 0, len);
                            }
                            gzs.Close();
                            deCompressBuffer = outputMS.ToArray();
                        }
                    }
                }

                using (MemoryStream ms = new MemoryStream(deCompressBuffer))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    return (T)bf.Deserialize(ms);
                }
            }
            catch(Exception e)
            {
                NETCommon.Log("反序列失败: " + e, NETLogLevel.Error);
                return default(T);
            }
        }

        public static byte[] PackMsg<T>(T data)
        {
            byte[] buffer = SerializeToObjectByte_GZip(data);
            int len = buffer.Length;
            byte[] pack = new byte[len + 4];
            byte[] head = BitConverter.GetBytes(len);
            head.CopyTo(pack, 0);
            buffer.CopyTo(pack, 4);
            return pack;
        }

        public static byte[] SerializeToXmlByte<T>(T data)
        {
            byte[] buffer = null;
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                using (MemoryStream ms = new MemoryStream())
                {
                    xs.Serialize(ms, data);
                    buffer = ms.ToArray();
                }
            }
            catch(Exception e)
            {
                NETCommon.Log("序列化失败: " + e, NETLogLevel.Error);
            }

            return buffer;

        }
        public static void XmlByteToXmlFile(string targetPath, byte[] buffer)
        {
            try
            {
                using (FileStream fs = new FileStream(targetPath, FileMode.Create))
                {
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Flush();
                    fs.Close();
                }
            }
            catch(Exception e)
            {
                NETCommon.Log("反序列失败: " + e, NETLogLevel.Error);
            }
        }
        public static T XmlByteToObject<T>(byte[] buffer)
        {
            T data = default(T);
            try
            {
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    data = (T)xs.Deserialize(ms);
                }
            }
            catch(Exception e)
            {
                NETCommon.Log("反序列失败: " + e, NETLogLevel.Error);
            }
            return data;
        }
        public static string XmlByteToStr(byte[] buffer)
        {
            string str = "";
            try
            {
                str = Encoding.UTF8.GetString(buffer);
            }
            catch(Exception e)
            {
                NETCommon.Log("反序列失败: " + e, NETLogLevel.Error);
            }

            return str;
        }
    }
}

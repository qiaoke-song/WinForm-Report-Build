/*---------------------------------------------
整理自用 EzReport Build 报表组件
适用版本：.Net 2.0 - .Net 4.6 (32、64位)
设计：Song Qiao Ke  
          Email: Qiaoke_Song@163.com
          QQ：2452243110
最后更新：2018.11
-----------------------------------------------*/
using System.Runtime.Serialization.Json;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;

public class Serializer
{
    /// 将对象序列化为json文件
    public static void ObjectToJson<T>(T t, string path) where T : class
    {
        DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(T));
        using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
        {
            formatter.WriteObject(stream, t);
        }
    }

    /// 将对象序列化为json字符串
    public static string ObjectToJson<T>(T t) where T : class
    {
        DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(T));
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.WriteObject(stream, t);
            string result = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            return result;
        }
    }

    /// json字符串转成对象
    public static T JsonToObject<T>(string json) where T : class
    {
        DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(T));
        using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
        {
            T result = formatter.ReadObject(stream) as T;
            return result;
        }
    }

    /// 将对象序列化为xml文件
    public static void ObjectToXml<T>(T t, string path) where T : class
    {
        XmlSerializer formatter = new XmlSerializer(typeof(T));
        using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
        {
            formatter.Serialize(stream, t);
        }
    }

    /// 将对象序列化为xml字符串
    public static string ObjectToXml<T>(T t) where T : class
    {
        XmlSerializer formatter = new XmlSerializer(typeof(T));
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, t);
            string result = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            return result;
        }
    }

    /// 将xml文件反序列化为对象
    public static T XmlToObject<T>(T t, string path) where T : class
    {
        XmlSerializer formatter = new XmlSerializer(typeof(T));
        using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
        {
            XmlReader xmlReader = new XmlTextReader(stream);
            T result = formatter.Deserialize(xmlReader) as T;
            return result;
        }
    }

    /// 将xml字符串反序列化为对象
    public static T XmlStrToObject<T>(T t, string str) where T : class
    {
        XmlSerializer formatter = new XmlSerializer(typeof(T));

        StringReader xmlReader = new StringReader(str);
        T result = formatter.Deserialize(xmlReader) as T;
        return result;
    }


    /// 将对象序列化为二进制流
    public static byte[] ObjectToString<T>(T t)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, t);
            byte[] result = stream.ToArray();
            return result;
        }
    }

    /// 将二进制流反序列为类型
    public static T StringToObject<T>(byte[] bytes) where T : class
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream(bytes, 0, bytes.Length))
        {
            T result = formatter.Deserialize(stream) as T;
            return result;
        }
    }

    /// 将对象序列化为文件
    public static void ObjectToFile<T>(T t, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
        {
            formatter.Serialize(stream, t);
            stream.Flush();
        }
    }

    /// 将文件反序列化为对象
    public static T FileToObject<T>(string path) where T : class
    {
        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            T result = formatter.Deserialize(stream) as T;
            return result;
        }
    }
}



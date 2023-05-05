using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;

namespace E9361App.Json
{
    public class JsonProcess
    {
        public static bool ReadJsonFile<T>(string fileName, ref T t, TypeNameHandling typeHandle = TypeNameHandling.Auto)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                StreamReader rd = new StreamReader(file);

                string jsonContent = rd.ReadToEnd();
                rd.Close();
                file.Close();
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = typeHandle };
                t = JsonConvert.DeserializeObject<T>(jsonContent, settings);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ReadJsonFile<T>(string fileName, ref T t)
        {
            return ReadJsonFile(fileName, ref t, TypeNameHandling.None);
        }

        public static void SaveToJsonFile<T>(string fileName, T t, TypeNameHandling typeHandle = TypeNameHandling.Auto)
        {
            string fullFileName = System.IO.Path.GetFullPath(fileName);
            string path = System.IO.Path.GetDirectoryName(fullFileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = typeHandle };
            string strJson = JsonConvert.SerializeObject(t, Formatting.Indented, settings);
            StreamWriter sw = null;
            sw = File.CreateText(fullFileName);
            sw.WriteLine(strJson);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

        public static void SaveToJsonFile<T>(string fileName, T t)
        {
            SaveToJsonFile(fileName, t, TypeNameHandling.None);
        }

        public static void SaveToJsonFile<T>(string fileName, T t, IsoDateTimeConverter f)
        {
            string fullFileName = System.IO.Path.GetFullPath(fileName);
            string path = System.IO.Path.GetDirectoryName(fullFileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string strJson = JsonConvert.SerializeObject(t, Formatting.Indented, f);
            StreamWriter sw = null;
            sw = File.CreateText(fullFileName);
            sw.WriteLine(strJson);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

        public static T DeserializeJsonString<T>(string jsonstr, TypeNameHandling typeHandle = TypeNameHandling.None)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = typeHandle };
            return JsonConvert.DeserializeObject<T>(jsonstr, settings);
        }
    }
}
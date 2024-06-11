using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;


public class JsonDataService : IDataService
{
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }



    public bool SaveData<T>(string RelativePath, T[] Data, bool Encrypted) 
    {
        string path = Application.persistentDataPath + RelativePath;    

        try 
        {
            if (File.Exists(path)) 
            {
                Debug.Log("Data Exists deleting old file and writing new one !");
                File.Delete(path);
            }
            else 
            {
                Debug.Log("Writing file for the first time !");
            }
            
            File.Delete(path);
            using FileStream stream = File.Create(path);
            stream.Close();
            
            Wrapper<T> _wrapper = new Wrapper<T>();
            _wrapper.Items = Data;

            File.WriteAllText(path, JsonConvert.SerializeObject(Data,Formatting.Indented));
            return true;
        } 
        catch(Exception e) 
        {
            Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
            return false;
        }
    }


    public T[] LoadData<T>(string RelativePath, bool encrypted) 
    {
        string path = Application.persistentDataPath + RelativePath;

        if (!File.Exists(path))
        {
            Debug.LogError($"Cannot load file at {path}. File does not exists!");
            throw new FileNotFoundException();
        }

        try
        {
            //T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            Wrapper<T> _wrapper = new Wrapper<T>();
            _wrapper = JsonConvert.DeserializeObject<Wrapper<T>>(File.ReadAllText(path));
            return _wrapper.Items;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data due to : {e.Message} {e.StackTrace}");
            throw e;
        }
    }

    /*
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        
    }
    */

}

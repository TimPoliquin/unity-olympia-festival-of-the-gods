using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Azul
{
    namespace Utils
    {
        public sealed class FileUtils
        {
            public static T LoadResource<T>(string fileName) where T : class
            {
                string destination = Application.persistentDataPath + "/" + fileName;
                FileStream file;

                if (File.Exists(destination))
                {
                    file = File.OpenRead(destination);
                }
                else
                {
                    UnityEngine.Debug.Log($"File not found: {fileName}");
                    return null;
                }
                BinaryFormatter bf = new BinaryFormatter();
                T data = (T)bf.Deserialize(file);
                file.Close();
                return data;
            }

            public static void SaveResource<T>(string fileName, T resource) where T : class
            {
                string destination = Application.persistentDataPath + "/" + fileName;
                FileStream file;

                if (File.Exists(destination)) file = File.OpenWrite(destination);
                else file = File.Create(destination);

                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, resource);
                file.Close();
            }
        }

    }
}

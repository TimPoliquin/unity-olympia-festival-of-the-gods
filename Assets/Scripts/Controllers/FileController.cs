using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Azul.Model;
using Azul.Util;
using Azul.Utils;
using Codice.Client.BaseCommands;
using GluonGui.Dialog;
using Unity.Jobs;
using UnityEngine;

namespace Azul
{
    namespace Controller
    {
        public class FileController : MonoBehaviour
        {
            public CoroutineResultValue<T> ReadFile<T>(string filename) where T : class
            {
                CoroutineResultValue<T> result = new CoroutineResultValue<T>();
                this.ReadFileCoroutine(Path.Combine(Application.persistentDataPath, filename), result);
                return result;
            }
            public CoroutineStatus WriteFile<T>(string filename, T data) where T : class
            {
                CoroutineStatus status = CoroutineStatus.Single();
                this.SaveFileCoroutine(Path.Combine(Application.persistentDataPath, filename), data, status);
                return status;
            }

            private async void ReadFileCoroutine<T>(string filename, CoroutineResultValue<T> status) where T : class
            {
                status.Start();
                T data = await this.ReadFileAsync<T>(filename);
                status.Finish(data);
            }

            private async void SaveFileCoroutine<T>(string filename, T data, CoroutineStatus status) where T : class
            {
                status.Start();
                await this.WriteFileAsync(filename, data);
                status.Finish();
            }

            private async Task<T> ReadFileAsync<T>(string filename) where T : class
            {
                if (File.Exists(filename))
                {
                    string[] lines = await File.ReadAllLinesAsync(filename);
                    if (null == lines || lines.Length == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return JsonUtility.FromJson<T>(string.Join('\n', lines));
                    }
                }
                else
                {
                    return null;
                }
            }

            private async Task WriteFileAsync<T>(string filename, T data) where T : class
            {
                await File.WriteAllTextAsync(
                    filename,
                    JsonUtility.ToJson(data)
                );
            }
        }
    }
}

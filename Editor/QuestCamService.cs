
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class QuestCamService
{
    public delegate void StatusResponse(bool success, string text);

    public delegate void HttpJsonResponse<in T>(bool success, T obj);
    
    public static bool CheckLogin()
    {
        return false;
    }

    public static void Login(string username, string password, bool rememberMe, StatusResponse response)
    {
        
    }

    public static void Register(string username, string email, string password, string rePassword, StatusResponse response)
    {
        
    }

    private static async void DoJsonHttpRequest<TInput, TOutput>(string url, string method, TInput obj, HttpJsonResponse<TOutput> response)
    {
        using var www = new UnityWebRequest(url, method);
        www.SetRequestHeader("Content-Type", "application/json");
        
        byte[] rawData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(obj));
        
        www.uploadHandler = new UploadHandlerRaw(rawData);
        www.downloadHandler = new DownloadHandlerBuffer();

        var op = www.SendWebRequest();

        while (!op.isDone)
            await Task.Yield();

        if (www.result == UnityWebRequest.Result.Success)
        {
            response(true, JsonUtility.FromJson<TOutput>(www.downloadHandler.text));
        }
        else
        {
            response(false, default(TOutput));
        }
    }

}
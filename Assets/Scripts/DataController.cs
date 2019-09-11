using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DataController : MonoBehaviour
{
    public static FlappyCandleData gameData;
    public static MainSceneData mainData;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        LoadGameData();
        LoadURLData();
        SceneManager.LoadScene(1);
    }

    public void LoadGameData()
    {
        string filePath = Application.streamingAssetsPath + "/data.json";

        string dataAsJson = "";

        if (Application.platform == RuntimePlatform.Android)
        {
            UnityWebRequest www = UnityWebRequest.Get(filePath);
            www.SendWebRequest();
            while (!www.isDone)
            {
            }
            if (string.IsNullOrEmpty(www.error))
            {
                dataAsJson = www.downloadHandler.text;
                gameData = JsonUtility.FromJson<FlappyCandleData>(dataAsJson);
            }
            else
            {
                Debug.LogError(www.error);
            }
        }
        else
        {
            if (File.Exists(filePath))
            {
                dataAsJson = File.ReadAllText(filePath);
                gameData = JsonUtility.FromJson<FlappyCandleData>(dataAsJson);
            }
            else
                Debug.Log("No such aaaa file!");
        }
    }

    public void LoadURLData()
    {
        string filePath = Application.streamingAssetsPath + "/mainSceneData.json";

        string dataAsJson = "";

        if (Application.platform == RuntimePlatform.Android)
        {
            UnityWebRequest www = UnityWebRequest.Get(filePath);
            www.SendWebRequest();
            while (!www.isDone)
            {
            }
            if (string.IsNullOrEmpty(www.error))
            {
                dataAsJson = www.downloadHandler.text;
                mainData = JsonUtility.FromJson<MainSceneData>(dataAsJson);
            }
            else
            {
                Debug.LogError(www.error);
            }
        }
        else
        {
            if (File.Exists(filePath))
            {
                dataAsJson = File.ReadAllText(filePath);
                mainData = JsonUtility.FromJson<MainSceneData>(dataAsJson);
            }
            else
                Debug.Log("No such aaaa file!");
        }
    }
}

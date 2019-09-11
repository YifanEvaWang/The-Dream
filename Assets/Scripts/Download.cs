using System;
using System.Net;
using System.IO;

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Download : MonoBehaviour
{
    public GameObject content;
    public GameObject IconContent;
    public GameObject buttonPrefab;
    private WebRequest request;
    private WebResponse response;
    private Regex regex;
    private int forwardCounter = 0;
    private int backwardCounter = 0;
    private string uri;
    private MatchCollection matches;
    private Button[] buttons;

    private string[] imgItems;
    WWW wwwImg;
    // Start is called before the first frame update
    void Start()
    {
        uri = "http://18.191.23.16/imageServer/uploadImages/";
        buttons = IconContent.GetComponentsInChildren<Button>();
        Time.timeScale = 1;
        //load1();
        StartCoroutine(getName());
    }

    public void buttonNext()
    {
        loadNextDAB(true);
    }

    public void buttonBack()
    {
        loadNextDAB(false);
    }

    IEnumerator getName()
    {
        WWWForm ImgForm = new WWWForm();
        //change this to your url
        wwwImg = new WWW("http://18.191.23.16/imageServer/itemsData.php");
        yield return wwwImg;
        string allString = (wwwImg.text);
        Debug.Log(allString);
        //seperate each tuples
        imgItems = allString.Split(';');
        string filename;
        for (int i = 0; i < imgItems.Length - 1; i++)
        {
            filename = GetDataValue(imgItems[i], "FileName:");
            imgItems[i] = filename + ".png";
        }
        loadNextDAB(true);
    }

    string GetDataValue(string data, string index)
    {
        Debug.Log("data: " + data + "index" + index);
        string value = data.Substring(data.IndexOf(index) + index.Length);
        if (value.Contains("|"))
        {
            value = value.Remove(value.IndexOf("|"));
        }
        return value;
    }

    // public void load1()
    // {
    //     uri = "http://18.191.23.16/imageServer/UnityUpload.php";
    //     request = WebRequest.Create(uri);
    //     response = request.GetResponse();
    //     regex = new Regex("<a href=\".*png\">(?<name>.*png)</a>");
    //     using (var reader = new StreamReader(response.GetResponseStream()))
    //     {
    //         string result = reader.ReadToEnd();
    //         matches = regex.Matches(result);
    //         if (matches.Count == 0)
    //         {
    //             Debug.Log("parse failed.");
    //             return;
    //         }
    //     }
    // }

    public void loadNext(bool next)
    {
        using (var reader = new StreamReader(response.GetResponseStream()))
        {
            if (next && forwardCounter<matches.Count)
            {
                int max = forwardCounter+10;
                if (forwardCounter + 10 > matches.Count)
                {
                    max = matches.Count;
                }
                backwardCounter = forwardCounter - 1;
                for (int i = forwardCounter; i < max; i++)
                {
                    if (!matches[i].Success) { continue; }
                    StartCoroutine(loadImage(i%10,uri + matches[i].Groups["name"]));
                }
                forwardCounter = max;
            } else if(!next && backwardCounter >0)
            {
                int min = backwardCounter - 10;
                if (backwardCounter - 10 < 0)
                {
                    min = 0;
                }
                forwardCounter = backwardCounter + 1;
                for (int i = backwardCounter; i >= min; i--)
                {
                    if (!matches[i].Success) { continue; }
                    StartCoroutine(loadImage(i%10,uri + matches[i].Groups["name"]));
                }
                backwardCounter = min;
            }
        }
    }

    public void loadNextDAB(bool next)
    {            
        if (next && forwardCounter<imgItems.Length)
        {
            int max = forwardCounter+10;
            if (forwardCounter + 10 > imgItems.Length)
            {
                max = imgItems.Length;
            }
            backwardCounter = forwardCounter - 1;
            for (int i = forwardCounter; i < max; i++)
            {
                StartCoroutine(loadImage(i%10,uri + imgItems[i]));
            }
            forwardCounter = max;
        } else if(!next && backwardCounter >0)
        {
            int min = backwardCounter - 10;
            if (backwardCounter - 10 < 0)
            {
                min = 0;
            }
            forwardCounter = backwardCounter + 1;
            for (int i = backwardCounter; i >= min; i--)
            {
                StartCoroutine(loadImage(i%10,uri + imgItems[i]));
            }
            backwardCounter = min;
        }
    }

    IEnumerator loadImage(int buttonNum,string url)
    {
        Debug.Log(url);
        UnityWebRequest wr = new UnityWebRequest(url);
        DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
        wr.downloadHandler = texDl;
        yield return wr.SendWebRequest();
        if (wr.isNetworkError || wr.isHttpError)
        {
            Debug.Log(wr.error);
        }
        else
        {
            Texture2D t = texDl.texture;
            buttons[buttonNum].GetComponent<Image>().sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero, 1f);
            buttons[buttonNum].GetComponent<DragAndDrop>().url = url;
        }
    }

    /*
    public void load()
    {
        string uri = "http://localhost:8000/";
        WebRequest request = WebRequest.Create(uri);
        WebResponse response = request.GetResponse();
        Regex regex = new Regex("<a href=\".*png\">(?<name>.*png)</a>");
        using (var reader = new StreamReader(response.GetResponseStream()))
        {
            string result = reader.ReadToEnd();
            Debug.Log(result);
            MatchCollection matches = regex.Matches(result);
            if (matches.Count == 0)
            {
                Debug.Log("parse failed.");
                return;
            }

            foreach (Match match in matches)
            {
                if (!match.Success) { continue; }
                Debug.Log(match);
                Debug.Log(match.Groups["name"]);
                download(uri + match.Groups["name"]);
            }
        }
    }
    public void download(string url)
    {
        WebClient client = new WebClient();
        string fileName = Path.GetFileName(url);
        client.DownloadFile(url, Application.dataPath + "/Sprites/" + fileName);
    }

    void loadAllImage()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath+"/Sprites");
        FileInfo[] info = dir.GetFiles("*.png");
        foreach (FileInfo f in info)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(Application.dataPath + "/Sprites/" + f.Name);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            GameObject button = GameObject.Instantiate(buttonPrefab);
            button.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero, 1f);
            button.transform.SetParent(IconContent.transform);
        }
    }*/
}

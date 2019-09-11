using System.Collections;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadImages : MonoBehaviour
{
    public GameObject slots;
    Image[] images;
    private int forwardCounter = 0;
    private int backwardCounter = 0;
    private string[] imgItems;
    // Start is called before the first frame update
    void Start()
    {
        //get images
        images = slots.GetComponentsInChildren<Image>();
        StartCoroutine(getName());
        //for slides
    }

    public void buttonNext()
    {
        loadNextDAB(true);
    }

    public void buttonBack()
    {
        loadNextDAB(false);
    }

    DBScene LoadDBScene(string url)
    {
        Debug.Log(url);
        string dataAsJson = "";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SendWebRequest();
        while (!www.isDone) { }
        if (string.IsNullOrEmpty(www.error))
        {
            dataAsJson = www.downloadHandler.text;
            return JsonUtility.FromJson<DBScene>(dataAsJson);
        } else{
            Debug.Log(www.error);
        }
        return null;
    }

    IEnumerator getName()
    {
        WWWForm ImgForm = new WWWForm();
        //change this to your url
        WWW wwwImg = new WWW("http://18.191.23.16/SceneJsonServer/itemsData.php");
        yield return wwwImg;
        string allString = (wwwImg.text);
        Debug.Log(allString);
        //seperate each tuples
        imgItems = allString.Split(';');
        string filename;
        for (int i = 0; i < imgItems.Length - 1; i++)
        {
            filename = GetDataValue(imgItems[i], "FileName:");
            imgItems[i] = filename + ".json";
        }
        Array.Resize(ref imgItems, imgItems.Length-1);
        loadNextDAB(true);
    }

    string GetDataValue(string data, string index)
    {
        string value = data.Substring(data.IndexOf(index) + index.Length);
        if (value.Contains("|"))
        {
            value = value.Remove(value.IndexOf("|"));
        }
        return value;
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
                StartCoroutine(loadImage(i%10, imgItems[i]));
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
                StartCoroutine(loadImage(i%10, imgItems[i]));
            }
            backwardCounter = min;
        }
    }
    IEnumerator loadImage(int imageNum, string name)
    {
        Debug.Log(name);
        DBScene currScene = LoadDBScene("http://18.191.23.16/SceneJsonServer/files/"+name);
        string BackgroundPath = currScene.mainPngAddr;
        UnityWebRequest wr = new UnityWebRequest(BackgroundPath);
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
            Sprite sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero, 1f);
            images[imageNum].sprite = sprite;
        }
        images[imageNum].gameObject.GetComponent<DragDropSc2>().JsonInfo = currScene;
    }

}
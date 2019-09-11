using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class loadPicturesFromWeb: MonoBehaviour
{
    public GameObject slots;
    Image[] images;
    string folderForImages;
    private int forwardCounter = 0;
    private int backwardCounter = 0;
    FileInfo[] info;

    string uri;
    private string[] imgItems;
    WWW wwwImg;
    // Start is called before the first frame update
    void Start()
    {
        uri = "http://18.191.23.16/imageServer/uploadImages/";
        //get images
        images = slots.GetComponentsInChildren<Image>();
        folderForImages = Application.dataPath + "/SavedPics/";
        DirectoryInfo dir = new DirectoryInfo(folderForImages);
        info = dir.GetFiles("*.png");
        //load from web
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
        imgItems[imgItems.Length-1] = "\0";
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
            Sprite sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero, 1f);
            images[buttonNum].sprite = sprite;
        }
    }
}

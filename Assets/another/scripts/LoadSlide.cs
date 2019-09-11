using System.Collections;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class LoadSlide : MonoBehaviour
{
    private int forwardCounter = 0;
    private int backwardCounter = 0;
    FileInfo[] infoSlides;
    public GameObject slideOptions;
    // Start is called before the first frame update
    void Start()
    {
        //get images
        loadNextDAB(true);
    }

    public void buttonNext()
    {
        loadNextDAB(true);
    }

    public void buttonBack()
    {
        loadNextDAB(false);
    }

    void loadImages(int imageNum, string name)
    {        
        slideOptions.transform.GetChild(imageNum).GetComponent<Slide>().slideJson = LoadslideShow("http://18.191.23.16/StoryJsonServer/files/"+name);
        Debug.Log(LoadslideShow("http://18.191.23.16/StoryJsonServer/files/"+name).name);
        slideOptions.transform.GetChild(imageNum).GetChild(0).GetComponent<Text>().text = LoadslideShow("http://18.191.23.16/StoryJsonServer/files/"+name).name;
    }

    slideShow LoadslideShow(string url)
    {
        Debug.Log(url);
        string dataAsJson = "";
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SendWebRequest();
        while (!www.isDone) { }
        if (string.IsNullOrEmpty(www.error))
        {
            dataAsJson = www.downloadHandler.text;
            return JsonUtility.FromJson<slideShow>(dataAsJson);
        } else{
            Debug.Log(www.error);
        }
        return null;
    }
    private string[] storyItems;
    IEnumerator getName()
    {
        WWWForm ImgForm = new WWWForm();
        //change this to your url
        WWW wwwImg = new WWW("http://18.191.23.16/StoryJsonServer/itemsData.php");
        yield return wwwImg;
        string allString = (wwwImg.text);
        Debug.Log(allString);
        //seperate each tuples
        storyItems = allString.Split(';');
        string filename;
        for (int i = 0; i < storyItems.Length - 1; i++)
        {
            filename = GetDataValue(storyItems[i], "FileName:");
            storyItems[i] = filename + ".json";
        }
        Array.Resize(ref storyItems, storyItems.Length-1);
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
        if (next && forwardCounter<storyItems.Length)
        {
            int max = forwardCounter+10;
            if (forwardCounter + 10 > storyItems.Length)
            {
                max = storyItems.Length;
            }
            backwardCounter = forwardCounter - 1;
            for (int i = forwardCounter; i < max; i++)
            {
                loadImages(i%10, storyItems[i]);
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
                loadImages(i%10, storyItems[i]);
            }
            backwardCounter = min;
        }
    }
}

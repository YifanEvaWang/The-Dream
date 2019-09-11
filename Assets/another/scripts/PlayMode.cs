using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayMode : MonoBehaviour
{
    public GameObject panel,iconOptions;
    public Text text;
    public GameObject optionContent;
    public GameObject nodes,imgs;
    public GameObject answerButtonPrefab;
    public slideShow slideJson;

    private void OnEnable()
    {
        if (nodes.transform.childCount == 1 && nodes.transform.GetChild(0).tag.Equals("slide"))
        {
            slideJson = nodes.transform.GetChild(0).GetComponent<Slide>().slideJson;
            setJsonImage(slideJson.Firstslide);
        } else
        {
            setImage(nodes.transform.GetChild(0).gameObject);
        }
    }


    //this is where we apply the questions with display window
    void setImage(GameObject node)
    {
        //set sprite
        panel.GetComponent<Image>().sprite = node.GetComponent<Image>().sprite;
        //set the text
        text.text = node.GetComponent<Node>().getText();
        //set new imgs
        if (currImg.Count != 0)
        {
            foreach (GameObject child in currImg)
            {
                GameObject.Destroy(child);
            }
            currImg.Clear();
        }
        setAnim(node.GetComponent<Node>().JsonInfo);
        display(node.GetComponent<Node>().JsonInfo);
        //get the answerlist
        foreach (Transform child in optionContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        if (node.GetComponent<Node>().getNodeAnswer() != null)
        {
            //for each answer, make an option for it
            foreach(Node n in node.GetComponent<Node>().getNodeAnswer().Keys)
            {
                GameObject button = Instantiate(answerButtonPrefab);
                button.transform.SetParent(optionContent.transform);
                button.GetComponentInChildren<Text>().text = node.GetComponent<Node>().getNodeAnswer()[n];
                button.GetComponent<Button>().onClick.AddListener(delegate { setImage(n.gameObject); });
            }
        } else
        {
            GameObject button = Instantiate(answerButtonPrefab);
            button.transform.SetParent(optionContent.transform);
            button.GetComponentInChildren<Text>().text = "next";
        }
    }

    void setJsonImage(DBScene firstScene)
    {
        //set sprite
        StartCoroutine(LoadPNG(panel,firstScene.mainPngAddr));
        //set the text
        text.text = firstScene.description;
        //set new imgs
        if (currImg.Count != 0)
        {
            foreach (GameObject child in currImg)
            {
                GameObject.Destroy(child);
            }
            currImg.Clear();
        }
        setAnim(firstScene);
        display(firstScene);
        //get the answerlist
        foreach (Transform child in optionContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        if (firstScene.next != null)
        {
            //for each answer, make an option for it
            foreach (SceneAnswerPair pair in firstScene.next)
            {
                GameObject button = Instantiate(answerButtonPrefab);
                button.transform.SetParent(optionContent.transform);
                button.GetComponentInChildren<Text>().text = pair.answer;
                button.GetComponent<Button>().onClick.AddListener(delegate { setJsonImage(slideJson.sceneSet[pair.nextSceneID]); });
            }
        }
        else
        {
            GameObject button = Instantiate(answerButtonPrefab);
            button.transform.SetParent(optionContent.transform);
            button.GetComponentInChildren<Text>().text = "next";
        }
    }

    FileInfo[] info;
    line[] lines;
    //set the animation of scene
    void setAnim(DBScene JsonInfo)
    {
        for(int i=0; i<JsonInfo.pngAddress.Length; i++)
        {
            GameObject image = new GameObject();
            image.AddComponent<Image>();
            StartCoroutine(LoadPNG(image,JsonInfo.pngAddress[i]));
            image.transform.SetParent(imgs.transform);
            lines = JsonInfo.lines;
            image.tag = "img";
            currImg.Add(image);
        }
    }

    List<GameObject> currImg = new List<GameObject>();
    float refTime;
    public void display(DBScene JsonInfo)
    {
        StartCoroutine(displayHelper(JsonInfo));
    }
    IEnumerator displayHelper(DBScene JsonInfo)
    {
        int counter = 0;
        foreach (GameObject obj in currImg)
        {
            if (obj.tag.Equals("img"))
            {
                //take reference time
                refTime = Time.realtimeSinceStartup;
                for (int i = 0; i < JsonInfo.lines[counter].points.Length; i++)
                {
                    float timeElapsed = Time.realtimeSinceStartup - refTime;
                    if (JsonInfo.lines[counter].points[i].timeStamp < timeElapsed)
                    {
                        move(obj, new Vector2(JsonInfo.lines[counter].points[i].x, JsonInfo.lines[counter].points[i].y));
                        yield return null;
                    }
                    else
                    {
                        i--;
                    }
                }
            }
            counter++;
        }
    }
    //move point to next place
    void move(GameObject obj, Vector2 position)
    {
        // Get the distance from start to finish
        float distance = Vector2.Distance(obj.transform.position, position);
        Vector2 direction = ((Vector2)obj.transform.position - position).normalized;
        Vector2 cur_position = obj.transform.position;

        // Calculate how many times we should interpolate between start_point and end_point based on the amount of time that has passed since the last update
        float lerp_steps = 1 / distance;

        for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
        {
            cur_position = Vector2.Lerp(obj.transform.position, position, lerp);
            obj.transform.position = cur_position;
        }
    }

    //load png as texture given path
    IEnumerator LoadPNG(GameObject obj ,string filePath)
    {
        Debug.Log(filePath);
        UnityWebRequest wr = new UnityWebRequest(filePath);
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
            obj.GetComponent<Image>().sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero, 1f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;

public class Controller02 : MonoBehaviour
{
    public GameObject canvas;
    public GameObject iconOptions;
    public GameObject nodes;
    public GameObject arrows;
    public slideShow slides;
    public GameObject SaveName;

    private void Awake()
    {
        canvas.SetActive(true);
        if (GameObject.FindGameObjectWithTag("currEdit") != null)
        {
            GameObject.FindGameObjectWithTag("currEdit").tag = "Untagged";
        }
    }
    void disableEverything()
    {
        canvas.GetComponent<ArrowModeController>().enabled = false;
        DragDropSc2[] nodeList = nodes.GetComponentsInChildren<DragDropSc2>();
        iconOptions.SetActive(false);
        foreach (DragDropSc2 i in nodeList)
        {
            i.enabled = false;
        }
        canvas.GetComponent<Question>().enabled = false;
    }

    public void onClickClear()
    {
        disableEverything();
        foreach(Transform child in nodes.transform)
        {
            Destroy(child.gameObject);
        }
        foreach(Transform child in arrows.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void onClickSave()
    {
        disableEverything();
        SaveName.SetActive(true);
    }

    public void OnClickConfirm()
    {
         
        slides = new slideShow();
        
        //goal is to fill slideshow
        //1.fill SceneAnswerPair
        //1. fill dbScene objects
        foreach (Node node in nodes.GetComponentsInChildren<Node>())
        {
            node.fillJson();
        }
        //2.fill slideShow
        List<DBScene> slideSet = new List<DBScene>();
        foreach (Transform child in nodes.transform)
        {
            slideSet.Add(child.GetComponent<Node>().JsonInfo);
        }
        slides.sceneSet = slideSet.ToArray();
        slides.Firstslide = slideSet[0];
        slides.name = SaveName.transform.GetChild(0).GetComponent<InputField>().text;
        //3.convertTojson
        //write to json file
        if(insertNewJson(slides.name)){
            uploadDBJson(slides, slides.name);
        }
        //4.clean and disable
        SaveName.transform.GetChild(0).GetComponent<InputField>().text = null;
        SaveName.gameObject.SetActive(false);
    }
    public void uploadDBJson(slideShow db, string name){
        //upload json
        string contents = "";
        string urlJson = "http://18.191.23.16/StoryJsonServer/UnityUpload.php";
        contents = JsonUtility.ToJson(db);
        byte[] bytes = Encoding.ASCII.GetBytes(contents);
        WWWForm form = new WWWForm();
        form.AddField("Name", name);
        form.AddBinaryData("post", bytes);
        WWW www = new WWW(urlJson, form);
    }
    public bool insertNewJson(string filename)
    {
        WWWForm form = new WWWForm();
        form.AddField("fileNamePost", filename);
        WWW www = new WWW("http://18.191.23.16/StoryJsonServer/InsertData.php", form);
        if (www.error != null)
        {
            Debug.Log(www.error);
            return false;
        } else {
            while (!www.isDone){}
            if (www.text != "exist")
            {
                Debug.Log("can upload it now");
                return true;
            }
            else
            {
                Debug.Log("rename it" + www.text);
                return false;
            }
        }
    }

    public void onClickEdit()
    {
        disableEverything();
        DragDropSc2[] nodeList = nodes.GetComponentsInChildren<DragDropSc2>();
        if (iconOptions.activeSelf)
        {
            iconOptions.SetActive(false);
            foreach(DragDropSc2 i in nodeList)
            {
                i.enabled = false;
            }
        } else
        {
            iconOptions.SetActive(true);
            foreach (DragDropSc2 i in nodeList)
            {
                i.enabled = true;
            }
        }
    }

    public void onClickArrow()
    {
        disableEverything();
        if (canvas.GetComponent<ArrowModeController>().enabled)
        {
            canvas.GetComponent<ArrowModeController>().enabled = false;
        } else
        {
            canvas.GetComponent<ArrowModeController>().enabled = true;
        }
    }

    public void onClickQuestion()
    {
        disableEverything();
        if (canvas.GetComponent<Question>().enabled)
        {
            canvas.GetComponent<Question>().enabled = false;
        } else
        {
            canvas.GetComponent<Question>().enabled = true;
        }
    }

    //when clicking draw button, add button listener on everynode to allow it to open drawing board.
    public void onClickDrawButton()
    {
        disableEverything();
        SceneManager.LoadScene(4);
    }

    public GameObject SlideSelect;
    public void OnclickSlideButton()
    {
        disableEverything();
        DragDropSc2[] nodeList = nodes.GetComponentsInChildren<DragDropSc2>();
        if (SlideSelect.activeSelf)
        {
            SlideSelect.SetActive(false);
            foreach (DragDropSc2 i in nodeList)
            {
                i.enabled = false;
            }
        }
        else
        {
            SlideSelect.SetActive(true);
            foreach (DragDropSc2 i in nodeList)
            {
                i.enabled = true;
            }
        }
    }

    void editDrawing(GameObject node)
    {
        DontDestroyOnLoad(canvas);
        node.tag = "currEdit";
        canvas.tag = "outerSpace";
        SceneManager.LoadScene("DrawingBoard");
    }

    public GameObject panel, questionPanel;
    public void onClickPlay()
    {
        canvas.GetComponent<PlayMode>().enabled = true;
        panel.SetActive(true);
        for (int i = 0; i < panel.transform.parent.childCount; i++)
        {
            if (i != panel.transform.GetSiblingIndex())
            {
                panel.transform.parent.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public GameObject chooseSlideOption;
    public void onClickExit()
    {
        canvas.GetComponent<PlayMode>().enabled = false;
        panel.SetActive(false);
        for (int i = 0; i < panel.transform.parent.childCount; i++)
        {
            if (i != panel.transform.GetSiblingIndex())
            {
                panel.transform.parent.GetChild(i).gameObject.SetActive(true);
            }
        }
        iconOptions.SetActive(false);
        questionPanel.SetActive(false);
        SaveName.SetActive(false);
        chooseSlideOption.SetActive(false);
    }
}

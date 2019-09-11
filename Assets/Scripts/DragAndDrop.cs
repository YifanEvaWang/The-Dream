using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string url;
    private MainController main;
    private Transform newParent;
    private Transform oldParent;
    private List<RaycastResult> list;
    MainSceneData mainData;
    void Start()
    {
        list = new List<RaycastResult>();
        oldParent = this.transform.parent;
        newParent = GameObject.Find("Canvas").transform;
        mainData = new MainSceneData
        {
            URL = DataController.mainData.URL
        };
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        this.transform.SetParent(newParent);
        this.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(oldParent);
        transform.localPosition = Vector3.zero;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        EventSystem.current.RaycastAll(eventData, list);
        foreach(RaycastResult r in list)
        {
            if (r.gameObject.tag.Equals("buttonIcon"))
            {
                r.gameObject.GetComponent<Image>().sprite = this.GetComponent<Image>().sprite;
                mainData.URL[r.gameObject.transform.GetSiblingIndex()] = url;
            }
        }
        //serialize json to file
        File.WriteAllText(Application.streamingAssetsPath + "/mainSceneData.json", JsonConvert.SerializeObject(mainData));
        using (StreamWriter file = File.CreateText(Application.streamingAssetsPath + "/mainSceneData.json")) {
        JsonSerializer serializer = new JsonSerializer();
        serializer.Serialize(file, mainData);
        }
    }
}

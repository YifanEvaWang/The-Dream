using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//DragDrop and store json information
public class DragDropSc2 : MonoBehaviour,  IDragHandler, IEndDragHandler
{
    public DBScene JsonInfo;
    private Transform newParent;
    private Transform oldParent;
  
    void Start()
    {
        oldParent = this.transform.parent;
        newParent = GameObject.Find("Canvas").transform;
    }

    //ondrag moving with cursor
    public void OnDrag(PointerEventData eventData)
    {
        if (this.transform.parent.name.Equals("IconOptions"))
        {
            this.transform.SetParent(newParent);
        }
        this.transform.position = Input.mousePosition;
    }

    //end of drag, duplicate node
    public void OnEndDrag(PointerEventData eventData)
    {
        if (this.transform.parent.name.Equals("Canvas"))
        {
            GameObject node = GameObject.Instantiate(this.gameObject);
            this.transform.SetParent(oldParent);
            node.transform.SetParent(GameObject.Find("nodes").transform);
            node.transform.position = Input.mousePosition;
            node.tag = "node";
            node.AddComponent<Node>();
            node.GetComponent<Node>().JsonInfo = this.JsonInfo;
        } 
    }
}

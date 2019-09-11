using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDopDB : MonoBehaviour, IDragHandler, IEndDragHandler
{
    // Start is called before the first frame update
    void Start()
    {
        oldParent = this.transform.parent;
    }

    void Update()
    {
        if(GameObject.Find("layers").transform.childCount!=0)
        {
            newParent = GameObject.Find("layers").transform.GetChild(GameObject.Find("layers").transform.childCount-1);
        }
    }

    //dragdrop function
    private Transform newParent;
    private Transform oldParent;

    public void OnDrag(PointerEventData eventData)
    {
        if (newParent != null)
        {
            if (this.transform.parent.name.Equals("IconOptions"))
            {
                this.transform.SetParent(newParent);
            }
            this.transform.position = Input.mousePosition;
        }    
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (this.transform.parent.parent.name.Equals("layers")&&!this.tag.Equals("component"))
        {
            GameObject node = GameObject.Instantiate(this.gameObject);
            this.transform.SetParent(oldParent);
            node.transform.SetParent(newParent);
            node.transform.position = Input.mousePosition;
            node.tag = "component";
            Destroy(node.GetComponent<Button>());
        }
    }
}

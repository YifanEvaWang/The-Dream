using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArrowModeController : MonoBehaviour
{
    public GameObject dot;
    public GameObject arrows;
    public GameObject emptyObject;
    GraphicRaycaster raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    List<GameObject> objects;

    private void Awake()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        objects = new List<GameObject>();
        m_EventSystem = GetComponent<EventSystem>();
    }
    private void Update()
    {
        drawArrow();
    }

    void drawArrow()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //use raycast to get the clicked object 
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);

            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            raycaster.Raycast(m_PointerEventData, results);

            foreach(RaycastResult result in results)
            {
                if (result.gameObject.tag.Equals("node"))
                {
                    objects.Add(result.gameObject);
                }
            }
        } 

        if (objects.Count == 2)
        {
            //if there are two objects selected, get start and end position
            Vector2 start_point = objects[0].transform.position; 
            Vector2 end_point = objects[1].transform.position; 

            // Get the distance from start to finish
            float distance = Vector2.Distance(start_point, end_point); 
            Vector2 direction = (start_point - end_point).normalized; 
            Vector2 cur_position = start_point; 

            //build a folder for this arrow
            GameObject arrow1 = GameObject.Instantiate(emptyObject,arrows.transform); 
            arrow1.AddComponent<Arrow>().parentNode = objects[0]; 
            arrow1.GetComponent<Arrow>().tailNode = objects[1]; 

            // Calculate how many times we should interpolate between start_point and end_point based on the amount of time that has passed since the last update
            float lerp_steps = 60 / distance;

            //add dots
            for (float lerp = lerp_steps; lerp <= 0.9; lerp += lerp_steps)
            {
                cur_position = Vector2.Lerp(start_point, end_point, lerp);
                GameObject currDot = GameObject.Instantiate(dot,arrow1.transform);
                currDot.transform.position = cur_position;
                currDot.AddComponent<Button>();
            }

            //add the other node to this node. so that a graph can be built
            objects[0].GetComponent<Node>().addChild(objects[1].GetComponent<Node>());

            //clear list
            objects.Clear();
        }
    }
}

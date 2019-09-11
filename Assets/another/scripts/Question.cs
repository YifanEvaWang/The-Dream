using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Question : MonoBehaviour
{
    GraphicRaycaster raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    public GameObject questionPanel;
    GameObject currentNode;
    public Button confirmButton;
    public static int sceneNum;

    private void Awake()
    {
        sceneNum = SceneManager.GetActiveScene().buildIndex;
        raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
    }

    private void Update()
    {
        clickOpen();
    }

    void clickOpen()
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

            foreach (RaycastResult result in results)
            {
                //if I click a node, I add a question
                if (result.gameObject.tag.Equals("node"))
                {
                    if (questionPanel.transform.parent.gameObject.activeSelf)
                    {
                        questionPanel.transform.parent.gameObject.SetActive(false);
                    }
                    else
                    {
                        questionPanel.transform.parent.gameObject.SetActive(true);
                        currentNode = result.gameObject;
                        confirmButton.onClick.RemoveAllListeners();
                        confirmButton.onClick.AddListener(onClickConfirmQuestion);
                    }
                }
                //if I click an arrow, I add answers
                else if (result.gameObject.transform.parent.parent!=null && result.gameObject.transform.parent.parent.name.Equals("arrows"))
                {
                    if (questionPanel.transform.parent.gameObject.activeSelf)
                    {
                        questionPanel.transform.parent.gameObject.SetActive(false);
                    }
                    else
                    {
                        questionPanel.transform.parent.gameObject.SetActive(true);
                        currentNode = result.gameObject.transform.parent.GetComponent<Arrow>().parentNode;
                        confirmButton.onClick.RemoveAllListeners();
                        confirmButton.onClick.AddListener(delegate { onclickConfirmAnswer(result.gameObject.transform.parent.GetComponent<Arrow>().tailNode.GetComponent<Node>()); });
                    }
                }
            }
        }
    }

    //confirm Button behaviors
     void onClickConfirmQuestion()
    {
        currentNode.GetComponent<Node>().setText(questionPanel.GetComponent<InputField>().text);
        questionPanel.GetComponent<InputField>().text = null;
        questionPanel.transform.parent.gameObject.SetActive(false);
    }
    //confirm Button behaviors
    void onclickConfirmAnswer(Node tailNode)
    {
        currentNode.GetComponent<Node>().addAnswers(questionPanel.GetComponent<InputField>().text,tailNode);
        questionPanel.GetComponent<InputField>().text = null;
        questionPanel.transform.parent.gameObject.SetActive(false);
    }
}

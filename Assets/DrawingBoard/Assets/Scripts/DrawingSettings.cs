using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FreeDraw
{
    // Helper methods used to set drawing settings
    public class DrawingSettings : MonoBehaviour
    {
        public GameObject PaintUI;
        public Sprite[] spritePrefabs;
        public static bool isCursorOverUI = false;
        public float Transparency = 1f;
        public Slider MakerSlide;
        public GameObject RWenabledImage;
        public GameObject layerButtonPrefab;
        public static GameObject currLayer;

        public static Sprite editingImage;
        private int backNumber = 1;
        private DragDopDB[] dragDropElements;
        private Drawable drawable;

        //on start, find the node that is going to be edited, put it on canvas to edit.
        private void Start()
        {
            backNumber = 3;//Question.sceneNum;
            editingImage = null;
            if (GameObject.FindGameObjectWithTag("currEdit") != null)
            {
                editPictureFromOtherScene();
            }
        }

        void editPictureFromOtherScene()
        {
            GameObject node = GameObject.FindGameObjectWithTag("currEdit");
            editingImage = Sprite.Create(node.GetComponent<Image>().sprite.texture, new Rect(0, 0, node.GetComponent<Image>().sprite.texture.width, node.GetComponent<Image>().sprite.texture.height), new Vector2(0.5f, 0.5f), 100);
            Debug.Log(editingImage.pixelsPerUnit);
            addNewLayer();
            GameObject.FindGameObjectWithTag("outerSpace").SetActive(false);
        }

        // Changing pen settings is easy as changing the static properties Drawable.Pen_Colour and Drawable.Pen_Width
        public void SetMarkerColour(Color new_color)
        {
            Drawable.Pen_Colour = new_color;
        }
        // new_width is radius in pixels

        public void SetMarkerWidth(int new_width)
        {
            
            Debug.Log("Marker width set to" + new_width);
            Drawable.Pen_Width = new_width;
        }

        public void SetMarkerWidth(float new_width)
        {
            new_width = MakerSlide.value;
            SetMarkerWidth((int)new_width);
        }

        // Call these these to change the pen settings
        public void SetMarkerRed()
        {
            DragDropControl(false);
            if (GameObject.Find("layers").transform.childCount != 0)
            {
                drawable.enabled = true;
            }
            Color c = Color.red;
            c.a = Transparency;
            SetMarkerColour(c);
            if (GameObject.Find("layers").transform.childCount != 0)
            {
                currLayer.GetComponent<Drawable>().SetPenBrush(false);
            }
        }
        public void SetMarkerGreen()
        {
            DragDropControl(false);
            if (GameObject.Find("layers").transform.childCount != 0)
            {
                drawable.enabled = true;
            }
            Color c = Color.green;
            c.a = Transparency;
            SetMarkerColour(c);
            if (GameObject.Find("layers").transform.childCount != 0)
            {
                currLayer.GetComponent<Drawable>().SetPenBrush(false);
            }
        }
        public void SetMarkerBlue()
        {
            DragDropControl(false);
            if (GameObject.Find("layers").transform.childCount != 0)
            {
                drawable.enabled = true;
            }
            Color c = Color.blue;
            c.a = Transparency;
            SetMarkerColour(c);
            if (GameObject.Find("layers").transform.childCount != 0)
            {
                currLayer.GetComponent<Drawable>().SetPenBrush(false);
            }
        }

        public void setMarkerBlack()
        {
            DragDropControl(false);
            if (GameObject.Find("layers").transform.childCount != 0)
            {
                drawable.enabled = true;
            }
            Color c = Color.black;
            c.a = Transparency;
            SetMarkerColour(c);
            if (GameObject.Find("layers").transform.childCount != 0)
            {
                currLayer.GetComponent<Drawable>().SetPenBrush(false);
            }
        }

        public void setCursor()
        {
            DragDropControl(true);
            if (GameObject.Find("layers").transform.childCount != 0)
            {
                drawable.enabled = false;
            }
        }
        public void SetEraser()
        {
            DragDropControl(false);
            SetMarkerColour(new Color(255f, 255f, 255f, 0f));
        }

        //set Penbrush with animation
        public void animationMode()
        {
            if (GameObject.Find("layers").transform.childCount != 0)
            {
                drawable.enabled = true;
            }
            DragDropControl(false);
            Color c = Color.yellow;
            c.a = Transparency;
            SetMarkerColour(c);
            if (GameObject.Find("layers").transform.childCount != 0)
            {
                currLayer.GetComponent<Drawable>().SetPenBrush(true);
            }
        }

        public void display()
        {
            foreach(Transform layer in GameObject.Find("layers").transform)
            {
                layer.GetComponent<Drawable>().display();
            }
        }

        public void back()
        {
            SceneManager.LoadScene(backNumber);
        }

        public void DragDropControl(bool isTrue)
        {
            if(GameObject.Find("layers").transform.GetChild(GameObject.Find("layers").transform.childCount - 1).childCount != 0)
            {
                foreach(DragDopDB child in dragDropElements)
                {
                    child.enabled = isTrue;
                }
            }
            
        }

        public void Update()
        {
            if (PaintUI.activeSelf)
            {
                if (GameObject.Find("layers").transform.childCount > 0)
                {
                    currLayer = GameObject.Find("layers").transform.GetChild(GameObject.Find("layers").transform.childCount - 1).gameObject;

                }
                setActiveLayer();
                setLayerOrder();
            }
            if (GameObject.Find("layers")!=null && GameObject.Find("layers").transform.childCount != 0)
            {
                if (GameObject.Find("layers").transform.GetChild(GameObject.Find("layers").transform.childCount - 1).childCount != 0)
                {
                    dragDropElements = GameObject.Find("layers").transform.GetChild(GameObject.Find("layers").transform.childCount - 1).GetComponentsInChildren<DragDopDB>();
                }
                drawable = GameObject.Find("layers").transform.GetChild(GameObject.Find("layers").transform.childCount - 1).GetComponent<Drawable>();
            }
        }

        public void setActiveLayer()
        {
            if (GameObject.Find("layers").transform.childCount > 1)
            {
                foreach (Drawable child in GameObject.Find("layers").transform.GetComponentsInChildren<Drawable>())
                {
                    if (!child.Equals(currLayer.GetComponent<Drawable>()))
                    {
                        child.GetComponent<Drawable>().enabled = false;
                    } else
                    {
                        child.GetComponent<Drawable>().enabled = true;
                    }
                }
            }
        }

        public void setLayerOrder()
        {
            if (GameObject.Find("layers").transform.childCount > 1)
            {
                foreach (SpriteRenderer child in GameObject.Find("layers").transform.GetComponentsInChildren<SpriteRenderer>())
                {
                    child.sortingOrder = child.transform.GetSiblingIndex();
                }
            }

        }

        public void addNewLayer()
        {
            GameObject newLayer = Instantiate(RWenabledImage, GameObject.Find("layers").transform);
            newLayer.transform.localPosition = new Vector3(-720, -1280,0);
            currLayer = newLayer;
            GameObject layerButton = GameObject.Instantiate(layerButtonPrefab);
            currLayer.GetComponent<Drawable>().layerButton = layerButton;
            layerButton.transform.SetParent(GameObject.Find("Content").transform);
            layerButton.GetComponentInChildren<Text>().text = currLayer.transform.GetSiblingIndex() + "";
            layerButton.GetComponent<LayerDraggable>().correspondingLayer = currLayer;
            currLayer.GetComponent<Drawable>().layerButton = layerButton;
        }
    }
}
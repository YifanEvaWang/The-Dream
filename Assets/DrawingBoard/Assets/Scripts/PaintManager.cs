using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;

namespace FreeDraw
{
    public class PaintManager : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject MenuPanel;
        public GameObject PaintBoard;
        public GameObject Gallery;
        public InputField inFiled;
        public static string paintName;
        public static bool isNamed = false;
        string url = "http://18.191.23.16/imageServer/UnityUpload.php";
        //string url = "http://18.191.23.16/jsonServer/jsonUploader.php";
        string insertURL = "http://18.191.23.16/imageServer/InsertData.php";



        public void onMenuButton()
        {
            MenuPanel.SetActive(true);
            PaintBoard.GetComponent<Canvas>().enabled = false;
        }

        public void onButtonUpload()
        {
            GameObject.Find("layers").transform.GetChild(0).GetComponent<Drawable>().uploadHelper(); 
        }

        public void onButtonClear()
        {
            //clear current panel
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
            MenuPanel.SetActive(false);
            PaintBoard.SetActive(true);
            Time.timeScale = 1;
        }

        public void onButtonExit()
        {
            Application.Quit();
        }

        public void onButtonCancel()
        {
            MenuPanel.SetActive(false);
            PaintBoard.SetActive(true);
            PaintBoard.GetComponent<Canvas>().enabled = true;
        }

        public void onButtonRedraw()
        {
            DrawingSettings.currLayer.GetComponent<Drawable>().rePaint();
        }

        public void onButtonWithdraw()
        {
            DrawingSettings.currLayer.GetComponent<Drawable>().withDraw();
        }

        public void save()
        {
            DrawingSettings.currLayer.GetComponent<Drawable>().save();
        }

        public void getName()
        {
            paintName = inFiled.text;
            isNamed = true;
        }

        public void ViewGallery()
        {
            Gallery.SetActive(true);
            MenuPanel.SetActive(false);
        }

        public void goBackToMenu()
        {
            Gallery.SetActive(false);
            MenuPanel.SetActive(true);
        }
    }
}

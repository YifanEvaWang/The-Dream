using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.IO;

public class MainController : MonoBehaviour
{
    public GameObject iconOptions;
    public GameObject contentOfScrollView;
    private Button[] buttons;
    int levelPassed;

    private void Start()
    {
        Time.timeScale = 1;
        levelSelect();
        StartCoroutine(fillButton());
    }

    IEnumerator fillButton()
    {
        for(int i=0;i<buttons.Length;i++)
        {
            UnityWebRequest wr = new UnityWebRequest(DataController.mainData.URL[i]);
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
                buttons[i].GetComponent<Image>().sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero, 1f);
            }
        }
    }

    void levelSelect()
    {
        levelPassed = PlayerPrefs.GetInt("levelPassed");
        buttons = contentOfScrollView.GetComponentsInChildren<Button>();
        buttons[0].interactable = true;
        buttons[0].onClick.AddListener(delegate { ButtonLoad(0); });
        for (int i = 0; i < buttons.Length; i++)
        {

            if (i <= levelPassed)
            {
                int temp = i;
                buttons[i].interactable = true;
                buttons[i].onClick.AddListener(delegate { ButtonLoad(temp); });
            }
            else
            {
                buttons[i].interactable = false;
            }
        }
    }

    void ButtonLoad(int i)
    {
        SceneManager.LoadScene(i+2);
    }

    public void resetPlayerPrefs()
    {
        for (int i = 1; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        PlayerPrefs.SetInt("levelPassed", 1);
    }

    public void editSprite()
    {
        if (iconOptions.activeSelf)
        {
            iconOptions.SetActive(false);

        } else
        {
            iconOptions.SetActive(true);
        }
    }

    public void openDrawingBoard()
    {
        SceneManager.LoadScene(4);
    }
}

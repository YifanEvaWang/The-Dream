using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PopulateGrid : MonoBehaviour
{
    public GameObject prefab; // This is our prefab object that will be exposed in the inspector
    public int numberToCreate; // number of objects to create. Exposed in inspector
    List<Texture2D> allTex2d = new List<Texture2D>();

    void Start()
    {
        load();
        Populate();
    }

    void Populate()
    {
        GameObject newObj; // Create GameObject instance

        for (int i = 0; i < allTex2d.Count; i++)
        {
            newObj = (GameObject)Instantiate(prefab, transform);
            //newObj.GetComponent<Transform>().SetParent(prefab.transform.parent);
            Sprite sprite = Sprite.Create(allTex2d[i], new Rect(0, 0, allTex2d[i].width, allTex2d[i].height), new Vector2(1f, 1f));
            newObj.GetComponent<Image>().sprite = sprite;
            newObj.transform.name = "Image" + i;
        }

    }

    void load()
    {
        List<string> filePaths = new List<string>();
        string imgtype = "*.BMP|*.JPG|*.GIF|*.PNG";
        string[] ImageType = imgtype.Split('|');
        for (int i = 0; i < ImageType.Length; i++)
        {
            string[] dirs = Directory.GetFiles((Application.dataPath + "/SavedPics"), ImageType[i]);
            for (int j = 0; j < dirs.Length; j++)
            {
                filePaths.Add(dirs[j]);
            }
        }

        for (int i = 0; i < filePaths.Count; i++)
        {
            Texture2D tx = new Texture2D(100, 100);
            tx.LoadImage(getImageByte(filePaths[i]));
            allTex2d.Add(tx);
        }
    }

    private static byte[] getImageByte(string imagePath)
    {
        FileStream files = new FileStream(imagePath, FileMode.Open);
        byte[] imgByte = new byte[files.Length];
        files.Read(imgByte, 0, imgByte.Length);
        files.Close();
        return imgByte;
    }
}

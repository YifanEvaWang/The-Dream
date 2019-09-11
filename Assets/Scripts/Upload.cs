// Saves screenshot as PNG file.
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;

public class Upload : MonoBehaviour
{
    void Start()
    {

       StartCoroutine(UploadPNG());
    }

    /*async System.Threading.Tasks.Task uploadfileAsync()
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("PUT"), "http://localhost:8000/test.txt"))
            {
                request.Content = new ByteArrayContent(File.ReadAllBytes("test.txt"));

                var response = await httpClient.SendAsync(request);
            }
        }
    } */

    IEnumerator UploadPNG()
    {
        DirectoryInfo dir = new DirectoryInfo(@"C:\Users\ywang8\Desktop\test2\");
        FileInfo[] info = dir.GetFiles("*.png");
        foreach (FileInfo f in info)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(@"C:\Users\ywang8\Desktop\test2\" + f.Name);
   
            // Upload to a cgi script
            var w = UnityWebRequest.Put("18.191.23.16/ContributeServer/Pics", bytes);
            yield return w.SendWebRequest();

            if (w.isNetworkError || w.isHttpError)
            {
                Debug.Log(w.error);
            }
            else
            {
                Debug.Log("Finished Uploading image");
            }
        }
    }
}
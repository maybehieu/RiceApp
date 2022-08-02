using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NativeCameraController : MonoBehaviour
{
    [SerializeField] Text textUIValue;
    [SerializeField] GameObject snapButton;
    [SerializeField] GameObject statusDisplay;
    [SerializeField] GameObject imgPath;
    [SerializeField] GameObject displayer;
    [SerializeField] GameObject pathDisplay;
    void Start()
    {
        snapButton.GetComponent<Button>().onClick.AddListener(TakePictureOnClick);
    }
    void Update()
    {
        if (NativeCamera.IsCameraBusy())
        {
            statusDisplay.GetComponent<Text>().text = "Busy taking picture...";
            return;
        }
        statusDisplay.GetComponent<Text>().text = "Idling...";
    }
    void TakePictureOnClick()
    {
        TakePicture(SystemInfo.maxTextureSize);
        //statusDisplay.GetComponent<Text>().text;
        
    }    
    string ConvertToBase64(string filePath)
    {
        byte[] imageArray = System.IO.File.ReadAllBytes(filePath);
        string base64ImageRepresentation = System.Convert.ToBase64String(imageArray);
        return base64ImageRepresentation;

        //var newPost = new Demo()
        //{
        //    Title = "Test POST",
        //    Body = "Hello hell!",
        //    UserId = 69
        //};

        //return newPost;
    }    
    IEnumerator APICalls(string _url, string _imgPath, System.Action<string> callback)
    {
        //using(var client = new HttpClient())
        //{
        //    var url = new System.Uri("https://203.162.88.122:18008/getpercent");
        //    var newPost = new Post()
        //    {
        //        file = ConvertToBase64(_imgPath)
        //    };
        //    var newPostJson = UnityEngine.JsonUtility.ToJson(newPost);
        //    var payload = new StringContent(newPostJson, Encoding.UTF8, "application/json");
        //    Debug.Log("complete all preparation step");
        //    var result = client.PostAsync(url, payload).Result.Content;
        //    var resultJson = result.ReadAsStringAsync().Result;
        //    Debug.Log(resultJson);
        //    pathDisplay.GetComponent<Text>().text = resultJson;
        //}    
        WWWForm form = new WWWForm();
        //form.AddField("file", ConvertToBase64(_imgPath));

        //using (UnityWebRequest www = UnityWebRequest.Post(_url, form))
        //{
        //    yield return www.SendWebRequest();
        //    if (www.result != UnityWebRequest.Result.Success)
        //    {
        //        Debug.Log(www.error);
        //    }
        //    else
        //    {
        //        Debug.Log("success");
        //    }
        //}
        form.AddField("Title", "Test POST");
        form.AddField("Body", "Hello hell!");
        form.AddField("UserId", 69);

        using (UnityWebRequest req = UnityWebRequest.Post("https://jsonplaceholder.typicode.com/posts", form))
        {
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(req.error);
            }
            else
            {
                Debug.Log(req.downloadHandler.text);
                Debug.Log("success");
            }
        }
    }
    private void TakePicture(int maxSize)
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            textUIValue.text = "Image path: " + path;

            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, 4000);
                StartCoroutine(APICalls("https://203.162.88.122:18008/getpercent", path, result => Debug.Log(result)));
                //NativeGallery.SaveImageToGallery(texture, "NewAlbum", "photo.jpg");
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                else
                {
                    imgPath.GetComponent<Text>().text = path;
                    pathDisplay.GetComponent<Text>().text = path;
                    displayer.SetActive(true);
                    Debug.Log("successfully activated displayer object");
                }
                // Assign texture to a temporary quad and destroy it after 5 seconds
                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
                quad.transform.forward = Camera.main.transform.forward;
                quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

                Material material = quad.GetComponent<Renderer>().material;
                if (!material.shader.isSupported) // happens when Standard shader is not included in the build
                    material.shader = Shader.Find("Legacy Shaders/Diffuse");

                material.mainTexture = texture;

                Destroy(quad, 5f);

                // If a procedural texture is not destroyed manually,
                // it will only be freed after a scene change
                Destroy(texture, 5f);
            }
        }, maxSize);

        Debug.Log("Permission result: " + permission);
    }

    private void RecordVideo()
    {
        NativeCamera.Permission permission = NativeCamera.RecordVideo((path) =>
        {
            textUIValue.text = "Video path: " + path;

            Debug.Log("Video path: " + path);
            if (path != null)
            {
                // Play the recorded video
                Handheld.PlayFullScreenMovie("file://" + path);
            }
        });

        Debug.Log("Permission result: " + permission);
    }
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadImage : MonoBehaviour
{
    public string imagePath;
    [SerializeField] GameObject pathObj;
    public void Start()
    {
        imagePath = pathObj.GetComponent<Text>().text;
        Debug.Log(imagePath);
        AssignTexture();
    }
    public void AssignTexture()
    {
        this.GetComponent<MeshRenderer>().material.mainTexture = NativeCamera.LoadImageAtPath(imagePath);
        Debug.Log("displayed?");
    }
    public static Texture2D LoadPNG(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;
        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        Debug.Log("successfully get image texture");
        return tex;
    }
}

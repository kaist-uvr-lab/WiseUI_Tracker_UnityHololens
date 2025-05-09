using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

//다른 스트림 모듈로 대체 예정

public class CameraManager : MonoBehaviour
{
    public TextMeshPro mText;
    [HideInInspector]
    WebCamTexture webCamTexture;
    [HideInInspector]
    Texture2D targetTexture;

    [HideInInspector]
    public int mnFrameIdx = 0;
    [HideInInspector]
    public int mnSkip = 4;
    [HideInInspector]
    public int mnDataSize = 0;
    [HideInInspector]
    public byte[] ImageData;

    // Start is called before the first frame update
    void Start()
    {
        var devices = WebCamTexture.devices;
        webCamTexture = new WebCamTexture(WebCamTexture.devices.First<WebCamDevice>().name, 640, 360, 30);
        webCamTexture.Play();
        targetTexture = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGB24, false);
    }

    void LoadWebcamTexture()
    {
        Color[] cdata = webCamTexture.GetPixels();
        targetTexture.SetPixels(cdata);
        targetTexture.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        try {
            if (webCamTexture.didUpdateThisFrame)
            {
                LoadWebcamTexture();
                ImageData = targetTexture.EncodeToJPG(50);
                mnDataSize = ImageData.Length;
                mnFrameIdx++;
            }
        }
        catch(Exception e)
        {
            mText.text = e.ToString();
        }
    }
}

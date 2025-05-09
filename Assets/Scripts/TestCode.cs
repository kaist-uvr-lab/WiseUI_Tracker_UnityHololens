using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    //[DllImport("OpencvBridge")]
    //private static extern void resize(IntPtr src, IntPtr dst, int sw, int sh, int rw, int rh);
    [DllImport("OpencvBridge")]
    private static extern void test();


    public TextMeshPro mText;
    WebCamTexture webCamTexture;
    Texture2D targetTexture;
    Texture2D resizedTexture;

    void LoadWebcamTexture() {
        Color[] cdata = webCamTexture.GetPixels();
        targetTexture.SetPixels(cdata);
        targetTexture.Apply();
    }

    void ScaleTexture(Texture2D src, ref Texture2D dst)
    {
        int targetWidth  = dst.width;
        int targetHeight = dst.height;

        //Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = dst.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = src.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        dst.SetPixels(rpixels, 0);
        dst.Apply();
        //return result;
    }

    // Start is called before the first frame update
    void Start()
    {
        var devices = WebCamTexture.devices;
        webCamTexture = new WebCamTexture(WebCamTexture.devices.First<WebCamDevice>().name, 640, 360, 30);
        webCamTexture.Play();
        targetTexture  = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGB24, false);
        resizedTexture = new Texture2D(webCamTexture.requestedWidth, webCamTexture.requestedHeight, TextureFormat.RGB24, false);
        
        //string str = devices.Length + " " + devices[0].depthCameraName+"||"+webCamTexture.height+" "+webCamTexture.width+"||"+ targetTexture.width+" "+targetTexture.height;
        //mText.text = str;
        ////연결
    }
    int mnFrameIdx = 0;
    int mnSkip = 4;
    double t1 = 0.0;
    double t2 = 0.0;
    int datalen = 0;
    // Update is called once per frame
    void Update()
    {
        if (webCamTexture.didUpdateThisFrame) {
            
            try {
                if (mnFrameIdx % mnSkip == 0)
                {
                    var A = DateTime.Now;
                    LoadWebcamTexture();
                    Color[] texData = targetTexture.GetPixels();
                    GCHandle texHandle = GCHandle.Alloc(texData, GCHandleType.Pinned);
                    IntPtr texPtr = texHandle.AddrOfPinnedObject();

                    Color[] rtexData = new Color[640 * 360];
                    GCHandle texHandle2 = GCHandle.Alloc(texData, GCHandleType.Pinned);
                    IntPtr rtexPtr = texHandle2.AddrOfPinnedObject();

                    var B = DateTime.Now;
                    //ScaleTexture(targetTexture, ref resizedTexture);
                    //byte[] data = targetTexture.EncodeToJPG(50);
                    //resize(texPtr, rtexPtr, webCamTexture.width, webCamTexture.height, 640, 360);
                    test();
                    var C = DateTime.Now;
                    var timeSpan1 = B-A;
                    var timeSpan2 = C-A;
                    t1 = timeSpan1.TotalMilliseconds;
                    t2 = timeSpan2.TotalMilliseconds;
                    datalen = 0;//data.Length;
                    ////전송
                    
                    texHandle.Free();
                    texHandle2.Free();
                }
                //mText.text = "Frame = "+mnFrameIdx + "," +t1+" "+t2+", "+datalen+"||"+ Application.dataPath;
                mnFrameIdx++;
            }
            catch (Exception e){
                //mText.text = e.ToString();
            }
        }        
    }
}

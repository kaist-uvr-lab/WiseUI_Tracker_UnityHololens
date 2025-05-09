using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UDPProcessor
{
    static private UDPProcessor m_pInstance = null;

    static public UDPProcessor Instance
    {
        get
        {
            if (m_pInstance == null)
            {
                m_pInstance = new UDPProcessor();
            }
            return m_pInstance;
        }
    }

    void Connect()
    {

    }

    void Disconnect()
    {

    }

    //여기에 처리 모듈을 등록
}

public class UDPController : MonoBehaviour
{

    public TextMeshPro mText;

    // Start is called before the first frame update
    void Start()
    {
        UdpAsyncHandler.Instance.UdpDataReceived += Process;
    }
    void Process(object sender, UdpEventArgs e)
    {
        try
        {
            int size = e.bdata.Length;
            string msg = System.Text.Encoding.Default.GetString(e.bdata);
            UdpData data = JsonUtility.FromJson<UdpData>(msg);
            data.receivedTime = DateTime.Now;
            
            //parsing code
        }
        catch (Exception ex)
        {
            //debugging text
        }
    }

    // Update is called once per frame
    void Update()
    {
    
    }


    //UnityWebRequest GetRequest(string keyword, int id)
    //{
    //    //string addr2 = SystemManager.Instance.AppData.Address + "/Load?keyword=" + keyword + "&id=" + id + "&src=" + SystemManager.Instance.User.UserName;
    //    //UnityWebRequest request = new UnityWebRequest(addr2);
    //    //request.method = "POST";
    //    //request.downloadHandler = new DownloadHandlerBuffer();
    //    ////request.SendWebRequest();
    //    //return request;
    //}
    //UnityWebRequest GetRequest(string keyword, int id, string src)
    //{
    //    //string addr2 = SystemManager.Instance.AppData.Address + "/Load?keyword=" + keyword + "&id=" + id + "&src=" + src;
    //    //UnityWebRequest request = new UnityWebRequest(addr2);
    //    //request.method = "POST";
    //    //request.downloadHandler = new DownloadHandlerBuffer();
    //    ////request.SendWebRequest();
    //    //return request;
    //}
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestManager : MonoBehaviour
{

    public CameraManager mCamManager;
    public DataSender mSender;
    public SystemManager mSystemManager;
    public TextMeshPro mText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var mnFrame = mCamManager.mnFrameIdx;
        var mnSkip = mCamManager.mnSkip;
        var buffersize = mCamManager.mnDataSize;
        try
        {
            if (mnFrame % mnSkip == 0 && buffersize > 0)
            {
                byte[] bdata = mCamManager.ImageData;
                var timeSpan = DateTime.UtcNow - mSystemManager.StartTime;
                double ts = timeSpan.TotalMilliseconds;
                UdpData idata = new UdpData("Image", mSystemManager.User.UserName, mnFrame, bdata, ts);
                StartCoroutine(mSender.SendData(idata));
            }
        }
        catch(Exception e)
        {
            mText.text = e.ToString();
        }
    }
}

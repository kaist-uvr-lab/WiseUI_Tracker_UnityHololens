using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

//게임오브젝트로 추가
public class HL2DataAPI : MonoBehaviour, IMixedRealitySpeechHandler
{
    [System.Serializable]
    public struct HL2SensorData
    {
        public byte[] imageData;
        public Vector3 headPosition;
        public Quaternion headRotation;
        public Vector3 eyeGazeOrigin;
        public Vector3 eyeGazeDirection;
        public Dictionary<TrackedHandJoint, Pose> rightHandJoints, leftHandJoints;
        public string lastRecognizedSpeech;
        public float timestamp;
    }

    [Header("Capture Settings")]  
    [Range(30, 60)]
    public int targetFPS = 60;
    [Range(10, 100)]
    public int jpgQuality = 75;
      
    public byte[] LatestImageBytes { get; private set; }
    public string LastSpeechKeyword { get; private set; } = "";

    //Image
    private WebCamTexture rgbTexture;
    private RenderTexture renderTexture;
    private Texture2D outputTexture;
    private bool isProcessingImage = false;

    //Speech
    private string currentSpeechText = "";
    void OnEnable() => CoreServices.InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);
    void OnDisable() => CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySpeechHandler>(this);

    int rw = 320;
    int rh = 180;

    void Start()
    {
        // RGB 카메라 초기화
        if (WebCamTexture.devices.Length > 0)
        {
            rgbTexture = new WebCamTexture(WebCamTexture.devices[0].name, 1280, 720, 30);
            rgbTexture.Play();

            renderTexture = new RenderTexture(rw, rh, 0);
            outputTexture = new Texture2D(rw, rh, TextureFormat.RGB24, false);
        }
    }

    public void GetSyncedHL2Data(System.Action<HL2SensorData> callback, int quality = 75)
    {
        if (rgbTexture == null || !rgbTexture.isPlaying) return;


        // 오른손 관절 Dictionary 생성 (21개 관절)
        Dictionary<TrackedHandJoint, Pose> GetHandData(Handedness handedness)
        {
            var joints = new Dictionary<TrackedHandJoint, Pose>();
            foreach (TrackedHandJoint j in System.Enum.GetValues(typeof(TrackedHandJoint)))
            {
                if (j != TrackedHandJoint.None &&
                    HandJointUtils.TryGetJointPose(j, handedness, out MixedRealityPose p))
                {
                    joints[j] = new Pose(p.Position, p.Rotation);
                }
            }
            return joints;
        }

        //데이터 복사
        HL2SensorData frame = new HL2SensorData
        {
            headPosition = Camera.main.transform.position,
            headRotation = Camera.main.transform.rotation,

            eyeGazeOrigin = CoreServices.InputSystem.EyeGazeProvider.GazeOrigin,
            eyeGazeDirection = CoreServices.InputSystem.EyeGazeProvider.GazeDirection,

            leftHandJoints = GetHandData(Handedness.Left),
            rightHandJoints = GetHandData(Handedness.Right),
            
            timestamp = Time.time
        };

        //GPU 복사 시작
        Graphics.Blit(rgbTexture, renderTexture);

        //비동기 처리 요청
        AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGB24, (request) => {
            if (request.hasError) return;

            // GPU 처리가 끝난 후 JPG 압축
            outputTexture.LoadRawTextureData(request.GetData<byte>());
            outputTexture.Apply();
            frame.imageData = outputTexture.EncodeToJPG(quality);

            //미리 기록해둔 센서 데이터와 함께 결과 반환
            callback?.Invoke(frame);
        });
    }
    public string GetLastSpeech() => currentSpeechText;
    // --- Speech Interaction Interface ---
    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        currentSpeechText = eventData.Command.Keyword;
        Debug.Log($"[Speech Recognized]: {currentSpeechText}");
    }
}
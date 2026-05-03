using UnityEngine;
public class HL2DataTester : MonoBehaviour
{
    // 인스펙터에서 HL2DataAPI 컴포넌트를 연결
    public HL2DataAPI api;

    [Header("Test Config")]
    public bool autoCapture = true;
    public float captureInterval = 0.5f;
    private float lastTime;

    void Update()
    {
        // 1. 수동 테스트: 스페이스바를 누를 때 캡처
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestData();
        }

        // 2. 자동 테스트: 일정 간격마다 캡처
        if (autoCapture && Time.time - lastTime > captureInterval)
        {
            RequestData();
            lastTime = Time.time;
        }
    }

    private void RequestData()
    {
        api.GetSyncedHL2Data((data) => {
            // 데이터가 잘 들어오는지 콘솔에 출력
            Debug.Log($"<color=cyan>[Captured]</color> Time: {data.timestamp:F2}s, " +
                      $"Img: {data.imageData.Length / 1024}KB, " +
                      $"Hand Joints: {data.rightHandJoints.Count}, {data.leftHandJoints.Count}" +
                      $"Head Pos: {data.headPosition}");

            // 시선 방향 확인용 로그
            Debug.Log($"Eye Gaze Dir: {data.eyeGazeDirection}");

            // 데이터 전송 및 활용
            // Example: MyServer.SendData(data);
        });
    }
}
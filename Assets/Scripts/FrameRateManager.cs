using UnityEngine;

public class FrameRateManager : MonoBehaviour
{
    void Start()
    {
        // iOS에서 최대 프레임 레이트를 60으로 설정
#if UNITY_IOS
        Application.targetFrameRate = 60;
#endif

        // Android에서 최대 프레임 레이트를 60으로 설정
#if UNITY_ANDROID
        Application.targetFrameRate = 60;
#endif

        // 기본 프레임 레이트 설정
        Application.targetFrameRate = 60;
    }
}

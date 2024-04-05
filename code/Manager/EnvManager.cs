using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 環境周りを管理するManager
/// </summary>
public class EnvManager : MonoBehaviour
{
    public static EnvManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private VolumeProfile volumeProfile;
    [SerializeField] Volume volume;
    [SerializeField] private Light2D globalLight;
    private int N = 20;
    private ColorAdjustments colorAdjustments;
    private const float normal = 0;//mode0
    private const float hack = -100;//mode1
    private int mode = 0;

    void Start()
    {
        volumeProfile = volume.sharedProfile;
        if (volumeProfile.TryGet(out colorAdjustments))
        {
            SetSaturation(normal);
            SetGlobalIntensity(0);//コメントアウトすれば明るいまま
        }
    }

    void SetSaturation(float value)
    {
        colorAdjustments.saturation.value = value;
    }

    void SetGlobalIntensity(float value)
    {
        globalLight.intensity = value;
    }

    /// <summary>
    /// リスタート時に通常モードにする
    /// </summary>
    public void ReStart()
    {
        Time.timeScale = 1;
        colorAdjustments.saturation.value = normal;
        globalLight.intensity = 0;
    }

    public async UniTask SetNormal(CancellationToken token)
    {
        for (int i = 0; i < N; i++)
        {
            colorAdjustments.saturation.value += (normal - hack)/N;
            globalLight.intensity -= (float)(1 - 0) / N;
            await UniTask.Delay(10);
        }
    }

    public async UniTask SetHack(CancellationToken token)
    {
        for (int i = 0; i < N; i++)
        {
            colorAdjustments.saturation.value -= (normal - hack) / N;
            globalLight.intensity += (float)(1 - 0) / N;
            await UniTask.Delay(10);
        }
    }
}

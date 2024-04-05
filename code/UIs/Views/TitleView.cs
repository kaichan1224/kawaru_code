using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class TitleView : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Slider seSlider;
    [SerializeField] LineView lineView;
    private ReactiveProperty<float> seVolume = new ReactiveProperty<float>(0.5f);
    public IReadOnlyReactiveProperty<float> SeVolume => seVolume;
    [SerializeField] Slider bgmSlider;
    private ReactiveProperty<float> bgmVolume = new ReactiveProperty<float>(0.5f);
    public IReadOnlyReactiveProperty<float> BgmVolume => bgmVolume;
    public IObservable<Unit> OnGameStart => startButton.OnClickAsObservable();

    public void Show()
    {
        //lineView.gameObject.SetActive(true);
        gameObject.SetActive(true);
        lineView.gameObject.SetActive(true);
        lineView.DrawTitleLine();
    }
    public void Hide()
    {
        lineView.gameObject.SetActive(false);
        gameObject.SetActive(false);
        StopLine();
    }

    private void Update()
    {
        seVolume.Value = seSlider.value;
        bgmVolume.Value = bgmSlider.value;
    }

    private void StopLine()
    {
        lineView.CancelToken();
    }
}

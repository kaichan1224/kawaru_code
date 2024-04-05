using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using Cysharp.Threading.Tasks;
using TMPro;
using System.Threading;

public class InGameView : MonoBehaviour
{
    [SerializeField] Fade fade;
    [SerializeField] float fadeTime;
    [SerializeField] float intervalTime;
    [SerializeField] Text fadeText;
    [SerializeField] Text bulletCountText;
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetBulletCount(int count)
    {
        bulletCountText.text = $"のこり{count}はつ";
    }

    /// <summary>
    /// TODOシーン切り替え演出
    /// </summary>
    public async UniTask NextStageAsync(CancellationToken token)
    {
        bulletCountText.gameObject.SetActive(false);
        await fade.FadeIn(fadeTime, () => FadeOut(token));
    }

    async void FadeOut(CancellationToken token)
    {
        //fadeText.text = "Next Floor";
        await UniTask.Delay((int)(intervalTime * 1000),cancellationToken:token);
        fadeText.text = "";
        await fade.FadeOut(fadeTime);
        bulletCountText.gameObject.SetActive(true);
    }
}

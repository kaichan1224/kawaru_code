using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class ResultView : MonoBehaviour
{
    [SerializeField] Button titleButton;
    [SerializeField] Button restartButton;
    [SerializeField] LineView lineView;
    public IObservable<Unit> OnTitle => titleButton.OnClickAsObservable();
    public IObservable<Unit> OnRestart => restartButton.OnClickAsObservable();
    public void Show()
    {
        lineView.CancelToken();
        lineView.gameObject.SetActive(true);
        lineView.DrawGameOverLine();
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

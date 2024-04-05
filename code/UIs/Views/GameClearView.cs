using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UniRx;
using System;

public class GameClearView : MonoBehaviour
{
    [SerializeField] Text endText;
    [SerializeField] Button tapButton;
    public IObservable<Unit> OnTap => tapButton.OnClickAsObservable();
    public void Show()
    {
        gameObject.SetActive(true);
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

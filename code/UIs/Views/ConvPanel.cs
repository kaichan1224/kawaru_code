using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class ConvPanel : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text wordsText;
    public void SetName(string name)
    { 
        nameText.text = name;
    }

    public void SetWords(string words)
    {
        wordsText.text = "";
        wordsText.DOText(words, words.Length * 0.1f);
    }
}

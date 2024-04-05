using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackPanel : MonoBehaviour
{
    [SerializeField] Text hackName;
    [SerializeField] Text explain;
    public void SetHack(string hackName,string explain)
    {
        this.hackName.text = hackName;
        this.explain.text = explain;
    }
}

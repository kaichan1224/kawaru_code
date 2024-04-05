using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ConvData", menuName = "Data/ConvData")]
public class ConvData : ScriptableObject
{
    public List<ConvParam> datas;
    [Serializable]
    public class ConvParam
    {
        public string name;
        public string words;
    }
}

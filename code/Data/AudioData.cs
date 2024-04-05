using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Data/AudioData")]
public class AudioData : ScriptableObject
{
    [SerializeField] List<DictPair<SEName, AudioClip>> se;
    [SerializeField] List<DictPair<BGMName, AudioClip>> bgm;
    public AudioClip GetSE(SEName soundName)
    {
        foreach (var data in se)
        {
            if (data.IsEquelKey(soundName))
            {
                return data.Value;
            }
        }
        return null;
    }

    public AudioClip GetBGM(BGMName soundName)
    {
        foreach (var data in bgm)
        {
            if (data.IsEquelKey(soundName))
            {
                return data.Value;
            }
        }
        return null;
    }

    public enum BGMName
    {
        titleBGM,
        inGameBGM,
        gameOverBGM,
        gameClearBGM,
        alertBGM,
        typeBGM,//ハッキング時のかちゃかちゃ音
    }

    //追加するときは末尾に足してください
    public enum SEName
    { 
        hackSE,
        DoorSE,
        DeathSE,
        ClickSE,
        TypeSE,//文字送りの音
        ShotSE,
        BrakeSE,
    }
}

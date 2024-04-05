using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
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
    //AudioSource
    [SerializeField] private AudioSource bgmAudio;
    [SerializeField] private AudioSource bgm2Audio;
    [SerializeField] private AudioSource SEAudio;
    //データ
    [SerializeField] private AudioData soundMasterData;
    public void PlayBGM(AudioData.BGMName soundName)
    {
        bgmAudio.clip = soundMasterData.GetBGM(soundName);
        bgmAudio.Play();
    }

    public void StopBGM()
    {
        bgmAudio.Stop();
    }

    public void PlaySE(AudioData.SEName soundName)
    {
        SEAudio.PlayOneShot(soundMasterData.GetSE(soundName));
    }

    public void SetBGMVolume(float value)
    {
        // 音楽の音量をスライドバーの値に変更
        bgmAudio.volume = value;
        bgm2Audio.volume = value;
    }

    public void SetSEVolume(float value)
    {
        // 音楽の音量をスライドバーの値に変更
        SEAudio.volume = value;
    }

    public void Play2BGM(AudioData.BGMName soundName)
    {
        bgm2Audio.clip = soundMasterData.GetBGM(soundName);
        bgm2Audio.Play();
    }

    public void Stop2BGM()
    {
        bgm2Audio.Stop();
    }




}

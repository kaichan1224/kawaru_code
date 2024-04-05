using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;
using VContainer.Unity;
using System;

public class TitlePresenter
{
    TitleView _titleView;
    StateModel _stateModel;
    public event Action StartGameCallBack;

    TitlePresenter(TitleView titleView,StateModel stateModel)
    {
        this._titleView = titleView;
        this._stateModel = stateModel;
    }

    public void Bind()
    {
        _stateModel.CurrentGameStateProp.Subscribe(state =>
        {
            if (state == StateModel.GameState.Title)
            {
                AudioManager.instance.PlayBGM(AudioData.BGMName.titleBGM);
                _titleView.Show();
            }
            else
            {
                _titleView.Hide();
            }
        });

        _titleView.OnGameStart.Subscribe(_ =>
        {
            StartGameCallBack?.Invoke();
        });

        _titleView.BgmVolume.Subscribe(value =>
        {
            AudioManager.instance.SetBGMVolume(value);
        });

        _titleView.SeVolume.Subscribe(value =>
        {
            AudioManager.instance.SetSEVolume(value);
        });
    }
}

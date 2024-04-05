using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Cysharp.Threading.Tasks;

public class GameClearPresenter
{
    GameClearView _gameClearView;
    StateModel _stateModel;
    ConvData convData;

    GameClearPresenter(GameClearView gameClearView, StateModel stateModel)
    {
        _gameClearView = gameClearView;
        _stateModel = stateModel;
    }

    public void Bind()
    {
        _stateModel.CurrentGameStateProp.Subscribe(state =>
        {
            if (state == StateModel.GameState.GameClear)
            {
                AudioManager.instance.StopBGM();
                _gameClearView.Show();
            }
            else
            {
                _gameClearView.Hide();
            }
        });

        _gameClearView.OnTap
        .Subscribe(_ =>
        {
            _stateModel.SetGameState(StateModel.GameState.Title);
        });
    }
}

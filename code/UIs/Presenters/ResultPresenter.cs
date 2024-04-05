using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;
using VContainer.Unity;
using System;

public class ResultPresenter
{
    ResultView _resultView;
    StateModel _stateModel;
    public event Action RestartCallBack;
    public event Action TitleCallBack;
    ResultPresenter(ResultView resultView,StateModel stateModel)
    {
        _resultView = resultView;
        _stateModel = stateModel;
    }

    public void Bind()
    {
        _stateModel.CurrentGameStateProp.Subscribe(state =>
        {
            if (state == StateModel.GameState.Result)
            {
                _resultView.Show();
            }
            else
            {
                _resultView.Hide();
            }
        });

        _resultView.OnRestart.Subscribe(_ =>
        {
            RestartCallBack?.Invoke();
        });

        _resultView.OnTitle.Subscribe(_ =>
        {
            TitleCallBack?.Invoke();
        });
    }
}

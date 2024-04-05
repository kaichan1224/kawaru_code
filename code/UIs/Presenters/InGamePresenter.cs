using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;
using VContainer.Unity;
using Cysharp.Threading.Tasks;
using System.Threading;

public class InGamePresenter
{
    InGameView _inGameView;
    StateModel _stateModel;
    InGamePresenter(InGameView inGameView,StateModel stateModel)
    {
        _inGameView = inGameView;
        _stateModel = stateModel;

    }

    public void Bind()
    {
        _stateModel.CurrentGameStateProp.Subscribe(state =>
        {
            if (state == StateModel.GameState.InGame)
            {
                _inGameView.Show();
            }
            else
            {
                _inGameView.Hide();
            }
        });
    }

    public async UniTask NextStageAsync(CancellationToken token)
    {
        await _inGameView.NextStageAsync(token);
    }
}

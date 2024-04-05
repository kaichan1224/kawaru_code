using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// ゲームの状態を保持するModelクラス
/// </summary>
public class StateModel
{
    /// <summary>
    /// ゲームの状況
    /// </summary>
    public enum GameState
    {
        Title,//タイトル画面
        InGame,//インゲーム
        Result,//リザルト
        GameClear//ゲームクリア
    }


    StateModel()
    {
        _currentGameStateProp = new ReactiveProperty<GameState>(GameState.Title);
    }

    private ReactiveProperty<GameState> _currentGameStateProp;
    public IReadOnlyReactiveProperty<GameState> CurrentGameStateProp => _currentGameStateProp;

    public void SetGameState(GameState gameState)
    {
        _currentGameStateProp.Value = gameState;
    }
}

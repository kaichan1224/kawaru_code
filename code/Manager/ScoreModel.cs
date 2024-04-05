using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

//ゲームのスコアに関する情報を持つクラス
public class ScoreModel
{
    private ReactiveProperty<int> _score;
    public IReadOnlyReactiveProperty<int> Score => _score;
    public ScoreModel()
    {
        _score = new ReactiveProperty<int>(0);
    }
    public void SetScore(int score)
    {
        _score.Value = score;
    }

    public void AddScore(int deltaScore)
    {
        _score.Value += deltaScore;
    }

    public void CalculateScore()
    {

    }
}



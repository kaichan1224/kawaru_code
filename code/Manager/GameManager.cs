using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.SceneManagement;
using UniRx.Triggers;
using UniRx;

/// <summary>
/// ゲームを総管理するマネージャクラス
/// </summary>
public class GameManager : MonoBehaviour
{
    [Inject] ScoreModel scoreModel;
    [Inject] StateModel stateModel;
    [Inject] TitlePresenter titlePresenter;
    [Inject] ResultPresenter resultPresenter;
    [Inject] InGamePresenter inGamePresenter;
    [Inject] GameClearPresenter gameClearPresenter;
    [Inject] PlayerManager playerManager;
    [Inject] StageManager stageManager;
    private CancellationTokenSource gameOverCancellToken = new CancellationTokenSource();

    private void Start()
    {
        Initialize();
        Bind();
    }

    private void Bind()
    {
        titlePresenter.Bind();
        resultPresenter.Bind();
        inGamePresenter.Bind();
        gameClearPresenter.Bind();
        this.UpdateAsObservable()
            .Where(_ => stateModel.CurrentGameStateProp.Value == StateModel.GameState.InGame)
            .Where(_ => Input.GetKeyDown(KeyCode.R))
            .Subscribe(_ => {
                GameReStart();
            });
    }

    private void CancellGameOverToken()
    {
        gameOverCancellToken?.Cancel();
        gameOverCancellToken?.Dispose();
        gameOverCancellToken = null;
    }

    /// <summary>
    /// 初期化メソッド
    /// </summary>
    private void Initialize()
    {
        titlePresenter.StartGameCallBack += () => GameStart();
        playerManager.DeathCallBack += async () => await GameOver();
        playerManager.NextStageCallBack += async () => await NextStage(gameOverCancellToken.Token);
        resultPresenter.TitleCallBack += () => BackToTitle();
        resultPresenter.RestartCallBack += () => GameReStart();
    }

    /// <summary>
    /// ゲームスタート時の処理を記述 TODO
    /// </summary>
    void GameStart()
    {
        gameOverCancellToken = new CancellationTokenSource();
        AudioManager.instance.PlayBGM(AudioData.BGMName.inGameBGM);
        stateModel.SetGameState(StateModel.GameState.InGame);
        playerManager.CreatePlayer(stageManager.GetPlayerFirstPoint());
        stageManager.Init();
        stageManager.NextStage();
    }

    async UniTask NextStage(CancellationToken token)
    {
        gameOverCancellToken = new CancellationTokenSource();
        if (stateModel.CurrentGameStateProp.Value != StateModel.GameState.InGame)
            return;
        stageManager.StagePlus();
        if (stageManager.IsGoal())
        {
            GameClear();
            return;
        }
        await inGamePresenter.NextStageAsync(gameOverCancellToken.Token);
        playerManager.CreatePlayer(stageManager.GetPlayerFirstPoint());
        stageManager.NextStage();//プレイヤーのインスタンスを生成したあとに、ステージを生成すること
    }

    /// <summary>
    /// ゲームオーバー時の処理
    /// </summary>
    async UniTask GameOver()
    {
        CancellGameOverToken();
        await playerManager.DestroyPlayer();
        AudioManager.instance.Stop2BGM();
        AudioManager.instance.PlayBGM(AudioData.BGMName.gameOverBGM);
        stateModel.SetGameState(StateModel.GameState.Result);
    }

    void GameReStart()
    {
        EnvManager.instance.ReStart();
        gameOverCancellToken = new CancellationTokenSource();
        AudioManager.instance.PlayBGM(AudioData.BGMName.inGameBGM);
        stateModel.SetGameState(StateModel.GameState.InGame);
        playerManager.CreatePlayer(stageManager.GetPlayerFirstPoint());
        stageManager.ReStartCurrentStage();
    }

    /// <summary>
    /// ゲームクリア時の処理を記述 TODO
    /// </summary>
    void GameClear()
    {
        AudioManager.instance.PlayBGM(AudioData.BGMName.gameClearBGM);
        playerManager.GameClearPlayer();
        stateModel.SetGameState(StateModel.GameState.GameClear);
        stageManager.Init();
    }

    void BackToTitle()
    {
        SceneManager.LoadScene("Game");
    }

}

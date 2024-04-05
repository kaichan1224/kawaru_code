using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// Monobehaviourを継承しないクラスのインスタンスを生成する場合はここでやる！
/// </summary>
public class GameLifeTimeScope : LifetimeScope
{
    [SerializeField] InGameView inGameView;
    [SerializeField] ResultView resultView;
    [SerializeField] TitleView titleView;
    [SerializeField] GameClearView gameClearView;
    [SerializeField] PlayerManager playerManager;
    [SerializeField] CameraManager cameraManager;
    [SerializeField] StageManager stageManager;
    [SerializeField] ConvData convData;

    protected override void Configure(IContainerBuilder builder)
    {
        //Model
        builder.Register<ScoreModel>(Lifetime.Singleton);
        builder.Register<StateModel>(Lifetime.Singleton);
        //各種View
        builder.RegisterInstance(inGameView);
        builder.RegisterInstance(resultView);
        builder.RegisterInstance(titleView);
        builder.RegisterInstance(gameClearView);
        //Presenter
        builder.Register<TitlePresenter>(Lifetime.Singleton);
        builder.Register<ResultPresenter>(Lifetime.Singleton);
        builder.Register<InGamePresenter>(Lifetime.Singleton);
        builder.Register<GameClearPresenter>(Lifetime.Singleton);
        //Manager
        builder.RegisterInstance<CameraManager>(cameraManager);
        builder.RegisterInstance<PlayerManager>(playerManager);
        builder.RegisterInstance(stageManager);
        //Datas
        builder.RegisterInstance(convData);
    }
}

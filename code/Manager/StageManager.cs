using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using System;

public class StageManager : MonoBehaviour
{
    [Inject] PlayerManager playerManager;
    [Inject] InGameView inGameView;
    [SerializeField] List<Stage> stages;
    private int currentStageIdx = 0;
    private Stage currentStage;
    public void Init()
    {
        currentStageIdx = 0;
    }

    public void ReStartCurrentStage()
    {
        if (currentStage != null)
            Destroy(currentStage.gameObject);
        currentStage = Instantiate(stages[currentStageIdx]);
        //敵の初期化
        IEnemyCore[] enemies = currentStage.enemyParent.GetComponentsInChildren<IEnemyCore>();
        foreach (var enemy in enemies)
        {
            Debug.Log("敵初期化");
            Debug.Log(playerManager.PlayerCore);
            enemy.SetTarget(playerManager.PlayerCore);

        }
        //ハック対象の初期化
        IHackable[] hackables = currentStage.hackTargets.GetComponentsInChildren<IHackable>();
        foreach (var hacks in hackables)
        {
            Debug.Log("ハッキング初期化");
            hacks.Init(playerManager.PlayerCore.gameObject.transform, inGameView.gameObject.transform);
            playerManager.AddHackObject(hacks);
        }
    }

    public void NextStage()
    {
        if (currentStage != null)
            Destroy(currentStage.gameObject);
        currentStage = Instantiate(stages[currentStageIdx]);
        //敵の初期化
        IEnemyCore[] enemies = currentStage.enemyParent.GetComponentsInChildren<IEnemyCore>();
        foreach (var enemy in enemies)
        {
            Debug.Log("敵初期化");
            Debug.Log(playerManager.PlayerCore);
            enemy.SetTarget(playerManager.PlayerCore);

        }
        //ハック対象の初期化
        IHackable[] hackables = currentStage.hackTargets.GetComponentsInChildren<IHackable>();
        foreach (var hacks in hackables)
        {
            Debug.Log("ハッキング初期化");
            hacks.Init(playerManager.PlayerCore.gameObject.transform,inGameView.gameObject.transform);
            playerManager.AddHackObject(hacks);
        }
    }

    public void StagePlus()
    {
        currentStageIdx++;
    }
            

    public bool IsGoal()
    {
        return currentStageIdx == stages.Count;
    }   


    public void SetStage()
    { 
        
    }

    public Vector2 GetPlayerFirstPoint()
    {
        return stages[currentStageIdx].firstPoint.position;
    }
}

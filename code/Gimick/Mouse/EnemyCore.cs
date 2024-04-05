using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// 
/// </summary>
public class EnemyCore : MonoBehaviour,IEnemyCore
{
    public enum EnemyState
    {
        MoveAround,
        Attack,
        None,
    }

    private ReactiveProperty<EnemyState> enemyStateProp;
    public IReadOnlyReactiveProperty<EnemyState> EnemyStateProp => enemyStateProp;
    [SerializeField] Searcher searcher;
    [SerializeField] EnemyMove enemyMove;
    [SerializeField] float chaceTime = 10f;
    private float timer = 0;
    private bool flag;
    public bool CanCheckFlag => flag;
    public void SetTarget(PlayerCore player)
    {
        enemyMove.SetTarget(player.gameObject.transform);
        player.OnDeath.Subscribe(_ =>
        {
            enemyStateProp.Value = EnemyState.None;
        }).AddTo(player.gameObject);
    }
    private void Start()
    {
        Init();
        Bind();
    }

    public void SetState(EnemyState enemyState)
    {
        enemyStateProp.Value = enemyState;
    }

    void Init()
    {
        enemyStateProp = new ReactiveProperty<EnemyState>(EnemyState.MoveAround);
        enemyMove.Init();
    }

    void Bind()
    {
        enemyStateProp.Where(state => state == EnemyState.MoveAround).Subscribe(state =>
        {
            searcher.InActiveDengerLight();
            enemyMove.StartPathAction();
        }).AddTo(this);

        enemyStateProp.Where(state => state == EnemyState.Attack).Subscribe(async state =>
        {
            AudioManager.instance.Play2BGM(AudioData.BGMName.alertBGM);
            try
            {
                await enemyMove.AttackMove(gameObject.GetCancellationTokenOnDestroy());
            }
            catch (OperationCanceledException)
            {
                Debug.Log("キャンセルされました");
            }
        }).AddTo(this);

        //何も処理しない
        enemyStateProp.Where(state => state == EnemyState.None).Subscribe(state =>
        {
            searcher.InActiveDengerLight();
            enemyMove.StopAction();
            searcher.InActiveCollider();
        }).AddTo(this);

        ///Searcherの当たり判定
        searcher.gameObject.OnTriggerEnter2DAsObservable().Subscribe(collision =>
        {
            if (collision.gameObject.tag == "Player")
            {
                enemyStateProp.Value = EnemyState.Attack;
                flag = true;
            }
        }).AddTo(this);

        gameObject.OnCollisionEnter2DAsObservable().Subscribe(collision =>
        {
            if (collision.gameObject.tag == "Bullet")
            {
                Death();
            }
        }).AddTo(this);
    }

    public void Death()
    {
        //要変更
        AudioManager.instance.Stop2BGM();
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Death();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (enemyStateProp.Value == EnemyState.Attack)
        {
            Death();
        }
    }
}

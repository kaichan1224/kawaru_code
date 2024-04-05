using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

/// <summary>
/// プレイヤーの当たり判定
/// </summary>
public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    public void InActive()
    {
        _collider.enabled = false;
    }
    //死亡通知を送る
    private Subject<Unit> _deathSubject = new Subject<Unit>();
    public IObservable<Unit> OnDeath => _deathSubject;

    //完全クリア通知を送る
    private Subject<Unit> _clearSubject = new Subject<Unit>();
    public IObservable<Unit> OnClear => _clearSubject;

    //次のステージへの通知を送る
    private Subject<NextDoor> _nextStageSubject = new Subject<NextDoor>();
    public IObservable<NextDoor> OnNextStage => _nextStageSubject;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //通知を送る処理
        //_deathSubject.OnNext(Unit.Default);
        if (collision.gameObject.tag == "Goal")
        {
            _clearSubject.OnNext(Unit.Default);
        }
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("HitEnemy");
            _deathSubject.OnNext(Unit.Default);
        }
        if (collision.gameObject.tag == "Next")
        {
            var nextDoor = collision.gameObject.GetComponent<NextDoor>();
            _nextStageSubject.OnNext(nextDoor);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            _deathSubject.OnNext(Unit.Default);
        }
    }

    public void PublishDeathSubject()
    {
        _deathSubject.OnNext(Unit.Default);
    }
}

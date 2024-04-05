using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using DG.Tweening;


public class PlayerCore : MonoBehaviour
{
    //状態
    public enum HackState
    {
        NONHACK,
        HACK,
    }

    //向いてる方向
    public enum Direction
    {
        Up,
        Down,
        Right,
        Left
    }

    //プレイヤーのマスタデータ
    [SerializeField] PlayerData _playerData;
    [SerializeField] PlayerCollision _playerCollision;
    [SerializeField] PlayerMove playerMove;
    [SerializeField] PlayerAttack playerAttack;
    [SerializeField] PlayerAnimation playerAnimation;
    [SerializeField] Rigidbody2D rb;
 
    //ハッキング状態
    private ReactiveProperty<HackState> _hackStateProp;
    public IReadOnlyReactiveProperty<HackState> HackStateProp => _hackStateProp;
    public HackState HackState_ => _hackStateProp.Value;

    //プレイヤーの向いている方向
    private ReactiveProperty<Direction> _currentDirection;
    public IReadOnlyReactiveProperty<Direction> CurrentDirection => _currentDirection;
    public Direction Direction_ => _currentDirection.Value;

    //プレイヤー死亡通知
    private Subject<Unit> deathSubject = new Subject<Unit>();
    public IObservable<Unit> OnDeath => deathSubject;


    //弾数
    public IReadOnlyReactiveProperty<int> BulletCount => playerAttack.BulletCount;

    //コールバック関数
    public event Action NextStageCallBack;//次のステージクリア
    public event Action DeathCallBack;//死亡時コールバック

    //入力
    private IInputEventProvider _inputEventProvider;

    //Hack切替フラグ
    private bool canHackFlag;

    /// <summary>
    /// プレイヤーのインスタンスが生成された時に呼ぶ初期化メソッド
    /// </summary>
    public void Initialaize()
    {
        _inputEventProvider = GetComponent<IInputEventProvider>();
        SetData();
        Bind();
    }

    private void Bind()
    {
        //クリア時アクション
        _playerCollision.OnClear.Subscribe(_ =>
        {
            //NextStageCallBack?.Invoke();
        })
        .AddTo(this);

        //死亡時アクション
        _playerCollision.OnDeath.Subscribe(_ =>
        {
            deathSubject.OnNext(Unit.Default);
            _playerCollision.InActive();
            DeathCallBack?.Invoke();
        })
        .AddTo(this);

        //ハッキング入力受け取り
        _inputEventProvider.OnHack.Where(_ => canHackFlag).Subscribe(_ =>
        {
            canHackFlag = false;
            SwitchHackMode();
        }).AddTo(this);

        //次のステージ(一回のみ発行する)
        _playerCollision.OnNextStage.Take(1).Subscribe(async nextDoor =>
        {
            _inputEventProvider.Stop();
            rb.velocity = Vector2.zero;
            await nextDoor.OpenDoor(this.GetCancellationTokenOnDestroy());
            playerAnimation.StopAnimation();
            playerAnimation.NextWalk();
            for (int i = 0; i < 10; i++)
            {
                transform.position += new Vector3(0,0.15f,0);
                await UniTask.Delay(100);
            }
            //await transform.DOMoveY(transform.position.y + 0.8f,0.75f).SetEase(Ease.Linear).WithCancellation(this.GetCancellationTokenOnDestroy());
            await nextDoor.CloseDoor(this.GetCancellationTokenOnDestroy());
            NextStageCallBack?.Invoke();
        }).AddTo(this);

        //ハッキングMode時
        _hackStateProp.Where(state => state == HackState.HACK).Subscribe
            (async _ =>
            {
                await EnvManager.instance.SetHack(this.GetCancellationTokenOnDestroy());
                playerAnimation.HackStartAnim();
                AudioManager.instance.Play2BGM(AudioData.BGMName.typeBGM);
                playerMove.SetCanMove(false);
                playerAttack.SetCanShot(false);
                await UniTask.Delay(900);
                canHackFlag = true;
            }).AddTo(this);

        //NonHack時
        _hackStateProp.Where(state => state == HackState.NONHACK).Skip(1).Subscribe
            (async _ =>
            {
                await EnvManager.instance.SetNormal(this.GetCancellationTokenOnDestroy());
                playerAnimation.HackEndAnim();
                AudioManager.instance.Stop2BGM();
                //アニメーション終わったら移動するようにするための待機時間
                await UniTask.Delay(900);
                playerMove.SetCanMove(true);
                playerAttack.SetCanShot(true);
                canHackFlag = true;
            }).AddTo(this);

        //キー入力方向受付
        _inputEventProvider.MoveDirection.Subscribe(d =>
        {
            if (d.y != 0)
            {
                if (d.y > 0)
                {
                    _currentDirection.Value = Direction.Up;
                }
                else
                {
                    _currentDirection.Value = Direction.Down;
                }
            }
            if (d.x != 0)
            {
                if (d.x > 0)
                {
                    _currentDirection.Value = Direction.Right;
                }
                else
                {
                    _currentDirection.Value = Direction.Left;
                }
            }
        }).AddTo(this);
    }

    /// <summary>
    /// データ初期化
    /// </summary>
    private void SetData()
    {
        _hackStateProp = new ReactiveProperty<HackState>(HackState.NONHACK);
        _currentDirection = new ReactiveProperty<Direction>(Direction.Right);
        playerAttack.SetBulletCounut(3);
        canHackFlag = true;
    }

    public void SetDirection()
    {

    }

    public async UniTask DeathPlayer()
    {
        _inputEventProvider.Stop();//入力受付をキャンセルする
        rb.bodyType = RigidbodyType2D.Static;
        playerMove.SetCanMove(false);//移動をしないようにする
        await playerAnimation.DeathAsync(this.GetCancellationTokenOnDestroy());
    }

    private void SwitchHackMode()
    {
        if (HackStateProp.Value == HackState.NONHACK)
        {
            _hackStateProp.Value = HackState.HACK;
        }
        else
        {
            _hackStateProp.Value = HackState.NONHACK;
        }
    }
}

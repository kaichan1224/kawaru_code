using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx.Triggers;
using System;

public class HumanEnemyCore : MonoBehaviour,IEnemyCore,IHackable
{
    public enum HumanEnemyState
    {
        Move,
        Shot,
        Phone,
    }
    [SerializeField] HumanEnemyAttack humanEnemyAttack;
    [SerializeField] HumanEnemyMove humanEnemyMove;
    [SerializeField] HumanEnemyAnimation humanEnemyAnimation;
    [SerializeField] Searcher searcher;
    [SerializeField] GameObject centerLight;
    [SerializeField] SpriteRenderer spriteRenderer;
    private ReactiveProperty<HumanEnemyState> enemyStateProp;
    public IReadOnlyReactiveProperty<HumanEnemyState> EnemyStateProp => enemyStateProp;
    private ReactiveProperty<int> lifeProp;
    public IReadOnlyReactiveProperty<int> LifeProp => lifeProp;

    #region IHackable
    private PlayerCore playerCore;
    private PlayerCore.HackState currentHackState;
    private Transform playerTransform;
    [SerializeField] HackLine hackLinePrefab;
    [SerializeField] HackPanel expPanelPrefab;
    [SerializeField] string hackName;
    [SerializeField] string explain;
    private HackLine hackLineClone;
    private HackPanel expPanelClone;
    public void HackPointerEnter()
    {
        if (currentHackState == PlayerCore.HackState.NONHACK)
            return;
        hackLineClone = Instantiate(hackLinePrefab);
        expPanelClone = Instantiate(expPanelPrefab, inGameViewTransform);
        expPanelClone.gameObject.GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position) + new Vector2(0, 100);
        expPanelClone.SetHack(hackName, explain);
        hackLineClone.Init(transform.position, playerTransform.position);
    }

    public void HackPointExit()
    {
        if (hackLineClone == null)
            return;
        Destroy(hackLineClone.gameObject);
        Destroy(expPanelClone.gameObject);
    }

    public void SetHackMode(PlayerCore.HackState mode)
    {
        currentHackState = mode;
    }

    private Transform inGameViewTransform;
    public void Init(Transform player, Transform inGameViewTransform)
    {
        playerTransform = player;
        this.inGameViewTransform = inGameViewTransform;
    }

    public void Hack()
    {
        if (currentHackState == PlayerCore.HackState.NONHACK)
        {
            HackPointExit();
            return;
        }
        SetState(HumanEnemyState.Phone);
    }
    #endregion


    public void SetTarget(PlayerCore playerCore)
    {
        this.playerCore = playerCore;
    }

    private void Start()
    {
        Init();
        Bind();
    }

    public void SetState(HumanEnemyState enemyState)
    {
        enemyStateProp.Value = enemyState;
    }

    void Init()
    {
        enemyStateProp = new ReactiveProperty<HumanEnemyState>(HumanEnemyState.Move);
        lifeProp = new ReactiveProperty<int>(2);
        humanEnemyMove.Init();
    }

    void Bind()
    {
        //TODO向いている方向がおかしいかも
        enemyStateProp.Where(state => state == HumanEnemyState.Move)
            .Subscribe(_ =>
            {
                humanEnemyMove.StartPathAction();
            }).AddTo(this);

        enemyStateProp.Where(state => state == HumanEnemyState.Shot)
            .Subscribe(async _ =>
            {

                humanEnemyMove.CancelMove();
                humanEnemyAnimation.SetCanAnimFlag(false);
                humanEnemyAnimation.StopAnimation();
                humanEnemyAnimation.SetDirection(playerTransform.position - transform.position);
                try
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (playerTransform != null)
                            humanEnemyAttack.Shot(playerTransform.position);
                        await UniTask.Delay(750, cancellationToken: this.gameObject.GetCancellationTokenOnDestroy());
                    }
                    SetState(HumanEnemyState.Move);
                    humanEnemyAnimation.ReStartAnimation();
                    humanEnemyAnimation.SetCanAnimFlag(true);
                }
                catch (OperationCanceledException)
                {

                }
            }).AddTo(this);

        enemyStateProp.Where(state => state == HumanEnemyState.Phone)
            .Subscribe(async _ =>
            {
                searcher.gameObject.SetActive(false);
                centerLight.SetActive(true);
                humanEnemyMove.CancelMove();
                //TODO: TryCatchする必要あり?
                humanEnemyAnimation.TriggerPhoneAnime();//アニメーションをOnに
                try
                {
                    await UniTask.Delay(5000, cancellationToken: this.gameObject.GetCancellationTokenOnDestroy());//電話待機時間
                    humanEnemyAnimation.TriggerPhoneAnime();//アニメーションをOFFに
                    SetState(HumanEnemyState.Move);
                    searcher.gameObject.SetActive(true);
                    centerLight.SetActive(false);
                }
                catch(OperationCanceledException)
                {

                }
            }).AddTo(this);

        ///Searcherの当たり判定
        searcher.gameObject.OnTriggerEnter2DAsObservable().Subscribe(collision =>
        {
            if (collision.gameObject.tag == "Player")
            {
                Debug.Log("SearchPlayer");
                SetState(HumanEnemyState.Shot);
            }
        }).AddTo(this);

        //敵の当たり判定
        gameObject.OnTriggerEnter2DAsObservable().Subscribe(collision =>
        {
            if (collision.gameObject.tag == "Bullet")
            {
                lifeProp.Value--;
                Destroy(collision.gameObject);
            }
        }).AddTo(this);

        lifeProp.Subscribe(value =>
        {
            if (value == 1)
            {
                humanEnemyMove.DivideMoveSpeed(3);
                spriteRenderer.color = new Color32(202,98,98,255);
            }
            if (value <= 0)
            {
                Death();
            }
        }).AddTo(this);
    }

    public void Death()
    {
        Destroy(this.gameObject);
    }
}

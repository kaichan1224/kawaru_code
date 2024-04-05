using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UniRx.Triggers;
using UniRx;

public class Esqudo : MonoBehaviour,IHackable
{
    private PlayerCore.HackState currentHackState;
    private Transform playerTransform;
    #region HACKLINE
    [SerializeField] HackLine hackLinePrefab;
    [SerializeField] HackPanel expPanelPrefab;
    [SerializeField] string hackName;
    [SerializeField] string explain;
    [SerializeField] Collider2D col;
    [SerializeField] GameObject prevImage;
    private HackLine hackLineClone;
    private HackPanel expPanelClone;
    public void HackPointerEnter()
    {
        if (currentHackState == PlayerCore.HackState.NONHACK || !flag)
            return;
        hackLineClone = Instantiate(hackLinePrefab);
        expPanelClone = Instantiate(expPanelPrefab, inGameViewTransform);
        expPanelClone.gameObject.GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position) + new Vector2(0, 130);
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
    #endregion

    public void SetHackMode(PlayerCore.HackState mode)
    {
        currentHackState = mode;
    }

    private Transform inGameViewTransform;
    public void Init(Transform player, Transform inGameViewTransform)
    {
        playerTransform = player;
        this.inGameViewTransform = inGameViewTransform;
        col.gameObject.OnCollisionEnter2DAsObservable().Subscribe(collision =>
        {
            if (collision.gameObject.tag == "Enemy")
            {
                collision.gameObject.GetComponent<IEnemyCore>().Death();
                Destroy(this.gameObject);
            }
        }).AddTo(this.gameObject);
    }


    public void Hack()
    {
        if (currentHackState == PlayerCore.HackState.NONHACK || !flag)
        {
            HackPointExit();
            return;
        }
        Show(this.gameObject.GetCancellationTokenOnDestroy()).Forget();
    }

    public enum Direction
    { 
        Vertical,
        Horizon,
    }

    [SerializeField] private Direction direction;
    [SerializeField] private Sprite verticalSprite;
    [SerializeField] private Sprite horizontalSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private void OnValidate()
    {
        switch (direction)
        {
            case Direction.Vertical:
                spriteRenderer.sprite = verticalSprite;
                break;
            case Direction.Horizon: 
                spriteRenderer.sprite = horizontalSprite;
                break;
            default:
                break;
        }
    }
    private bool flag = true;
    [SerializeField] Transform maskTransform;
    [Header("表示時間")]
    [SerializeField] float time = 10f;
    private const float showY = 0.7f * 3;
    private const float hideY = 0.0f;
    private async UniTask Show(CancellationToken token)
    {
        flag = false;
        prevImage.SetActive(false);
        await maskTransform.DOMoveY(showY,0.5f).WithCancellation(token);
        col.isTrigger = false;
        await UniTask.Delay((int)(time * 1000),cancellationToken:token);
        await maskTransform.DOMoveY(hideY, 0.5f).WithCancellation(token);
        Destroy(this.gameObject);
    }

    
}

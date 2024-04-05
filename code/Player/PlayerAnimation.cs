using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private const string moveX = "moveX";
    private const string moveY = "moveY";
    private const string lastX = "lastX";
    private const string lastY = "lastY";
    private const string input = "input";
    private const string hack = "Hack";
    private const string shot = "Shot";
    private const string death = "Death";
    IInputEventProvider inputEventProvider;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] List<Sprite> deathImages;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        inputEventProvider = GetComponent<IInputEventProvider>();
    }

    private void Start()
    {
        Bind();
    }

    private bool flag = true;

    public void StopAnimation()
    {
        flag = false;
    }

    void Bind()
    {
        var core = GetComponent<PlayerCore>();
        inputEventProvider.MoveDirection.Where(_ => flag).Where(_ => core.HackState_ == PlayerCore.HackState.NONHACK).Subscribe(d =>
        {
            if (d == Vector2.zero)
            {
                animator.SetBool(input,false);
            }
            else
            {
                animator.SetBool(input, true);
                animator.SetFloat(moveX,d.x);
                animator.SetFloat(moveY,d.y);
                animator.SetFloat(lastX, d.x);
                animator.SetFloat(lastY, d.y);
            }
        }).AddTo(this);

        inputEventProvider.OnShot.Where(_ => core.HackStateProp.Value == PlayerCore.HackState.NONHACK).Subscribe(_ =>
        {
            animator.SetTrigger(shot);
        }).AddTo(this);
    }

    /// <summary>
    /// 死亡時アニメーション
    /// </summary>
    public async UniTask DeathAsync(CancellationToken token)
    {
        animator.SetTrigger(death);
        await UniTask.Delay(1500);
    }

    public void NextWalk()
    {
        animator.SetBool(input, true);
        animator.SetFloat(moveX,0);
        animator.SetFloat(moveY,1);
    }

    public void HackStartAnim()
    {
        animator.SetBool(input, false);
        animator.SetTrigger(hack);
        animator.SetFloat(lastX, -1);//終わったあとに左向かせるため
    }

    public void HackEndAnim()
    {
        animator.SetTrigger(hack);
    }
}

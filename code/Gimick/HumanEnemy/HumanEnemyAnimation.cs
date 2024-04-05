using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanEnemyAnimation : MonoBehaviour
{
    private Animator animator;
    private const string moveX = "MoveX";
    private const string moveY = "MoveY";
    private const string phone = "Phone";
    private Vector3 prevPos;
    [SerializeField] Transform lightTransform;
    private bool canAnimFlag = true;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector2 v = transform.position - prevPos;
        v.Normalize();
        if (canAnimFlag)
        {
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, v);
            lightTransform.rotation = rotation;
            animator.SetFloat(moveX, v.x);
            animator.SetFloat(moveY, v.y);
        }
        prevPos = transform.position;
    }

    /// <summary>
    /// プレイヤーと光の方向を決める
    /// </summary>
    /// <param name="v"></param>
    public void SetDirection(Vector2 v)
    {
        canAnimFlag = false;
        lightTransform.rotation = Quaternion.FromToRotation(Vector3.up, v);
        animator.SetFloat(moveX, v.x);
        animator.SetFloat(moveY, v.y);
    }

    public void SetCanAnimFlag(bool flag)
    {
        canAnimFlag = flag;
    }

    public void TriggerPhoneAnime()
    {
        animator.SetTrigger(phone);
    }

    public void StopAnimation()
    {
        animator.speed = 0;
    }

    public void ReStartAnimation()
    {
        animator.speed = 1f;
    }
}

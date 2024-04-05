using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animator animator;
    private const string moveX = "MoveX";
    private const string moveY = "MoveY";
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
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward,v);
        lightTransform.rotation = rotation;
        if (canAnimFlag)
        {
            animator.SetFloat(moveX, v.x);
            animator.SetFloat(moveY, v.y);
        }
        prevPos = transform.position;
    }

    public void SetDirection(Vector2 v)
    {
        canAnimFlag = false;
        animator.SetFloat(moveX, v.x);
        animator.SetFloat(moveY, v.y);
    }
}

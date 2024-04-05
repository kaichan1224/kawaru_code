using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
public class PlayerMove : MonoBehaviour
{
    IInputEventProvider inputEventProvider;
    [Header("移動速度")]
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    private bool canMove = true;
    public void SetCanMove(bool flag)
    {
        canMove = flag;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        inputEventProvider = GetComponent<IInputEventProvider>();
    }
    private void FixedUpdate()
    {
        if (canMove == false)
        {
            rb.velocity = Vector3.zero;
            return;
        }
        rb.velocity = inputEventProvider.MoveDirection.Value * speed; 
    }
}

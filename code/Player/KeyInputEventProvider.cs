using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System;
public class KeyInputEventProvider : MonoBehaviour,IInputEventProvider
{
    #region IInputEventProvider
    public IReadOnlyReactiveProperty<Vector2> MoveDirection => moveDirection;
    public IObservable<Unit> OnShot => shotSubject;
    public IObservable<Unit> OnHack => hackSubject;

    #endregion
    private ReactiveProperty<Vector2> moveDirection = new ReactiveProperty<Vector2>();
    private Subject<Unit> shotSubject = new Subject<Unit>();
    private Subject<Unit> hackSubject = new Subject<Unit>();

    private bool flag = true;
    public void Stop()
    {
        flag = false;
        moveDirection.Value = Vector2.zero;
    }
    private void Update()
    {
        if (!flag)
            return;
        var inputVector = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.A))
        {
            inputVector += new Vector2(-1, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVector += new Vector2(1, 0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            inputVector += new Vector2(0,1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVector += new Vector2(0,-1);
        }
        inputVector.Normalize();
        moveDirection.SetValueAndForceNotify(inputVector);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            hackSubject.OnNext(Unit.Default);
        }
        if (Input.GetMouseButtonDown(0))
        {
            shotSubject.OnNext(Unit.Default);
        }
    }

}

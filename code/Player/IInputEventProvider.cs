using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

/// <summary>
/// 入力を受け取るインターフェース
/// </summary>
public interface IInputEventProvider
{
    IReadOnlyReactiveProperty<Vector2> MoveDirection { get; }
    IObservable<Unit> OnShot { get; }
    IObservable<Unit> OnHack { get; }
    public void Stop();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHackable
{
    /// <summary>
    /// ハッキング可能なオブジェクトにアクションを起こす
    /// </summary>
    void Hack();

    /// <summary>
    /// 初期化メソッド
    /// </summary>
    void SetHackMode(PlayerCore.HackState mode);

    /// <summary>
    /// プレイヤー初期化
    /// </summary>
    public void Init(Transform player, Transform inGameViewTransform);

    void HackPointerEnter();

    void HackPointExit();
}

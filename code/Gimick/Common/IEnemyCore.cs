using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyCore
{
    public void SetTarget(PlayerCore player);
    public void Death();
}

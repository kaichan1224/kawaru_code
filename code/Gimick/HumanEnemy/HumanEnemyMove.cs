using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;


public class HumanEnemyMove : MonoBehaviour
{
    [Header("WayPoint")]
    [SerializeField] Transform[] positions;
    [SerializeField] private float moveSpeed;
    private Tween currentTween;
    private Vector3[] wayPoints;
    private float wayPointsDistance;
    public void DivideMoveSpeed(float value)
    {
        moveSpeed /= value;
    }
    public void Init()
    {
        var tmpwayPoints = new List<Vector3>();
        wayPointsDistance = 0;
        if (positions.Length == 0)
        {
            return;
        }
        var tmpPoint = positions[positions.Length - 1];
        foreach (var pos in positions)
        {
            tmpwayPoints.Add(pos.position);
            wayPointsDistance += (pos.position - tmpPoint.position).magnitude;
            tmpPoint = pos;
        }
        wayPoints = tmpwayPoints.ToArray();
        transform.position = positions[0].position;
    }

    public void StartPathAction()
    {
        if (positions.Length == 0)
        {
            return;
        }
        transform.DOPlay();
        transform.DOPath(
            wayPoints,
            wayPointsDistance / moveSpeed,
            PathType.Linear,
            gizmoColor: Color.red,
            pathMode: PathMode.TopDown2D
            )
            .SetEase(Ease.Linear)
            .SetOptions(true)
            .SetLoops(-1)
            .SetLink(this.gameObject);
    }

    public void CancelMove()
    {
        transform.DOPause();
    }
}

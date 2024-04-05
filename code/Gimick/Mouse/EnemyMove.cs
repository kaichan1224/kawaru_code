using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

public class EnemyMove : MonoBehaviour
{
    [Header("WayPoint")]
    [SerializeField] Transform[] positions;
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject searchLight2D;
    [SerializeField] GameObject attackLight2D;
    [Header("WayPoint")]
    [SerializeField] GameObject wayPointParent;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] EnemyAnimation enemyAnimation;
    private Transform target;
    private Vector3[] wayPoints;
    private float wayPointsDistance;
    private AStar aStar;
    private WayPoint[] routePoint;
    private Tween currentTween;
    private EnemyCore enemyCore;

    public void SetTarget(Transform target)
    { 
        this.target = target;
    }
    public void Init()
    {
        enemyCore = GetComponent<EnemyCore>();
        var tmpwayPoints = new List<Vector3>();
        wayPointsDistance = 0;
        var tmpPoint = positions[positions.Length-1];
        foreach (var pos in positions)
        {
            tmpwayPoints.Add(pos.position);
            wayPointsDistance += (pos.position - tmpPoint.position).magnitude;
            tmpPoint = pos;
        }
        wayPoints = tmpwayPoints.ToArray();
        _pointList = wayPointParent.GetComponentsInChildren<WayPoint>();
        //CheckRouteAsync(enemyCore.GetCancellationTokenOnDestroy()).Forget();
    }

    public void StartPathAction()
    {
        currentTween?.Kill();
        transform.position = positions[0].position;
        currentTween = transform.DOPath(
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

    public void InitAStar()
    {
        var startPoint = GetNearestWaypoint(transform);
        var endPoint = GetNearestWaypoint(target);
        aStar = new AStar(_pointList,startPoint,endPoint);
        aStar.SearchAll();
    }

    WayPoint[] _pointList;
    public void StartAstarMove()
    {
        currentTween?.Kill();
        InitAStar();
        routePoint = aStar.GetRoute();
        var tmpwayPoints = new List<Vector3>();
        var tmpDistance = 0f;
        var tmpPoint = routePoint[routePoint.Length - 1];
        foreach (var point in routePoint)
        {
            tmpwayPoints.Add(point.transform.position);
            tmpDistance += (point.transform.position - tmpPoint.transform.position).magnitude;
            tmpPoint = point;
        }
        var wayPoints = tmpwayPoints.ToArray();
        currentTween = transform.DOPath(
            wayPoints,
            tmpDistance / moveSpeed,
            PathType.Linear,
            gizmoColor: Color.red,
            pathMode: PathMode.TopDown2D
            )
            .SetEase(Ease.Linear)
            .SetOptions(false)
            .SetLink(this.gameObject)
            .OnComplete(() =>
            {
                enemyCore.SetState(EnemyCore.EnemyState.Attack);
            });
            
    }

    /*
    private async UniTask CheckRouteAsync(CancellationToken token)
    { 
        while(true)
        {
            if (transform == null)
                break;
            if (enemyCore.EnemyStateProp.Value != EnemyCore.EnemyState.Chase || !enemyCore.CanCheckFlag)
            {
                await UniTask.Delay(1000,cancellationToken:token);
                continue;
            }
            var startPoint = GetNearestWaypoint(transform);
            var endPoint = GetNearestWaypoint(target);
            var tmpaStar = new AStar(_pointList, startPoint, endPoint);
            tmpaStar.SearchAll();
            var tmproutePoint = tmpaStar.GetRoute();
            if (ListsAreEqual(tmproutePoint, routePoint) == false)
            {
                currentTween?.Kill();
                StartAstarMove();
            }
            await UniTask.Delay(2000,cancellationToken:token);
        }
    }
    */

    bool ListsAreEqual(WayPoint[] list1, WayPoint[] list2)
    {
        if (list1.Length != list2.Length)
        {
            return false;
        }

        for (int i = 0; i < list1.Length; i++)
        {
            if (!list1[i].Equals(list2[i]))
            {
                return false;
            }
        }

        return true;
    }

    private WayPoint GetNearestWaypoint(Transform transform)
    {
        var minDistance = 10000f;
        var minIdx = 0;
        for(int i=0;i<_pointList.Length;i++)
        {
            if (transform == null)
                break;
            var dis = (_pointList[i].transform.position - transform.position).magnitude;
            if (dis < minDistance)
            {
                minIdx = i;
                minDistance = dis;
            }
        }
        return _pointList[minIdx];
    }

    public void StopAction()
    {
        currentTween?.Kill();
    }

    public void BackToRoute()
    {
        currentTween?.Kill();
        var startPoint = GetNearestWaypoint(transform);
        var endPoint = GetNearestWaypoint(positions[0]);
        aStar = new AStar(_pointList, startPoint, endPoint);
        aStar.SearchAll();
        routePoint = aStar.GetRoute();
        var tmpwayPoints = new List<Vector3>();
        var tmpDistance = 0f;
        var tmpPoint = routePoint[routePoint.Length - 1];
        foreach (var point in routePoint)
        {
            tmpwayPoints.Add(point.transform.position);
            tmpDistance += (point.transform.position - tmpPoint.transform.position).magnitude;
            tmpPoint = point;
        }
        var wayPoints = tmpwayPoints.ToArray();
        currentTween = transform.DOPath(
            wayPoints,
            tmpDistance / moveSpeed,
            PathType.Linear,
            gizmoColor: Color.red,
            pathMode: PathMode.TopDown2D
            )
            .SetEase(Ease.Linear)
            .SetOptions(false)
            .SetLink(this.gameObject)
            .OnComplete(()=>
            {
                enemyCore.SetState(EnemyCore.EnemyState.MoveAround);
            });
        currentTween.Play();
    }

    public async UniTask AttackMove(CancellationToken token)
    {
        if (target == null)
            return;
        Vector2 dir = (target.position - transform.position).normalized;
        enemyAnimation.SetDirection(dir);
        searchLight2D.SetActive(false);
        attackLight2D.SetActive(true);
        currentTween?.Kill();
        spriteRenderer.color = Color.red;
        await UniTask.Delay(100, cancellationToken: token);
        rb.velocity = dir * moveSpeed * 2;
    }
}

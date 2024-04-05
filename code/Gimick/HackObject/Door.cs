using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using UnityEngine;
using VContainer;

public class Door : MonoBehaviour,IHackable
{
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
    [SerializeField] Direction targetDirection;
    [Header("アクション前にアクティブなWayPoint")]
    [SerializeField] List<WayPoint> ActiveWayPoints;
    [Header("アクション前に非アクティブなWayPoint")]
    [SerializeField] List<WayPoint> InActiveWayPoints;
    private PlayerCore.HackState currentHackState;
    private Transform playerTransform;
    private Dictionary<Direction,Vector3>  dict = new Dictionary<Direction,Vector3>() {
        {Direction.Up,new Vector3(0,0,0)},
        {Direction.Down,new Vector3(0,0,270)},
        {Direction.Right,new Vector3(0,0,90)},
        {Direction.Left,new Vector3(0,0,180)},
    };

    #region HACKLINE
    [SerializeField] HackLine hackLinePrefab;
    [SerializeField] HackPanel expPanelPrefab;
    [SerializeField] string hackName;
    [SerializeField] string explain;
    private HackLine hackLineClone;
    private HackPanel expPanelClone;
    public void HackPointerEnter()
    {
        if (currentHackState == PlayerCore.HackState.NONHACK || flag)
            return;
        hackLineClone = Instantiate(hackLinePrefab);
        expPanelClone = Instantiate(expPanelPrefab, inGameViewTransform);
        expPanelClone.gameObject.GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position) + new Vector2(0, 130);
        expPanelClone.SetHack(hackName, explain);
        hackLineClone.Init(transform.position, playerTransform.position);
    }

    public void HackPointExit()
    {
        if (hackLineClone == null)
            return;
        Destroy(hackLineClone.gameObject);
        Destroy(expPanelClone.gameObject);
    }
    #endregion

    public void SetHackMode(PlayerCore.HackState mode)
    {
        currentHackState = mode;
    }

    private Transform inGameViewTransform;
    public void Init(Transform player, Transform inGameViewTransform)
    {
        playerTransform = player;
        this.inGameViewTransform = inGameViewTransform;
    }

    [SerializeField] Transform maskTransform;
    [SerializeField] Collider2D col;
    private const float targetY = 0f;

    public void Hack()
    {
        if (currentHackState == PlayerCore.HackState.NONHACK || flag)
        {
            HackPointExit();
            return;
        }
        Open(this.gameObject.GetCancellationTokenOnDestroy()).Forget();
    }
    private bool flag = false;
    [Header("表示時間")]
    [SerializeField] float time = 10f;
    private async UniTask Open(CancellationToken token)
    {
        await maskTransform.DOLocalMoveY(targetY, 0.5f).WithCancellation(token);
        col.isTrigger = true;
        flag = true;
    }
}

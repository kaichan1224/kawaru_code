using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Laser : MonoBehaviour,IHackable
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform shotPosition;
    [SerializeField] float lazerDistance = 10f;
    [SerializeField] float lineWidth = 0.01f;
    private Transform playerTransform;
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    private PlayerCore.HackState currentHackState;
    private bool isOnLaser = true;
    [SerializeField] Direction targetDirection;
    private Dictionary<Direction, Vector2> dict = new Dictionary<Direction, Vector2>() {
        {Direction.Up,new Vector2(0,1)},
        {Direction.Down,new Vector2(0,-1)},
        {Direction.Right,new Vector2(1,0)},
        {Direction.Left,new Vector2(-1,0)},
    };

    private Transform inGameViewTransform;
    #region HACKLINE
    [SerializeField] HackLine hackLinePrefab;
    [SerializeField] HackPanel expPanelPrefab;
    [SerializeField] string hackName;
    [SerializeField] string explain;
    private HackLine hackLineClone;
    private HackPanel expPanelClone;
    public void HackPointerEnter()
    {
        if (currentHackState == PlayerCore.HackState.NONHACK)
            return;
        hackLineClone = Instantiate(hackLinePrefab);
        expPanelClone = Instantiate(expPanelPrefab, inGameViewTransform);
        expPanelClone.gameObject.GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position) + new Vector2(0, 100);
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

    private void OnValidate()
    {
        switch (targetDirection)
        {
            case Direction.Up:
                transform.eulerAngles = (new Vector3(0, 0, 270));
                break;
            case Direction.Right:
                transform.eulerAngles = (new Vector3(0, 0, 180));
                break;
            case Direction.Down:
                transform.eulerAngles = (new Vector3(0, 0, 90));
                break;
            case Direction.Left:
                transform.eulerAngles = (new Vector3(0, 0, 0));
                break;
            default:
                break;
        }
    }

    public void Init(Transform player, Transform inGameViewTransform)
    {
        playerTransform = player;
        this.inGameViewTransform = inGameViewTransform;
    }

    public void SetHackMode(PlayerCore.HackState mode)
    {
        currentHackState = mode;
    }

    private CancellationTokenSource hackCancell = new CancellationTokenSource();
    void HackCancell()
    {
        hackCancell?.Cancel();
        hackCancell?.Dispose();
        hackCancell = null;
        hackCancell = new CancellationTokenSource();
    }

    public async void Hack()
    {
        HackCancell();
        if (currentHackState == PlayerCore.HackState.NONHACK)
        {
            HackPointExit();
            return;
        }
        Debug.Log("Hack!");
        if (isOnLaser)
        {
            isOnLaser = false;
            try
            {
                await UniTask.Delay(7000, cancellationToken: hackCancell.Token);
                isOnLaser = true;
            }
            catch
            {

            }
        }
        else
        {
            isOnLaser = true;
        }
    }

    private void RazerOn()
    {
        isOnLaser = true;
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
    }

    private void Reset()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
    }

    private void Update()
    {
        if (isOnLaser)
        {
            lineRenderer.positionCount = 2;
            OnRay();
        }
        else
        { 
            lineRenderer.positionCount = 0;
        }
    }

    private void OnRay()
    {
        Vector2 startPos = shotPosition.transform.position;
        Ray2D ray = new Ray2D(startPos, dict[targetDirection]);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin,ray.direction);
        lineRenderer.SetPosition(0, startPos);
        if (hit.collider)
        {
            if (hit.collider.gameObject.tag == "Wall")
            {
                //Debug.DrawRay(ray.origin, hit.point - ray.origin, Color.red);
                lineRenderer.SetPosition(1, hit.point);
            }
            else if (hit.collider.gameObject.tag == "Player")
            {
                //プレイヤに当たったら死亡通知送る
                hit.collider.gameObject.GetComponent<PlayerCollision>().PublishDeathSubject();
            }
        }
        else
        {
            Vector2 tmp = dict[targetDirection] * lazerDistance;
            //lineRenderer.SetPosition(1, startPos + tmp);
            //Debug.DrawRay(ray.origin,tmp, Color.red);
        }
    }
}

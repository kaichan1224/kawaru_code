using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static EnemyCore;

public class Car : MonoBehaviour,IHackable
{
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
    [SerializeField] Direction targetDirection;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] List<Sprite> rights;
    [SerializeField] List<Sprite> lefts;
    [SerializeField] List<Sprite> ups;
    [SerializeField] List<Sprite> downs;
    private Dictionary<Direction, Vector2> dict = new Dictionary<Direction, Vector2>() {
        {Direction.Up,new Vector2(0,1)},
        {Direction.Down,new Vector2(0,-1)},
        {Direction.Right,new Vector2(1,0)},
        {Direction.Left,new Vector2(-1,0)},
    };
    private Transform playerTransform;
    private PlayerCore.HackState currentHackState;
    private int uIndex = 0;
    private int lIndex = 0;
    private int rIndex = 0;
    private int dIndex = 0;
    private bool flag = false;
    private const float SpriteChangeIntervalTime = 0.1f;
    private float SpriteChangeTimer = 0;
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
        expPanelClone.SetHack(hackName,explain);
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

    private void SetSpeed()
    {
        rb.velocity = dict[targetDirection] * moveSpeed;
        Debug.Log(rb.velocity);
        flag = true;
        AudioManager.instance.PlaySE(AudioData.SEName.BrakeSE);
    }

    private void OnValidate()
    {
        SetDirection();
    }

    private void SetDirection()
    {
        switch (targetDirection)
        {
            case Direction.Up:
                spriteRenderer.sprite = ups[0];
                break;
            case Direction.Right:
                spriteRenderer.sprite = rights[0];
                break;
            case Direction.Left:
                spriteRenderer.sprite = lefts[0];
                break;
            case Direction.Down:
                spriteRenderer.sprite = downs[0];
                break;
        }
    }

    private void Update()
    {
        if (!flag)
            return;
        ViewUpdate(Time.deltaTime);
    }

    private void ViewUpdate(float deltaTime)
    {

        switch (targetDirection)
        {
            case Direction.Up:
                SpriteChangeTimer -= deltaTime;
                if (SpriteChangeTimer < 0f)
                {
                    SpriteChangeTimer += SpriteChangeIntervalTime;
                    uIndex = uIndex.Repeat(1, ups.Count);
                    spriteRenderer.sprite = ups[uIndex];
                }
                break;
            case Direction.Down:
                SpriteChangeTimer -= deltaTime;
                if (SpriteChangeTimer < 0f)
                {
                    SpriteChangeTimer += SpriteChangeIntervalTime;
                    dIndex = dIndex.Repeat(1, downs.Count);
                    spriteRenderer.sprite = downs[dIndex];
                }
                break;
            case Direction.Left:
                SpriteChangeTimer -= deltaTime;
                if (SpriteChangeTimer < 0f)
                {
                    SpriteChangeTimer += SpriteChangeIntervalTime;
                    lIndex = lIndex.Repeat(1, lefts.Count);
                    spriteRenderer.sprite = lefts[lIndex];
                }
                break;
            case Direction.Right:
                SpriteChangeTimer -= deltaTime;
                if (SpriteChangeTimer < 0f)
                {
                    SpriteChangeTimer += SpriteChangeIntervalTime;
                    rIndex = rIndex.Repeat(1, rights.Count);
                    spriteRenderer.sprite = rights[rIndex];
                }
                break;
        }
    }

    public void Hack()
    {
        if (currentHackState == PlayerCore.HackState.NONHACK)
        {
            HackPointExit();
            return;
        }
        Debug.Log("速度設定");
        SetSpeed();
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("何かに触れた1");
        if (collision.gameObject.tag == "Wall")
        {
            Debug.Log("壁に触れた");
            flag = false;
            rb.velocity = Vector2.zero;
        }
        if (collision.gameObject.tag == "Player" && flag)
        {
            collision.gameObject.GetComponent<PlayerCollision>().PublishDeathSubject();
        }
        if (collision.gameObject.tag == "Enemy")
        {
            flag = false;
            collision.gameObject.GetComponent<IEnemyCore>().Death();
        }
    }
}


public static class IntExtensions
{
    public static int Repeat(this int self, int value, int max)
    {
        return (self + value + max) % max;
    }
}
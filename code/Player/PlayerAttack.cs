using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 3f;
    private ReactiveProperty<int> bulletCount = new ReactiveProperty<int>();
    public IReadOnlyReactiveProperty<int> BulletCount => bulletCount;
    const int shotInterval = 200;
    IInputEventProvider inputEventProvider;
    private bool canShot = true;
    public void SetCanShot(bool flag)
    {
        canShot = flag;
    }
    public void SetBulletCounut(int count)
    { 
        bulletCount.Value = count;
    }
    void Start()
    {
        inputEventProvider = GetComponent<IInputEventProvider>();
        Bind();
    }

    void Bind()
    {
        var core = GetComponent<PlayerCore>();
        inputEventProvider.OnShot.Where(_ => bulletCount.Value > 0).Subscribe(async _ =>
        {
            try
            {
                await UniTask.Delay(shotInterval, cancellationToken: this.GetCancellationTokenOnDestroy());
                Shot(core.Direction_);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("キャンセルされました");
            }
        }).AddTo(this.gameObject);
    }

    void Shot(PlayerCore.Direction direction)
    {
        if (!canShot)
            return;
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        AudioManager.instance.PlaySE(AudioData.SEName.ShotSE);
        bulletCount.Value--;
        switch (direction)
        {
            case PlayerCore.Direction.Up:
                bullet.GetComponent<Rigidbody2D>().velocity = transform.up * bulletSpeed;
                bullet.transform.Rotate(new Vector3(0, 0, 90));
                break;
            case PlayerCore.Direction.Down:
                bullet.GetComponent<Rigidbody2D>().velocity = -transform.up * bulletSpeed;
                bullet.transform.Rotate(-transform.up);
                bullet.transform.Rotate(new Vector3(0, 0, 270));
                break;
            case PlayerCore.Direction.Right:
                bullet.GetComponent<Rigidbody2D>().velocity = transform.right * bulletSpeed;
                bullet.transform.Rotate(transform.right);
                break;
            case PlayerCore.Direction.Left:
                bullet.GetComponent<Rigidbody2D>().velocity = -transform.right * bulletSpeed;
                bullet.transform.Rotate(new Vector3(0,0,180));
                break;
        }
    }
}

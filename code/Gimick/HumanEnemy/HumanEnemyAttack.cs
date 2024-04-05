using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanEnemyAttack : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 3f;
    public void Shot(Vector3 target)
    {
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        AudioManager.instance.PlaySE(AudioData.SEName.ShotSE);
        bullet.GetComponent<Rigidbody2D>().velocity = (target - transform.position).normalized * bulletSpeed;
        bullet.transform.Rotate(target - transform.position);
    }
}

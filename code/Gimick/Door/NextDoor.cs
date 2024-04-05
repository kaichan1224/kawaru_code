using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class NextDoor : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] openSprites;
    [SerializeField] Collider2D col;
    public async UniTask OpenDoor(CancellationToken token)
    {
        col.enabled = false;
        for (int i = 0; i < openSprites.Length; i++)
        {
            spriteRenderer.sprite = openSprites[i];
            await UniTask.Delay(100,cancellationToken:token);
        }
        spriteRenderer.sortingOrder = 3;
    }

    public async UniTask CloseDoor(CancellationToken token)
    {
        for (int i = openSprites.Length-1; i >= 0; i--)
        {
            spriteRenderer.sprite = openSprites[i];
            await UniTask.Delay(100, cancellationToken: token);
        }
    }
}

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class WayPoint : MonoBehaviour
{
    [SerializeField] List<WayPoint> _relations;
    // 通行可能かどうか
    [SerializeField] bool _canMove = true;// Start is called before the first frame update

    // 直前のノード隣接ノード状態を記録するリスト
    readonly List<WayPoint> _previousList = new List<WayPoint>();

    // Props

    // 移動可能な隣接ノードのリストを取得する
    public List<WayPoint> Rerations => _relations;

    // この地点が移動可能かどうかを取得します。
    public bool CanMove => _canMove;

    // Rintime impl

    private void Awake()
    {
        SynchronizeRelationsToPrevious();
    }
#if UNITY_EDITOR
    public async void OnValidate()
    {
        if (_relations is null || _previousList is null)
        {
            return;
        }
        // 相手に自分を追加する(復路登録)
        foreach (var p in _relations.Except(_previousList))
        {
            //Log.Trace(p.name);
            if (!p.Rerations.Contains(this))
            {
                p.Rerations.Add(this);
                p.SynchronizeRelationsToPrevious();
            }
        }

        if (_relations.Count < _previousList.Count)
        {
            foreach (var p in _previousList.Except(_relations))
            {
                p.Rerations.Remove(this); // 相手から自分を削除
                p.SynchronizeRelationsToPrevious();
            }
        }
        //待機しないと、リストに追加できなくなる
        await UniTask.Delay(5000);
        //重複してたら消す
        _relations = _relations.Distinct().ToList();
        SynchronizeRelationsToPrevious();
    }
#endif
    public void SynchronizeRelationsToPrevious()
    {
        _previousList.Clear();
        foreach (var p in _relations)
        {
            _previousList.Add(p);
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_relations is null || _relations.Count == 0)
        {
            return;
        }

        foreach (var next in _relations)
        {
            //if (next is null) continue;
            // ポイント間に線を引く
            Gizmos.color = new Color(1f, 0f, 0, 0.3f);
            Gizmos.DrawLine(transform.position, next.transform.position);
        }
    }
#endif
}

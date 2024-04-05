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
    // �ʍs�\���ǂ���
    [SerializeField] bool _canMove = true;// Start is called before the first frame update

    // ���O�̃m�[�h�אڃm�[�h��Ԃ��L�^���郊�X�g
    readonly List<WayPoint> _previousList = new List<WayPoint>();

    // Props

    // �ړ��\�ȗאڃm�[�h�̃��X�g���擾����
    public List<WayPoint> Rerations => _relations;

    // ���̒n�_���ړ��\���ǂ������擾���܂��B
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
        // ����Ɏ�����ǉ�����(���H�o�^)
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
                p.Rerations.Remove(this); // ���肩�玩�����폜
                p.SynchronizeRelationsToPrevious();
            }
        }
        //�ҋ@���Ȃ��ƁA���X�g�ɒǉ��ł��Ȃ��Ȃ�
        await UniTask.Delay(5000);
        //�d�����Ă������
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
            // �|�C���g�Ԃɐ�������
            Gizmos.color = new Color(1f, 0f, 0, 0.3f);
            Gizmos.DrawLine(transform.position, next.transform.position);
        }
    }
#endif
}

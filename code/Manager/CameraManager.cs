using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] Transform _target;
    private const int distance = -10;
    public void SetTarget(Transform target)
    {
        _target = target;
    }

    void Update()
    {
        if (_target == null)
            return;
        transform.position = new Vector3(_target.position.x,_target.position.y,distance);
    }
}

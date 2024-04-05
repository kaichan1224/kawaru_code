using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class Searcher : MonoBehaviour
{
    [SerializeField] Light2D light2D;
    [SerializeField] Collider2D _collider;

    public void InActiveCollider()
    {
        _collider.enabled = false;
    }

    public void ActiveDengerLight()
    {
        light2D.color = Color.red;
    }

    public void InActiveDengerLight()
    {
        light2D.color = Color.white;
    }
}

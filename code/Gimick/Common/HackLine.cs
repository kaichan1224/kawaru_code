using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackLine : MonoBehaviour
{
    [SerializeField] LineRenderer line;
    public void Init(Vector2 from,Vector2 to)
    {
        line.startWidth = 0.1f;
        line.SetPosition(0,from);
        line.SetPosition(1, to-new Vector2(0.1f,0.3f));
    }
}

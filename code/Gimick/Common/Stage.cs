using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StageManager;

public class Stage : MonoBehaviour
{
    public enum StageName
    {
        Stage1,
        Stage2,
        Stage3,
        Stage4,
        Stage5,
    }
    public StageName Stagename;
    public GameObject wayPoints;
    public GameObject enemyParent;
    public GameObject hackTargets;
    public Transform firstPoint;
}

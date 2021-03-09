using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "PluggableAI/New EnemyStats file")]
public class EnemyStats : ScriptableObject
{
    public float moveSpeed = 1;
    public float rotationSpeed = 1;

    [MinMaxRange(0, 4)]
    public RangedFloat shortWaitInterval = new RangedFloat();

    [MinMaxRange(0, 8)]
    public RangedFloat longWaitInterval = new RangedFloat();

    [MinMaxRange(0, 120)]
    public RangedFloat randomWaitInterval = new RangedFloat();



}

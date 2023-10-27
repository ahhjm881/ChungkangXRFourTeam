using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="XR/Enemy/EnemyData", fileName="EnemyData ", order = 3)]
public class EnemyData : ScriptableObject
{
    [SerializeField] private float _hp;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _SleepDecisionTime;
    [SerializeField] private float _propagationForce;
    [SerializeField] private int _propagationCount;
    [SerializeField] private Color _flameColor;
    [SerializeField] private Color _waterColor;

    public float SleepDecisionTime => _SleepDecisionTime;   
    public float PropagationForce => _propagationForce;


    public float Hp => _hp;
    public Color FlameColor => _flameColor;

    public Color WaterColor => _waterColor;

    public float MovementSpeed => _movementSpeed;
    public int PropagationCount => _propagationCount;
}
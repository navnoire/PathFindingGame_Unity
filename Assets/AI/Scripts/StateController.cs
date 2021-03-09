using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour
{
    public PuzzleManager puzzleMgr;

    public EnemyType type;
    public State currentState;
    public State startState;
    public State remainInState;
    public EnemyStats stats;
    public float updateInterval = 1f;
    public Item targetItem;
    //public bool showStateColor;

    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Vector3 targetCoords;
    [HideInInspector] public Vector3 prevCoords;
    [HideInInspector] public SimpleTimer timer;
    [HideInInspector] public bool rotateInProcess;
    [HideInInspector] public bool aiActive;

    MaterialPropertyBlock colorValue;
    Renderer m_renderer;
    int colorID;
    int materialIndex;

    private void Awake()
    {
        puzzleMgr = FindObjectOfType<PuzzleManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        timer = GetComponent<SimpleTimer>();
        m_renderer = GetComponentInChildren<Renderer>();
        colorValue = new MaterialPropertyBlock();

        if (type == EnemyType.robber)
        {
            navMeshAgent.updateRotation = false;
        }
    }

    void OnEnable()
    {
        currentState = startState;
        SetupAI();

    }

    private void OnDisable()
    {
        currentState = startState;
        timer.ResetTimer();
        if (RotateSmoothly() != null)
        {
            StopCoroutine(RotateSmoothly());
            rotateInProcess = false;
        }
    }

    //private void Start()
    //{
    //if (showStateColor)
    //{
    //    colorID = Shader.PropertyToID("_Color");
    //    switch (type)
    //    {
    //        case EnemyType.rotator:
    //            materialIndex = 1;
    //            break;
    //        case EnemyType.robber:
    //            materialIndex = 0;
    //            break;
    //    }
    //}
    //}

    public void SetupAI()
    {
        aiActive = true;

        prevCoords = transform.position;
        StartCoroutine(UpdateState());

    }

    public void TransitionTostate(State nextState)
    {
        if (nextState != remainInState)
        {
            currentState = nextState;
            //if (showStateColor)
            //{
            //    colorValue.SetColor(colorID, currentState.stateColor);
            //    m_renderer.SetPropertyBlock(colorValue, materialIndex);
            //}
        }
    }


    private IEnumerator UpdateState()
    {
        while (aiActive)
        {
            currentState.UpdateState(this);
            yield return new WaitForSeconds(updateInterval);
        }
    }

    public IEnumerator RotateSmoothly()
    {
        Vector3 targetDir = (targetCoords - transform.position).normalized;
        var targetAngle = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
        Vector3 rotation = targetAngle * Vector3.up;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.y, rotation.y)) > .5f)
        {
            transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotation), navMeshAgent.angularSpeed * Time.deltaTime);
            yield return null;
        }

        rotateInProcess = false;
        yield return null;
    }

}

public enum EnemyType
{
    empty = -1,
    rotator = 9,
    robber = 10

}

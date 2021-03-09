using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carriage : MonoBehaviour
{
    public Transform pocket;
    public Transform target;
    public int index;

    public static List<Vector3> currentPath;
    public static float moveSpeed;
    public static float rotationSpeed;
    static int animSpeedID;
    static int collectedStateID;
    static int brakeID;

    public Vector3 currentTargetPosition;
    public float moveThreshold = .5f;

    public bool isReachedOrigin;
    public bool isReachedTarget;

    Animator carriageAnimator;

    float speedSmoothVelocity;
    float speedSmoothTime = .1f;

    private void Awake()
    {
        carriageAnimator = GetComponentInChildren<Animator>();

        if (animSpeedID == 0)
        {
            animSpeedID = Animator.StringToHash("speed_factor");
            collectedStateID = Animator.StringToHash("JumpOut");
        }

    }


    public void MoveToTarget()
    {
        if (FollowPath() != null)
        {
            StopCoroutine(FollowPath());
        }
        StartCoroutine(FollowPath());
    }


    public IEnumerator FollowPath()
    {

        while (currentPath.Count > 0)
        {
            currentTargetPosition = currentPath[index];

            while (Vector3.Distance(transform.position, currentTargetPosition) > moveThreshold)
            {
                if (isReachedTarget || isReachedOrigin)
                {
                    //var speed = carriageAnimator.GetFloat(animSpeedID);
                    carriageAnimator.SetFloat(animSpeedID, 0);
                }
                else
                {
                    Vector3 targetDir = (currentTargetPosition - transform.position).normalized;
                    var targetAngle = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
                    Vector3 rotation = targetAngle * Vector3.up;
                    float damper = Vector3.Distance(transform.position, target.position);

                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotation), rotationSpeed * Time.deltaTime);
                    transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed * damper, Space.Self);
                    carriageAnimator.SetFloat(animSpeedID, moveSpeed * damper);
                }
                yield return null;
            }
            var currSpeed = carriageAnimator.GetFloat(animSpeedID);
            carriageAnimator.SetFloat(animSpeedID, SmoothBrake(currSpeed, currSpeed / Random.Range(1.5f, 2f)));
            yield return null;
        }

    }

    public void IsReachedOrigin(bool b)
    {
        isReachedOrigin = b;
    }

    public void CarriageCollect()
    {
        carriageAnimator.Play(collectedStateID);
        Destroy(gameObject, carriageAnimator.GetCurrentAnimatorStateInfo(0).length + .5f);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.transform == target)
        {
            isReachedTarget = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform == target)
        {
            isReachedTarget = false;
        }
    }

    private float SmoothBrake(float currentSpeed, float targetSpeed)
    {
        return Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
    }


}


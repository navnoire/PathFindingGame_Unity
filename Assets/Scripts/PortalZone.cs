using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PortalZone : MonoBehaviour
{
    Transform transgressor;

    public static event Action OnRoomExit;

    void Start()
    {
        transgressor = transform.GetChild(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (Vector3.Dot(transform.up, other.transform.parent.forward) < 0)
            {
                other.gameObject.GetComponentInParent<Player>().CurrentPath.Add(new Vector2Int(Mathf.RoundToInt(transgressor.position.x), Mathf.CeilToInt(transgressor.position.z)));
                OnRoomExit?.Invoke();
            }
        }


    }
}

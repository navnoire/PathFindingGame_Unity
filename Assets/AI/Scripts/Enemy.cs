using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    public Transform m_spawnPoint;

    [HideInInspector] public GameObject m_instance;

    private StateController m_stateController;

    private void OnEnable()
    {
        m_stateController = GetComponent<StateController>();
    }

    private void OnDisable()
    {
        switch (m_stateController.type)
        {
            case EnemyType.robber:
                if (m_stateController.targetItem != null)
                {
                    m_stateController.targetItem.transform.parent = GeneralPooler.Instance.poolHolder;
                    m_stateController.targetItem = null;
                }
                break;
            case EnemyType.rotator:

                break;
        }

    }

    // TODO Реализовать на андроиде со счетчиком кликов (если это вобще нужно)
    //void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    //{
    //    if (m_stateController.type == EnemyType.robber && eventData.clickCount == 3)
    //    {
    //        print("Triple click on " + m_instance.name);
    //        m_stateController.targetItem.transform.parent = null;
    //        m_stateController.targetItem.coords = new SerializableVector2Int(Mathf.RoundToInt(m_stateController.targetItem.transform.position.x),
    //                                                                         Mathf.RoundToInt(m_stateController.targetItem.transform.position.z));
    //        m_stateController.targetItem = null;
    //    }
    //}
}

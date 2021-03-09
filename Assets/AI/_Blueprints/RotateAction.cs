using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/RotateTile")]
public class RotateAction : ActionBase
{
    public override void Act(StateController controller)
    {
        RaycastHit hit;
        Tile target = null;
        if (Physics.Raycast(controller.transform.position, -controller.transform.up, out hit, 1f) && hit.collider.CompareTag("Tile"))
        {
            target = hit.collider.gameObject.GetComponent<Tile>();
        }

        if (target != null && target.isRotatable)
        {
            target.RotateTile();
        }


    }

}

using UnityEngine;


public class TriggerInteraction : ColliderInteractionBase
{
    protected override bool ValidateColliders()
    {
        if (!FindOtherCollider())
        {
            return false;
        }

        if (m_thisCollider.isTrigger)
        {
            return true;
        }

        foreach (var targetCollider in m_targetColliders)
        {
            if (targetCollider.isTrigger)
            {
                return true;
            }
        }

        return false;
    }


    private void OnTriggerEnter(Collider other)
    {
        Enter(other);
    }


    private void OnTriggerStay(Collider other)
    {
        Stay(other);
    }


    private void OnTriggerExit(Collider other)
    {
        Exit(other);
    }
}
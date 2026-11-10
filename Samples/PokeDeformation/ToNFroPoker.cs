using UnityEngine;

/// <summary>
///     Moving TO the target (the OffsetObject), and moving back with the moveAmount AWAY from the target.
/// </summary>
public class ToNFroPoker : MonoBehaviour
{
    [SerializeField]
    private float m_speedSlider;

    [Tooltip("If null, uses parent object")]
    [SerializeField]
    private GameObject m_offsetObject;

    [Tooltip("Amount to move BACK")]
    [SerializeField]
    private Vector3 m_maxWithdrawal = new(0, 0, 0.68f);

    [Tooltip("Will need to investigate how length affects this all.")]
    [SerializeField]
    private float m_length = 1f;

    private void Awake()
    {
        if (m_offsetObject == null)
        {
            m_offsetObject = gameObject.transform.parent.gameObject;
        }
    }

    private void LateUpdate()
    {
        var speed = m_speedSlider;
        var step = speed * Time.deltaTime;

        // Move this transform between the target and the (target - moveAmount) positions
        // once hitting the target, it moves back to (target - moveamount) and vice versa
        Vector3 targetPosition = m_offsetObject.transform.position;
        Vector3 furthestPosition = targetPosition - m_maxWithdrawal;

        // Calculate the ping-pong position
        var pingPong = Mathf.PingPong(step, m_length);
        transform.position = Vector3.Lerp(furthestPosition, targetPosition, pingPong);
    }
}

using ScriptableObjectArchitecture;
using UnityEngine;


/// <summary>
///     Moving TO the target (the OffsetObject), and moving back with the moveAmount AWAY from the target.
/// </summary>
public class ToNFroPoker : MonoBehaviour
{
    public FloatVariable speed_slider;
    [Tooltip("If null, uses parent object")]
    public GameObject offsetObject;
    [Tooltip("Amount to move BACK")]
    public Vector3 MaxWithdrawal = new(0, 0, 0.68f);
    [Tooltip("Will need to investigate how length affects this all.")]
    public float length = 1f;


    private void Awake()
    {
        if (offsetObject == null)
        {
            offsetObject = gameObject.transform.parent.gameObject;
        }
    }


    private void LateUpdate()
    {
        var speed = speed_slider.Value;
        var step = speed * Time.deltaTime;

        // Move this transform between the target and the (target - moveAmount) positions
        // once hitting the target, it moves back to (target - moveamount) and vice versa
        var targetPosition = offsetObject.transform.position;
        var furthestPosition = targetPosition - MaxWithdrawal;

        // Calculate the ping-pong position
        var pingPong = Mathf.PingPong(Time.time * speed, length);
        transform.position = Vector3.Lerp(furthestPosition, targetPosition, pingPong);
    }
}
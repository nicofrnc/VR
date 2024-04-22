using UnityEngine;
using UnityEngine.Events;

public class DoorSound : MonoBehaviour
{
    private HingeJoint hingeJoint;
    [SerializeField] float doorOpenAngle = 1f;
    [SerializeField] hingeJointDefaultState hingeJointDefault = hingeJointDefaultState.closed;

    enum hingeJointDefaultState { open, closed }
    bool isDoorOpen, isDoorClosed = false;

    [SerializeField] UnityEvent OnDoorOpenEvent;
    [SerializeField] UnityEvent OnDoorCloseEvent;

    // Start is called before the first frame update
    void Start()
    {
        hingeJoint = GetComponent<HingeJoint>();
        if (hingeJointDefault == hingeJointDefaultState.closed)
        {
            isDoorClosed = true;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (hingeJoint.angle > doorOpenAngle)
        {
            if (isDoorOpen == false)
            {
                Debug.Log("DoorOpen");
                OnDoorCloseEvent.Invoke();
                isDoorOpen = true;
                isDoorClosed = false;
            }
        }
        else if (hingeJoint.angle < doorOpenAngle)
        {
            if (isDoorClosed == false)
            {
                Debug.Log("DoorClosed");
                OnDoorOpenEvent.Invoke();
                isDoorClosed = true;
                isDoorOpen = false;
            }
        }

    }
}

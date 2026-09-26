using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;



[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class StackableObject : MonoBehaviour
{
    public event System.Action<StackableObject> onFailed;
    public event System.Action<StackableObject> onStable;

    [SerializeField, Range(0f, 1f)] float minUpwardNormal = 0.7f;
    Rigidbody rb;
    XRGrabInteractable grabInteractable;
    bool isResolved;
    bool isHeld;
   
    GameObject requiredSupport;
    bool isOnSupport;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        rb.maxDepenetrationVelocity = 0.3f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

    }
    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);

    }
    void OnGrabbed(SelectEnterEventArgs evArgs)
    {
        isHeld = true;

    }

    void OnReleased(SelectExitEventArgs evArgs)
    {
        isHeld = false;
        rb.isKinematic = false;
        rb.useGravity = true;
        StartCoroutine(WaitForSettle());

    }

    public void SetRequiredSupport(GameObject support)
    {
        requiredSupport = support;
        
    }


     void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject != requiredSupport) { return; }
        isOnSupport = false;
        for (int i = 0; i < collision.contactCount; i++) 
        {
            if (collision.GetContact(i).normal.y > minUpwardNormal)
            {
                isOnSupport = true;
                break;
            }

        }

    }

    void OnColissionExit(Collision collision)
    {
        if(collision.gameObject ==requiredSupport)
        {
            isOnSupport = false;
        }

    }

    bool IsAboveOfSupport()
    {
        if(requiredSupport == null) { return true; }
        return isOnSupport;
    }

    void OnTriggerEnter(Collider other)
    {

        if (isResolved || isHeld) {return; }

        if (other.CompareTag("FallZone"))
        {
            isResolved = true;
            onFailed?.Invoke(this);

        }


    }

    IEnumerator WaitForSettle()
    {
        yield return new WaitForSeconds(1.0f);

        while (!isResolved)
        {
            if (rb.IsSleeping())
            {
                isResolved= true;
                grabInteractable.enabled = false;

                if (IsAboveOfSupport())
                { 
                    onStable?.Invoke(this); 
                }
                else
                {
                    onFailed?.Invoke(this); 
                }
                
                yield break;

            }

            yield return null; 
        }

    }






}

using System;
using UnityEngine;

public class GolfBallController : MonoBehaviour
{
    public Collider collider;
    public Rigidbody body;
    public Transform anchor;
    public Transform hole;
    public float upwardVelocity = .25f;
    public float forwardVelocity = 8f;
    [Range(0.001f, 0.01f)]
    public float swingRate = 0.001f;
    public float groundRaycastDistance = 1f;

    [HideInInspector]
    public bool prepSwing = false;
    [HideInInspector]
    public float swingStrength = 0f;

    private int strengthBarDir = 1;
    private int missTimer = 0;

    private bool lerpToHole = false;
    
    void Start()
    {
        
    }

    void Update()
    {
        if (lerpToHole)
        {
            return;
        }
        
        var grounded = Physics.Raycast(transform.position, Vector3.down, groundRaycastDistance);
        if (Input.GetMouseButtonDown(0))
        {
            prepSwing = true;
            strengthBarDir = 1;
            swingStrength = 0f;
        }
        if (prepSwing)
        {
            if (Input.GetMouseButtonUp(0))
            {
                // Only allow hitting if the ball is currently grounded
                if (grounded)
                {
                    body.AddForce((anchor.forward + new Vector3(0, upwardVelocity * swingStrength, 0)) * (forwardVelocity * swingStrength), ForceMode.Impulse);
                    body.AddTorque(anchor.right * 10f, ForceMode.Impulse);
                }
                else
                {
                    MissSwing();
                }

                swingStrength = 0f;
                prepSwing = false;
            }
            else
            {
                swingStrength = Math.Clamp(swingStrength + swingRate * strengthBarDir, 0f, 1f);
                if (swingStrength == 0f || swingStrength == 1f)
                {
                    strengthBarDir = -strengthBarDir;
                }
            }
        }

        TickTimers();
    }

    private void FixedUpdate()
    {
        if (lerpToHole)
        {
            transform.position = Vector3.Lerp(transform.position, hole.position, .9f * Time.fixedDeltaTime);
        }
    }

    private void TickTimers()
    {
        if (missTimer > 0)
        {
            missTimer--;
            if (missTimer == 0)
            {
                var UI = GameObject.FindWithTag("UI");
                var missText = UI.transform.Find("Miss");
                missText.gameObject.SetActive(false);
            }
        }
    }

    private void MissSwing()
    {
        var UI = GameObject.FindWithTag("UI");
        var missText = UI.transform.Find("Miss");
        missText.gameObject.SetActive(true);
        missTimer = 300;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Hole"))
        {
            collider.enabled = false;
            body.isKinematic = true;
            lerpToHole = true;
        }
    }
}

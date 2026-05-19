using UnityEngine;

// Forțăm scriptul să folosească Debug-ul din Unity pentru a elimina erorile Ambiguous
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    private Vector3 startPoint;
    private Rigidbody rb;
    private LineRenderer lr;
    private PionRand pionRand;

    [Header("Setari Putere")]
    public float forceMultiplier = 15f;
    public float maxForce = 25f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lr = GetComponent<LineRenderer>();
        pionRand = GetComponent<PionRand>();

        if (lr != null)
        {
            lr.positionCount = 0;
            lr.useWorldSpace = true;
        }
    }

    void OnMouseDown()
    {
        if (pionRand != null && !pionRand.PotSaTrag())
        {
            Debug.Log("<color=yellow><b>[Meci]</b></color> Nu poți trage cu acest pion! Este rândul celeilalte echipe.");
            return;
        }

        startPoint = GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        if (pionRand != null && !pionRand.PotSaTrag()) return;

        Vector3 currentPoint = GetMouseWorldPos();

        if (lr != null)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, transform.position);

            Vector3 dragDirection = transform.position - currentPoint;
            Vector3 forwardPoint = transform.position + dragDirection;

            lr.SetPosition(1, forwardPoint);
        }
    }

    void OnMouseUp()
    {
        if (pionRand != null && !pionRand.PotSaTrag()) return;

        if (lr != null) lr.positionCount = 0;

        Vector3 endPoint = GetMouseWorldPos();
        Vector3 forceDirection = transform.position - endPoint;
        float magnitude = forceDirection.magnitude;

        if (magnitude < 0.1f) return;

        Vector3 finalForce = forceDirection.normalized * magnitude * forceMultiplier;

        if (finalForce.magnitude > maxForce)
        {
            finalForce = finalForce.normalized * maxForce;
        }

        rb.AddForce(finalForce, ForceMode.Impulse);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SchimbaRandul();
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position;
    }
}
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 startPoint;
    private Rigidbody rb;
    private LineRenderer lr;

    [Header("Setari Putere")]
    public float forceMultiplier = 15f;
    public float maxForce = 25f; // Limită pentru a nu zbura pionul prea tare

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lr = GetComponent<LineRenderer>();

        // Initializam linia sa fie invizibila
        if (lr != null)
        {
            lr.positionCount = 0;
            lr.useWorldSpace = true;
        }
    }

    void OnMouseDown()
    {
        // Salvam punctul de start al click-ului
        startPoint = GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        Vector3 currentPoint = GetMouseWorldPos();

        if (lr != null)
        {
            lr.positionCount = 2;
            // Punctul 0 este intotdeauna pozitia actuala a pionului
            lr.SetPosition(0, transform.position);

            // CALCULAM DIRECTIA IN FATA (Oglinda tragerii)
            // Calculam vectorul dintre deget/mouse si pion
            Vector3 dragDirection = transform.position - currentPoint;

            // Proiectam acest vector in fata pionului
            Vector3 forwardPoint = transform.position + dragDirection;

            // Setam capatul liniei in fata, indicand directia de mers
            lr.SetPosition(1, forwardPoint);
        }
    }

    void OnMouseUp()
    {
        // Ascundem linia cand ridicam degetul
        if (lr != null) lr.positionCount = 0;

        Vector3 endPoint = GetMouseWorldPos();

        // Calculam forța bazată pe cât de mult am tras înapoi
        Vector3 forceDirection = transform.position - endPoint;
        float magnitude = forceDirection.magnitude;

        // Aplicăm forța de impuls
        Vector3 finalForce = forceDirection.normalized * magnitude * forceMultiplier;

        // Limităm forța maximă pentru control mai bun
        if (finalForce.magnitude > maxForce)
        {
            finalForce = finalForce.normalized * maxForce;
        }

        rb.AddForce(finalForce, ForceMode.Impulse);
    }

    private Vector3 GetMouseWorldPos()
    {
        // Metoda sigura pentru Unity 6 si Input System "Both"
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position;
    }
}
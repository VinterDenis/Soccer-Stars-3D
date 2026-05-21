using UnityEngine;
using System.Collections.Generic;

public class DetectorGol : MonoBehaviour
{
    [Header("Configurare Poartă (Bifă Simplă)")]
    [Tooltip("Bifează doar dacă în această poartă trebuie să înceapă echipa BLUE (Albastră) după gol.")]
    public bool estePoartaAlbastra;

    public Vector3 pozitieCentruMinge = new Vector3(0, 0.5f, 0);
    private Rigidbody rbMinge;

    private List<Transform> pioni = new List<Transform>();
    private List<Vector3> pozitiiInitialePioni = new List<Vector3>();
    private List<Quaternion> rotatiiInitialePioni = new List<Quaternion>();

    void Start()
    {
        GameObject objMinge = GameObject.Find("Minge");
        if (objMinge != null) rbMinge = objMinge.GetComponent<Rigidbody>();

        PionRand[] totiPioniiScena = GameObject.FindObjectsByType<PionRand>(FindObjectsSortMode.None);
        foreach (PionRand p in totiPioniiScena)
        {
            if (p != null)
            {
                pioni.Add(p.transform);
                pozitiiInitialePioni.Add(p.transform.position);
                rotatiiInitialePioni.Add(p.transform.rotation);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Minge" || other.CompareTag("Minge"))
        {
            ResetareToataArena();

            if (GameManager.Instance != null)
            {
                // Trimitem numele în engleză conform setărilor din GameManager
                if (estePoartaAlbastra)
                {
                    GameManager.Instance.MarcatGolInPoarta("Blue");
                }
                else
                {
                    GameManager.Instance.MarcatGolInPoarta("Red");
                }
            }
        }
    }

    public void ResetareToataArena()
    {
        if (rbMinge != null)
        {
            rbMinge.transform.position = pozitieCentruMinge;
            rbMinge.linearVelocity = Vector3.zero;
            rbMinge.angularVelocity = Vector3.zero;
        }

        for (int i = 0; i < pioni.Count; i++)
        {
            if (pioni[i] != null)
            {
                pioni[i].position = pozitiiInitialePioni[i];
                pioni[i].rotation = rotatiiInitialePioni[i];

                Rigidbody rbPion = pioni[i].GetComponent<Rigidbody>();
                if (rbPion != null)
                {
                    rbPion.linearVelocity = Vector3.zero;
                    rbPion.angularVelocity = Vector3.zero;
                }
            }
        }
    }
}
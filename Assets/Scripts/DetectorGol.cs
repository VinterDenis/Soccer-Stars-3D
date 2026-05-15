using UnityEngine;
using System.Collections.Generic;

public class DetectorGol : MonoBehaviour
{
    public string numeEchipa;
    public Vector3 pozitieCentruMinge = new Vector3(0, 0.5f, 0);

    private Rigidbody rbMinge;

    // Liste pentru a salva datele pionilor
    private List<Transform> pioni = new List<Transform>();
    private List<Vector3> pozitiiInitialePioni = new List<Vector3>();
    private List<Quaternion> rotatiiInitialePioni = new List<Quaternion>();

    void Start()
    {
        // Gasim mingea
        GameObject objMinge = GameObject.Find("Minge");
        if (objMinge != null) rbMinge = objMinge.GetComponent<Rigidbody>();

        // Gasim toti pionii (Cilindrii) din scena
        // Presupunem ca toti pionii tai au un script de control sau un Tag specific
        // Daca nu au Tag, ii cautam dupa nume sau tip
        GameObject[] obiectePioni = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in obiectePioni)
        {
            pioni.Add(p.transform);
            pozitiiInitialePioni.Add(p.transform.position);
            rotatiiInitialePioni.Add(p.transform.rotation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            UnityEngine.Debug.Log("<color=cyan>GOOOL!</color> Resetare teren...");
            ResetareToataArena();
        }
    }

    public void ResetareToataArena()
    {
        // 1. Resetare Minge
        if (rbMinge != null)
        {
            rbMinge.transform.position = pozitieCentruMinge;
            rbMinge.linearVelocity = Vector3.zero;
            rbMinge.angularVelocity = Vector3.zero;
        }

        // 2. Resetare Pioni
        for (int i = 0; i < pioni.Count; i++)
        {
            if (pioni[i] != null)
            {
                pioni[i].position = pozitiiInitialePioni[i];
                pioni[i].rotation = rotatiiInitialePioni[i];

                // Daca pionii au Rigidbody, le oprim si lor viteza
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
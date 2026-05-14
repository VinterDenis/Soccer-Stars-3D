using UnityEngine;

public class DetectorGol : MonoBehaviour
{
    public string numeEchipa;
    private Vector3 pozitieInitialaMinge;
    private Rigidbody rbMinge;

    void Start()
    {
        // Găsim obiectul numit "Minge" din Hierarchy
        GameObject minge = GameObject.Find("Minge");
        if (minge != null)
        {
            pozitieInitialaMinge = minge.transform.position;
            rbMinge = minge.GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificăm dacă obiectul care a intrat în poartă are tag-ul "Finish"
        if (other.CompareTag("Finish"))
        {
            UnityEngine.Debug.Log("GOOOL pentru echipa: " + numeEchipa);
            ResetareMinge();
        }
    }

    void ResetareMinge()
    {
        if (rbMinge != null)
        {
            // Resetăm poziția la cea salvată în Start
            rbMinge.transform.position = pozitieInitialaMinge;

            // Oprim orice mișcare anterioară (Specific Unity 6)
            rbMinge.linearVelocity = Vector3.zero;
            rbMinge.angularVelocity = Vector3.zero;
        }
    }
}
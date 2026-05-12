using UnityEngine;

public class DetectorGol : MonoBehaviour
{
    // Această variabilă îți permite să scrii numele echipei direct în Unity
    [Header("Setari Scrut")]
    public string numeEchipa;

    private void OnTriggerEnter(Collider other)
    {
        // Verificăm dacă obiectul care a intrat în poartă este mingea (Tag: Finish)
        if (other.CompareTag("Finish"))
        {
            // Folosim explicit UnityEngine.Debug pentru a evita orice eroare viitoare
            UnityEngine.Debug.Log("GOOOL pentru echipa: " + numeEchipa);
        }
    }
}
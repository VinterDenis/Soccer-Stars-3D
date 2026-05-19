using UnityEngine;

public class PionRand : MonoBehaviour
{
    [Header("Configurare Pion")]
    public string echipaPion; // "Rosu" sau "Albastru"
    public GameObject luminaPion; // Trage aici obiectul Point Light din interiorul pionului

    private bool esteRandulMeu = false;
    private Light componentaLumina;

    void Awake()
    {
        // Preluăm componenta Light dacă obiectul a fost asociat în Inspector
        if (luminaPion != null)
        {
            componentaLumina = luminaPion.GetComponent<Light>();
        }
    }

    public void SetActiveRand(bool status)
    {
        esteRandulMeu = status;

        // Activăm sau dezactivăm obiectul și componenta de lumină
        // FĂRĂ să mai modificăm forțat valorile de Intensity sau Range din cod
        if (luminaPion != null)
        {
            luminaPion.SetActive(status);
        }

        if (componentaLumina != null)
        {
            componentaLumina.enabled = status;
        }
    }

    public bool PotSaTrag()
    {
        return esteRandulMeu;
    }
}
using UnityEngine;
using System.Collections.Generic;
using TMPro; // Obligatoriu pentru TextMeshPro

using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Sistem de Meniu Start")]
    public GameObject panouMeniu; // Trage aici obiectul Panou_Meniu

    [Header("Setări Rând")]
    public string randulEchipei; // "Red" sau "Blue"

    [Header("Setări Resetare Minge")]
    public Transform pozitieCentruMinge;
    private Rigidbody rbMinge;

    [Header("Sistem de Scor & UI")]
    public TextMeshProUGUI textScor; // Trage aici textul pentru scor (ex: "Blue 0 - 0 Red")
    public GameObject panouFinalMeci; // Trage aici panoul cu "Match Over"
    public TextMeshProUGUI textCastigator; // Trage aici textul din interiorul panoului (ex: "Red Wins!")

    private int scorRed = 0;
    private int scorBlue = 0;
    private bool meciTerminat = false;

    private List<PionRand> totiPionii = new List<PionRand>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        GameObject minge = GameObject.Find("Minge");
        if (minge != null) rbMinge = minge.GetComponent<Rigidbody>();

        if (panouFinalMeci != null) panouFinalMeci.SetActive(false); // Ascundem panoul final la început
        ActualizeazaTextScor();

        // INTEGRARE MENIU START:
        if (panouMeniu != null)
        {
            panouMeniu.SetActive(true); // Afișăm meniul principal
            if (textScor != null) textScor.gameObject.SetActive(false); // Ascundem tabela de scor în meniu
            Time.timeScale = 0f; // Înghețăm jocul fizic în fundal
        }
        else
        {
            Time.timeScale = 1f;
            PornireMeci();
        }
    }

    /// <summary>
    /// Funcție publică legată de evenimentul On Click() al butonului tău START GAME
    /// </summary>
    public void IncepeJocul()
    {
        if (panouMeniu != null)
        {
            panouMeniu.SetActive(false); // Închidem meniul de start
        }

        if (textScor != null)
        {
            textScor.gameObject.SetActive(true); // Afișăm tabela de scor când meciul începe efectiv
        }

        Time.timeScale = 1f; // Dezghețăm timpul în Unity pentru a porni fizica
        PornireMeci(); // Rulăm inițializarea pionilor și a rândului
        Debug.Log("<color=green><b>[JOC PORNIT]</b></color> Meciul a început oficial!");
    }

    void PornireMeci()
    {
        totiPionii.Clear();
        PionRand[] pioniGasiti = GameObject.FindObjectsByType<PionRand>(FindObjectsSortMode.None);
        totiPionii.AddRange(pioniGasiti);

        // Actualizăm direct echipa pionilor în scripturile lor din română în engleză
        foreach (PionRand pion in totiPionii)
        {
            if (pion != null)
            {
                if (pion.echipaPion == "Rosu" || pion.echipaPion == "Rosie") pion.echipaPion = "Red";
                if (pion.echipaPion == "Albastru" || pion.echipaPion == "Albastra") pion.echipaPion = "Blue";
            }
        }

        if (!meciTerminat)
        {
            randulEchipei = (Random.value > 0.5f) ? "Red" : "Blue";
            ActualizeazaLuminileTerenului();
        }
    }

    public void SchimbaRandul()
    {
        if (meciTerminat) return;

        randulEchipei = (randulEchipei == "Red") ? "Blue" : "Red";
        ActualizeazaLuminileTerenului();
    }

    public void MarcatGolInPoarta(string echipaPoarta)
    {
        if (meciTerminat) return;

        // Dacă s-a marcat în poarta Red -> Punctează Blue
        // Dacă s-a marcat în poarta Blue -> Punctează Red
        if (echipaPoarta == "Red")
        {
            scorBlue++;
            randulEchipei = "Red"; // Repune Red din centru
            Debug.Log("<color=cyan><b>[GOAL!]</b></color> Blue scored! Red restarts from center.");
        }
        else if (echipaPoarta == "Blue")
        {
            scorRed++;
            randulEchipei = "Blue"; // Repune Blue din centru
            Debug.Log("<color=red><b>[GOAL!]</b></color> Red scored! Blue restarts from center.");
        }

        ActualizeazaTextScor();
        ResetarePozitieMinge();

        // Verificăm limita de 2 goluri stabilită
        if (scorRed >= 2)
        {
            TerminaMeciul("Red Wins!");
        }
        else if (scorBlue >= 2)
        {
            TerminaMeciul("Blue Wins!");
        }
        else
        {
            ActualizeazaLuminileTerenului();
        }
    }

    void ActualizeazaTextScor()
    {
        if (textScor != null)
        {
            // Păstrăm ordinea dorită: Blue în stânga, Red în dreapta
            textScor.text = "Blue " + scorBlue + " - " + scorRed + " Red";

            // Forțăm interfața să se redeseneze instant
            textScor.ForceMeshUpdate();
        }
    }

    void TerminaMeciul(string mesajCastigator)
    {
        meciTerminat = true;

        // Stingem toate luminile de pe teren
        foreach (PionRand pion in totiPionii)
        {
            if (pion != null) pion.SetActiveRand(false);
        }

        // Oprim fizica mingii complet
        if (rbMinge != null)
        {
            rbMinge.linearVelocity = Vector3.zero;
            rbMinge.angularVelocity = Vector3.zero;
        }

        // Afișăm panoul de final de meci și textul aferent
        if (panouFinalMeci != null) panouFinalMeci.SetActive(true);
        if (textCastigator != null) textCastigator.text = mesajCastigator;

        Debug.Log("<color=yellow><b>[MATCH OVER]</b></color> " + mesajCastigator);
    }

    private void ResetarePozitieMinge()
    {
        if (rbMinge != null)
        {
            rbMinge.linearVelocity = Vector3.zero;
            rbMinge.angularVelocity = Vector3.zero;
            if (pozitieCentruMinge != null) rbMinge.transform.position = pozitieCentruMinge.position;
            else rbMinge.transform.position = new Vector3(0f, 0.5f, 0f);
        }
    }

    public void ActualizeazaLuminileTerenului()
    {
        if (meciTerminat) return;

        foreach (PionRand pion in totiPionii)
        {
            if (pion != null)
            {
                if (pion.echipaPion == randulEchipei)
                {
                    pion.SetActiveRand(true);
                }
                else
                {
                    pion.SetActiveRand(false);
                }
            }
        }
    }
}
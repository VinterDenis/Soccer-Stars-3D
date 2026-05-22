using UnityEngine;
using System.Collections.Generic;
using TMPro; // Obligatoriu pentru TextMeshPro
using System.Collections; // Obligatoriu pentru rutine (IEnumerator)

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
    public GameObject panouFinalMeci; // Trage aici obiectul Panou_Final duplicat
    public TextMeshProUGUI textCastigator; // Trage aici textul de titlu din Panou_Final (ex: "BLUE WINS!")

    [Header("Imagini UI Gol")]
    public GameObject imagineGolAlbastru; // Trage aici imaginea cu textul albastru din Canvas
    public GameObject imagineGolRosu;     // Trage aici imaginea cu textul roșu din Canvas

    [Header("Sistem Audio")]
    public AudioSource sursaAudio; // Trage aici obiectul _GameManager (care are componenta AudioSource)
    public AudioClip sunetBucurieGol; // Trage aici fișierul tău .mp3 descărcat cu GOOOL/suporteri

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

        // Ne asigurăm că ambele imagini de gol sunt stinse la început
        if (imagineGolAlbastru != null) imagineGolAlbastru.SetActive(false);
        if (imagineGolRosu != null) imagineGolRosu.SetActive(false);

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

    public void IncepeJocul()
    {
        if (panouMeniu != null)
        {
            panouMeniu.SetActive(false); // Închidem meniul de start
        }

        if (textScor != null)
        {
            textScor.gameObject.SetActive(true); // Afișăm tabela de scor
        }

        meciTerminat = false;
        scorRed = 0;
        scorBlue = 0;
        ActualizeazaTextScor();

        Time.timeScale = 1f; // Pornim fizica jocului
        PornireMeci(); // Inițializăm pionii și alegem rândul
        Debug.Log("<color=green><b>[JOC PORNIT]</b></color> Meciul a început oficial!");
    }

    void PornireMeci()
    {
        totiPionii.Clear();
        // Unity 6 optimizat pentru a evita avertismentele galbene (warnings)
        PionRand[] pioniGasiti = GameObject.FindObjectsByType<PionRand>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        totiPionii.AddRange(pioniGasiti);

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

        // --- PORNEȘTE SUNETUL DE GOL ȘI SUPORTERI ---
        if (sursaAudio != null && sunetBucurieGol != null)
        {
            sursaAudio.PlayOneShot(sunetBucurieGol);
        }

        if (echipaPoarta == "Red")
        {
            scorBlue++;
            randulEchipei = "Red";
            Debug.Log("<color=cyan><b>[GOAL!]</b></color> Blue scored! Red restarts from center.");
            StartCoroutine(AfiseazaImagineGol(imagineGolAlbastru));
        }
        else if (echipaPoarta == "Blue")
        {
            scorRed++;
            randulEchipei = "Blue";
            Debug.Log("<color=red><b>[GOAL!]</b></color> Red scored! Blue restarts from center.");
            StartCoroutine(AfiseazaImagineGol(imagineGolRosu));
        }

        ActualizeazaTextScor();
        ResetarePozitieMinge();

        if (scorRed >= 2)
        {
            TerminaMeciul("RED WINS!");
        }
        else if (scorBlue >= 2)
        {
            TerminaMeciul("BLUE WINS!");
        }
        else
        {
            ActualizeazaLuminileTerenului();
        }
    }

    private IEnumerator AfiseazaImagineGol(GameObject imagineDeAfisat)
    {
        if (imagineDeAfisat != null)
        {
            imagineDeAfisat.SetActive(true);
            yield return new WaitForSecondsRealtime(2.0f);
            imagineDeAfisat.SetActive(false);
        }
    }

    void ActualizeazaTextScor()
    {
        if (textScor != null)
        {
            textScor.text = "Blue " + scorBlue + " - " + scorRed + " Red";
            textScor.ForceMeshUpdate();
        }
    }

    void TerminaMeciul(string mesajCastigator)
    {
        meciTerminat = true;
        Time.timeScale = 0f; // Înghețăm jocul la final ca să nu se mai tragă în pioni/minge

        foreach (PionRand pion in totiPionii)
        {
            if (pion != null) pion.SetActiveRand(false); // Dezactivăm rândurile tuturor
        }

        if (rbMinge != null)
        {
            rbMinge.linearVelocity = Vector3.zero;
            rbMinge.angularVelocity = Vector3.zero;
        }

        if (panouFinalMeci != null) panouFinalMeci.SetActive(true); // Afișăm ecranul final clonat
        if (textCastigator != null) textCastigator.text = mesajCastigator; // "BLUE WINS!" sau "RED WINS!"

        Debug.Log("<color=yellow><b>[MATCH OVER]</b></color> " + mesajCastigator);
    }

    // --- LOGICĂ BUTOANE INTERFAȚĂ FINALĂ ---

    // Atașează funcția asta pe butonul "REMATCH"
    public void Rematch()
    {
        meciTerminat = false;
        scorRed = 0;
        scorBlue = 0;
        ActualizeazaTextScor();

        if (panouFinalMeci != null) panouFinalMeci.SetActive(false); // Închidem panoul de final

        Time.timeScale = 1f; // Dezghețăm timpul pentru noul meci
        ResetarePozitieMinge();
        PornireMeci();
        Debug.Log("<color=orange><b>[REMATCH]</b></color> Jocul a fost repornit!");
    }

    // Atașează funcția asta pe butonul "EXIT GAME"
    public void ExitToMenu()
    {
        Debug.Log("<color=red><b>[EXIT]</b></color> Jocul se închide...");

        // Folosim referința completă explicită pentru a elimina eroarea de ambiguitate (CS0104)
        UnityEngine.Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Oprește automat modul Play când testezi în Editor
#endif
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
                pion.SetActiveRand(pion.echipaPion == randulEchipei);
            }
        }
    }
}
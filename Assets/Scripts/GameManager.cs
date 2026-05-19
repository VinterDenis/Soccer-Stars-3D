using UnityEngine;
using System.Collections.Generic;

using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Setări Rând")]
    public string randulEchipei; // "Rosu" sau "Albastru"

    [Header("Setări Resetare Gol")]
    public Transform pozitieCentruMinge;
    private Rigidbody rbMinge;

    private List<PionRand> totiPionii = new List<PionRand>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        GameObject minge = GameObject.Find("Minge");

        if (minge != null)
        {
            rbMinge = minge.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError("<color=red><b>[EROARE]</b></color> Nu am găsit niciun obiect numit exact 'Minge' în Hierarchy!");
        }

        Invoke("PornireMeci", 0.1f);
    }

    void PornireMeci()
    {
        totiPionii.Clear();
        PionRand[] pioniGasiti = GameObject.FindObjectsByType<PionRand>(FindObjectsSortMode.None);
        totiPionii.AddRange(pioniGasiti);

        Debug.Log("<color=yellow><b>[DETECTIV]</b></color> Am găsit " + totiPionii.Count + " pioni pe teren.");

        randulEchipei = (Random.value > 0.5f) ? "Rosu" : "Albastru";

        string culoareHex = (randulEchipei == "Rosu") ? "red" : "cyan";
        Debug.Log("Meciul a început! Începe echipa: <color=" + culoareHex + "><b>" + randulEchipei + "</b></color>");

        ActualizeazaLuminileTerenului();
    }

    public void SchimbaRandul()
    {
        randulEchipei = (randulEchipei == "Rosu") ? "Albastru" : "Rosu";

        string culoareHex = (randulEchipei == "Rosu") ? "red" : "cyan";
        Debug.Log("S-a schimbat rândul! Acum este rândul echipei: <color=" + culoareHex + "><b>" + randulEchipei + "</b></color>");

        ActualizeazaLuminileTerenului();
    }

    // REGULA STRICTĂ DE GOL/AUTOGOL
    public void MarcatGolInPoarta(string echipaPoarta)
    {
        if (echipaPoarta == "Rosu")
        {
            randulEchipei = "Rosu";
            Debug.Log("<color=red><b>[GOL / AUTOGOL]</b></color> S-a marcat în poarta Roșie. Echipa Roșie începe de la centru!");
        }
        else if (echipaPoarta == "Albastru")
        {
            randulEchipei = "Albastru";
            Debug.Log("<color=cyan><b>[GOL / AUTOGOL]</b></color> S-a marcat în poarta Albastră. Echipa Albastră începe de la centru!");
        }

        ResetarePozitieMinge();
        ActualizeazaLuminileTerenului();
    }

    private void ResetarePozitieMinge()
    {
        if (rbMinge != null)
        {
            rbMinge.linearVelocity = Vector3.zero;
            rbMinge.angularVelocity = Vector3.zero;

            if (pozitieCentruMinge != null)
            {
                rbMinge.transform.position = pozitieCentruMinge.position;
            }
            else
            {
                rbMinge.transform.position = new Vector3(0f, 0.5f, 0f);
            }
        }
    }

    public void ActualizeazaLuminileTerenului()
    {
        int luminiAprinse = 0;
        foreach (PionRand pion in totiPionii)
        {
            if (pion != null)
            {
                if (pion.echipaPion == randulEchipei)
                {
                    pion.SetActiveRand(true);
                    luminiAprinse++;
                }
                else
                {
                    pion.SetActiveRand(false);
                }
            }
        }
        Debug.Log("<color=orange><b>[LUMINI]</b></color> Am actualizat terenul. Lumini aprinse acum: " + luminiAprinse);
    }
}
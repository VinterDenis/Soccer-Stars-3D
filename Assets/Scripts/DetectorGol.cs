using UnityEngine;

public class DetectorGol : MonoBehaviour
{
    public string numeEchipa;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            UnityEngine.Debug.Log("GOOOL pentru echipa: " + numeEchipa);
        }
    }
}
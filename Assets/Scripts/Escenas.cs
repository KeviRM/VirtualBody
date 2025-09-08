using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Escenas : MonoBehaviour
{
    public void CambiaEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }
}

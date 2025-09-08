using UnityEngine;
using System.Collections;

public class RobotSaludador : MonoBehaviour
{
    public Animator robotAnimator;
    public AudioSource robotAudioSource;
    public string saludoAnimationName = "hol"; // Asegúrate de que este nombre coincida con el estado de tu animación de saludo
    public float delayBeforeDisappear = 0.5f; // Tiempo de espera adicional después de terminar el audio antes de desaparecer

    private void Start()
    {
        // Asegurarse de que los componentes estén asignados
        if (robotAnimator == null)
        {
            Debug.LogError("Animator del robot no asignado en " + gameObject.name);
            enabled = false;
            return;
        }
        if (robotAudioSource == null)
        {
            Debug.LogError("AudioSource del robot no asignado en " + gameObject.name);
            enabled = false;
            return;
        }

        // Iniciar la animación y el audio al entrar en la escena
        if (!string.IsNullOrEmpty(saludoAnimationName))
        {
            robotAnimator.Play(saludoAnimationName);
        }
        else
        {
            Debug.LogWarning("No se ha especificado el nombre de la animación de saludo en " + gameObject.name);
        }

        if (robotAudioSource.clip != null)
        {
            robotAudioSource.Play();
            // Llamar a la función para detener la animación y desaparecer después de que termine el audio
            StartCoroutine(DetenerYDesaparecer());
        }
        else
        {
            Debug.LogWarning("No se ha asignado un clip de audio al AudioSource del robot en " + gameObject.name);
            // Si no hay audio, podrías querer detener la animación y desaparecer inmediatamente o después de un tiempo.
            // Por ejemplo:
            // robotAnimator.enabled = false;
            // StartCoroutine(DesaparecerDespuesDeTiempo(2f));
        }
    }

    private IEnumerator DetenerYDesaparecer()
    {
        // Esperar hasta que el audio termine de reproducirse
        while (robotAudioSource.isPlaying)
        {
            yield return null; // Esperar el siguiente frame
        }

        // Esperar un pequeño delay adicional (opcional)
        yield return new WaitForSeconds(delayBeforeDisappear);

        // Detener la animación desactivando el Animator
        robotAnimator.enabled = false;

        // Hacer que el objeto desaparezca desactivando su Renderer
        
        gameObject.SetActive(false);
        // Opcional: Si quieres destruir completamente el objeto después de desaparecer
        // Destroy(gameObject);
    }

    // Opcional: Función para desaparecer después de un tiempo (en caso de que no haya audio)
    private IEnumerator DesaparecerDespuesDeTiempo(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }
        // Opcional: Destroy(gameObject);
    }
}

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DetenerAnim : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;
    public string animationToPlay; // Nombre del estado de la animación que quieres reproducir
    public AudioClipWithName[] namedAudioClips; // Array de contenedores de AudioClip con nombre

    private bool isPlayingAnimation = false;
    private AudioClip currentAudioClip;

    // Clase interna para asociar un nombre a un AudioClip
    [System.Serializable]
    public class AudioClipWithName
    {
        public string clipName;
        public AudioClip clip;
    }

    void Start()
    {
        // Asegúrate de que el Animator y el AudioSource estén asignados
        if (animator == null)
        {
            Debug.LogError("Animator no asignado en " + gameObject.name);
            enabled = false;
            return;
        }
        if (audioSource == null)
        {
            Debug.LogError("AudioSource no asignado en " + gameObject.name);
            enabled = false;
            return;
        }
        // Asegúrate de que la animación a reproducir esté definida
        if (string.IsNullOrEmpty(animationToPlay))
        {
            Debug.LogError("Nombre de la animación a reproducir no definido en " + gameObject.name);
            enabled = false;
            return;
        }
        // Asegúrate de que haya al menos un clip de audio con nombre en la lista
        if (namedAudioClips == null || namedAudioClips.Length == 0)
        {
            Debug.LogWarning("No hay clips de audio con nombre asignados en " + gameObject.name);
        }
    }

    void Update()
    {
        // Si el audio ha terminado de reproducirse y la animación estaba en curso
        if (!audioSource.isPlaying && isPlayingAnimation)
        {
            // Detén la animación
            animator.enabled = false; // Desactiva el Animator para detener la animación en su frame actual
            isPlayingAnimation = false;

            // Desactiva el GameObject
            gameObject.SetActive(false);
        }
    }

    // Función para iniciar la animación y el audio por nombre
    public void PlayAnimationWithAudio(string audioName)
    {
        if (animator != null && audioSource != null && !string.IsNullOrEmpty(animationToPlay) && namedAudioClips != null && namedAudioClips.Length > 0)
        {
            AudioClip clipToPlay = namedAudioClips.FirstOrDefault(nac => nac.clipName == audioName)?.clip;

            if (clipToPlay != null)
            {
                // Activa el GameObject (en caso de que estuviera desactivado previamente)
                gameObject.SetActive(true);
                // Activa el Animator
                animator.enabled = true;
                // Reproduce la animación
                animator.Play(animationToPlay);
                // Asigna el clip de audio seleccionado al AudioSource
                audioSource.clip = clipToPlay;
                currentAudioClip = clipToPlay;
                // Reproduce el audio
                audioSource.Play();
                isPlayingAnimation = true;
            }
            else
            {
                Debug.LogError("No se encontró ningún clip de audio con el nombre: " + audioName + " en " + gameObject.name);
            }
        }
        else
        {
            string error = "No se puede iniciar la animación y el audio. Asegúrate de que Animator, AudioSource y animationToPlay estén correctamente configurados en " + gameObject.name + ".";
            if (namedAudioClips == null || namedAudioClips.Length == 0)
            {
                error += " No hay clips de audio con nombre asignados.";
            }
            Debug.LogError(error);
        }
    }

    // Opcional: Función para detener la animación y el audio inmediatamente
    public void StopAnimationAndAudio()
    {
        if (animator != null && audioSource != null)
        {
            animator.enabled = false;
            audioSource.Stop();
            isPlayingAnimation = false;
            gameObject.SetActive(false); // También desactivamos el GameObject al detener manualmente
        }
    }
}

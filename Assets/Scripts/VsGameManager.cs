using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class VsGameManager : MonoBehaviour
{
    [Header("Configuración UI")]
    public GameObject introPanel; // Panel "¿Estás listo?" 
    public TextMeshProUGUI countdownText; // Texto para la cuenta regresiva 
    public GameObject slotMachinePanel; // Panel de la tragamonedas 
    public GameObject winPanel;
    public GameObject losePanel;


    [Header("Configuración Tragamonedas")]
    public Image slotImageDisplay; // Donde se muestra el órgano
    public float minSpinDuration = 2.0f; // Tiempo mínimo girando

    [Header("Robot IA")]
    public RobotIAController robotController; // Arrastra al robot aquí
    public Transform robotMannequinSpawn; // Ubicación donde se pone el órgano en el maniquí robot


    [Header("Audio")]
    public AudioSource uiAudioSource;
    public AudioClip tickSound;       // Sonido "tic" al cambiar imagen
    public AudioClip organSelectedSound; // Sonido "ding" al final

    [Header("Referencias")]
    public AudioSource robotAudioSource; // Para la explicación del robot 
    public AudioClip introClip;

    [Header("Datos de Juego")]
    public List<OrganData> allOrgans; // Lista de órganos disponibles
    public List<Transform> coolerSpawns; // Ubicaciones de las hieleras 

    private List<OrganData> remainingOrgans;
    private bool isGameActive = false;

    void Start()
    {
        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {

        Debug.Log("1. Iniciando secuencia..."); // DEBUG

        // VALIDACIÓN: Si olvidaste asignar el clip en el script, usa el del AudioSource
        if (introClip == null && robotAudioSource.clip != null)
        {
            introClip = robotAudioSource.clip;
            Debug.LogWarning("Usando el audio asignado en el AudioSource porque IntroClip estaba vacío.");
        }

        if (introClip != null)
        {
            robotAudioSource.clip = introClip;
            robotAudioSource.Play();

            Debug.Log("2. Reproduciendo audio de: " + introClip.length + " segundos."); // DEBUG

            // Esperamos la duración exacta del audio
            yield return new WaitForSeconds(introClip.length);
        }
        else
        {
            Debug.LogError("ERROR: ¡No hay ningún audio asignado! Saltando espera."); // DEBUG
        }

        Debug.Log("3. Intentando activar panel..."); // DEBUG

        if (introPanel != null)
        {
            introPanel.SetActive(true);
            Debug.Log("4. ¡Panel activado!"); // DEBUG
        }
        else
        {
            Debug.LogError("ERROR: ¡Olvidaste asignar el introPanel en el Inspector!"); // DEBUG
        }
    }

    // Vincula este método al botón "Sí" del panel 
    public void OnPlayerReady()
    {
        introPanel.SetActive(false);
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        // Cuenta regresiva de 5 seg 
        float count = 5;
        while (count > 0)
        {
            countdownText.text = count.ToString("F0");
            yield return new WaitForSeconds(1f);
            count--;
        }
        countdownText.text = "¡YA!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);

        StartGame();
    }

    void StartGame()
    {
        remainingOrgans = new List<OrganData>(allOrgans);
        isGameActive = true;
        StartRoulette();
    }

    public void OnOrganPlacedCorrectly()
    {
        // Reproducir sonido de éxito / partículas

        // Verificar si ya ganó (si quedan órganos o no)
        if (remainingOrgans.Count == 0)
        {
            return;
            //EndGame(true); // ¡Ganaste!
        }
        else
        {// Siguiente turno: Girar tragamonedas de nuevo [cite: 41]
            StartRoulette();
        }
    }



    public void StartRoulette()
    {
        // 1. Elegir lógicamente el órgano ganador (sin repetir, como vimos antes)
        if (remainingOrgans.Count == 0) return; // O fin del juego

        int index = Random.Range(0, remainingOrgans.Count);
        OrganData targetOrgan = remainingOrgans[index];
        remainingOrgans.RemoveAt(index); // Lo sacamos para no repetir [cite: 27]

        // 2. Iniciar la animación visual
        StartCoroutine(AnimateSlotMachine(targetOrgan));
    }

    void SpawnOrganInCooler(OrganData organ)
    {
        // Lógica para instanciar el prefab en una hielera aleatoria 
        Transform randomCooler = coolerSpawns[Random.Range(0, coolerSpawns.Count)];
        Instantiate(organ.prefab, randomCooler.position, Quaternion.identity);

        robotController.GoFetchOrgan(randomCooler, organ.prefab);
    }


    public void OnRobotFinishedTurn()
    {
        Debug.Log("El robot colocó su órgano");
        // Aquí podrías validar si el robot ganó primero la ronda
        // O simplemente actualizar el marcador del robot
    }




    IEnumerator AnimateSlotMachine(OrganData targetOrgan)
    {
        slotMachinePanel.SetActive(true);
        float timer = 0f;
        float speed = 0.05f; // Tiempo entre cambios (0.05s = muy rápido)

        // Fase 1: Girar durante un tiempo base
        while (timer < minSpinDuration)
        {
            // Mostrar imagen aleatoria puramente visual
            slotImageDisplay.sprite = allOrgans[Random.Range(0, allOrgans.Count)].icon;

            // Reproducir sonido "tic"
            if (uiAudioSource && tickSound) uiAudioSource.PlayOneShot(tickSound);

            yield return new WaitForSeconds(speed);

            timer += speed;

            // Opcional: Hacer que vaya un poco más lento al final de esta fase
            if (timer > minSpinDuration * 0.7f) speed += 0.01f;
        }

        // Fase 2: Efecto de "frenado" final (últimos 3 cambios lentos)
        for (int i = 0; i < 3; i++)
        {
            slotImageDisplay.sprite = allOrgans[Random.Range(0, allOrgans.Count)].icon;
            if (uiAudioSource && tickSound) uiAudioSource.PlayOneShot(tickSound);
            yield return new WaitForSeconds(speed * 2); // Más lento
            speed += 0.1f;
        }

        // Fase 3: ¡Resultado Final!
        slotImageDisplay.sprite = targetOrgan.icon;
        if (uiAudioSource && organSelectedSound) uiAudioSource.PlayOneShot(organSelectedSound);

        // AQUÍ es donde realmente spawneamos el objeto en el juego
        SpawnOrganInCooler(targetOrgan);
    }

    // Método placeholder para definir struct
    [System.Serializable]
    public struct OrganData
    {
        public string organName;
        public GameObject prefab;
        public Sprite icon;
    }
}
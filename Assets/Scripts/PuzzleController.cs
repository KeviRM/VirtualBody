using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PuzzleController : MonoBehaviour
{
    public ParticleSystem celebrationParticles;
    public int totalObjectsToPlace = 10;
    private int objectsPlacedCount = 0;


    private void Awake()
    {
        celebrationParticles.Stop();
    }


    public void ObjectPlaced(XRBaseInteractable interactable)
    {
        objectsPlacedCount++;
        Debug.Log($"Objeto colocado. Total: {objectsPlacedCount}/{totalObjectsToPlace}");

        if (objectsPlacedCount >= totalObjectsToPlace)
        {
            if (celebrationParticles != null && !celebrationParticles.isPlaying)
            {
                celebrationParticles.Play();
                Debug.Log("¡Felicidades! Todos los objetos han sido colocados.");
            }
        }
    }

    public void ObjectRemoved(XRBaseInteractable interactable)
    {
        if (objectsPlacedCount > 0)
        {
            objectsPlacedCount--;
            Debug.Log($"Objeto removido. Total: {objectsPlacedCount}/{totalObjectsToPlace}");
            if (celebrationParticles != null && celebrationParticles.isPlaying)
            {
                celebrationParticles.Stop();
                Debug.Log("Se removió un objeto.");
            }
        }
    }

    //public void ResetPuzzle()
    //{
    //    objectsPlacedCount = 0;
    //    if (celebrationParticles != null && celebrationParticles.isPlaying)
    //    {
    //        celebrationParticles.Stop();
    //    }
    //    Debug.Log("Puzzle reseteado.");
    //}
}
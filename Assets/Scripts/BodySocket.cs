using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using System.Collections.Generic;

public class BodySocket : MonoBehaviour
{
    [Header("Configuración")]
    public string requiredID; 
    public XRSocketInteractor socketInteractor;

    [Header("Referencias")]
    public VsGameManager gameManager; 

    private void OnEnable()
    {
        // Nos suscribimos al evento de cuando algo entra en el socket
        socketInteractor.selectEntered.AddListener(OnOrganSnapped);
    }

    private void OnDisable()
    {
        socketInteractor.selectEntered.RemoveListener(OnOrganSnapped);
    }

    public void OnOrganSnapped(SelectEnterEventArgs args)
    {
        // el objeto que acaba de entrar
        GameObject organoColocado = args.interactableObject.transform.gameObject;

  
        OrganIdentity identity = organoColocado.GetComponent<OrganIdentity>();

        if (identity != null && identity.organID == requiredID)
        {
           
            Debug.Log("¡Órgano Correcto!");

            // Bloqueamos el órgano para que no lo puedan volver a sacar 
           args.interactableObject.transform.GetComponent<XRGrabInteractable>().enabled = false;

            // Avisamos al Manager para que sume punto y siga la ronda
            gameManager.OnOrganPlacedCorrectly();
        }
        else
        {
            Debug.Log("Órgano Incorrecto");

            // Forzamos al socket a escupir el objeto porque no pertenece ahí
            // Usamos un pequeño timer para expulsarlo o desactivamos el socket temporalmente
            StartCoroutine(EjectInvalidOrgan(organoColocado));
        }
    }

    System.Collections.IEnumerator EjectInvalidOrgan(GameObject organ)
    {
        // Feedback visual o auditivo de error aquí (sonido de error)

        // Desactivamos el socket un momento para soltar el objeto
        socketInteractor.interactionLayers = 0; // Nada puede interactuar
        yield return new WaitForSeconds(1f);
        socketInteractor.interactionLayers = ~0; // Restauramos capas (todo interactúa)
    }
}

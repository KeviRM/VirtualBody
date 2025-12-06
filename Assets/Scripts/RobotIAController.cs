using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobotIAController : MonoBehaviour
{

    [Header("Componentes")]
    public NavMeshAgent agent;
    public Animator animator;
    public Transform robotHand; // Asigna la mano derecha del robot aquí

    [Header("Destinos")]
    public Transform robotMannequinChest; // Donde el robot debe pararse para colocar
    private Transform currentTargetCooler; // La hielera a la que debe ir

    private GameObject currentOrganObject; // El órgano físico que lleva
    private bool isBusy = false;


    private int animMove = Animator.StringToHash("IsMoving");
    private int animGrab = Animator.StringToHash("TriggerGrab");
    private int animPlace = Animator.StringToHash("TriggerPlace");




    void Update()
    {
        // Actualizar animación de movimiento
        animator.SetBool(animMove, agent.velocity.magnitude > 0.1f);
    }

    // --- Recibir Órden ---
    public void GoFetchOrgan(Transform coolerPosition, GameObject organPrefab)
    {
        if (isBusy) return;
        StartCoroutine(RoutineFetchOrgan(coolerPosition, organPrefab));
    }

    // --- La Rutina de IA ---
    IEnumerator RoutineFetchOrgan(Transform coolerLoc, GameObject organPrefab)
    {
        isBusy = true;

        // -- Ir a la Hielera
        agent.SetDestination(coolerLoc.position);

        // Esperar a llegar 
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        // --Simular Agarre
        transform.LookAt(coolerLoc);

        // Animación de agarrar
        animator.SetTrigger(animGrab);
        yield return new WaitForSeconds(0.5f); // Esperar al punto exacto de la anim

        // Instanciar el órgano en la mano del robot (Truco visual)
        currentOrganObject = Instantiate(organPrefab, robotHand.position, robotHand.rotation);
        currentOrganObject.transform.SetParent(robotHand);

        // Desactivamos físicas del órgano para que no se caiga de la mano
        if (currentOrganObject.GetComponent<Rigidbody>())
            currentOrganObject.GetComponent<Rigidbody>().isKinematic = true;

        yield return new WaitForSeconds(0.5f); // Terminar animación

        //Ir al Maniquí del Robot
        agent.SetDestination(robotMannequinChest.position);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        // Colocar Órgano
        transform.LookAt(robotMannequinChest); // Mirar al maniquí
        animator.SetTrigger(animPlace);

        yield return new WaitForSeconds(0.5f); // Esperar animación

        // "Colocar" el órgano (destruimos el de la mano y activamos el del maniquí o lo movemos)
        // Por simplicidad visual, aquí lo destruimos y asumimos que el maniquí del robot lo muestra
        Destroy(currentOrganObject);

        // AVISAR AL MANAGER QUE EL ROBOT TERMINÓ
        FindObjectOfType<VsGameManager>().OnRobotFinishedTurn();

        isBusy = false;
    }

    // Funciones para burlas/derrota
    public void PlayVictoryAnim() { animator.SetTrigger("Victory"); }
    public void PlayDefeatAnim() { animator.SetTrigger("Defeat"); }





}

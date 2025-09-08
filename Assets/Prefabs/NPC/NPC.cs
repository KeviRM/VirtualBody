using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    [Header("Agente")]
    NavMeshAgent agent;
    Animator animator;

    [Header("Movimiento")]
    [SerializeField] float tiempo = 3.0f;
    [SerializeField] float rango = 8.0f;
    [SerializeField] float espera = 0.6f;

    [Header("Acciones")]
    bool esperando;



    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();


        MoverEnPuntos();

    }

    private void Update()
    {
        if (!esperando && !agent.pathPending && agent.remainingDistance < 0.5)
        {
            if (Random.value < espera)
            {

                StartCoroutine(EsperarM());
            }
            else
            {
                MoverEnPuntos();
            }

        }
        else
        {
            //animator.SetBool("caminar", false);

            animator.SetBool("parametro", agent.velocity.sqrMagnitude > 0.1f);
        }

    }


    private IEnumerator EsperarM()
    {
        esperando = true;
        animator.SetBool("parametro", false);
        yield return new WaitForSeconds(tiempo);
        MoverEnPuntos();
        esperando = false;

    }

    private void MoverEnPuntos()
    {

        Vector3 randomDireccion = Random.insideUnitSphere * rango;
        randomDireccion += transform.position;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDireccion, out hit, rango, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            animator.SetBool("parametro", true);
        }







    }
}

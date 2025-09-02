using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
public class EnemyAI : NetworkBehaviour
{
    private NavMeshAgent agent;
    private Transform target;

    [Header("Detection Player Range")]
    public float range = 8f;
    public float minDis = 1.5f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!IsServer) return;

        FindPlayers();

        if (target != null)
        {
            float dis = Vector3.Distance(transform.position, target.position);
            if (dis <= range)
            {
                agent.stoppingDistance = minDis;
                agent.SetDestination(target.position);
            }
            else
            {
                agent.ResetPath();
            }
        }
    }

    private void FindPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float distanMin = Mathf.Infinity;
        Transform disMinPlayer = null;

        for (int i = 0; i < players.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, players[i].transform.position);
            if (dist < distanMin)
            {
                distanMin = dist;
                disMinPlayer = players[i].transform;
            }
        }
        target = disMinPlayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Player"))
        {
            GetComponent<NetworkObject>().Despawn(true);
        }
    }
}
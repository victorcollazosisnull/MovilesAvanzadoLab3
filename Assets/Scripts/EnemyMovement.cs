using System;
using Unity.Netcode;
using UnityEngine;

public class EnemyMovement : NetworkBehaviour
{
    public float followDistance = 10f;
    public float moveSpeed = 3f;

    private Transform targetPlayer;

    void Update()
    {
        if (!IsServer) return; 

        if (targetPlayer == null)
        {
            return;
        }
        float distance = Vector3.Distance(transform.position, targetPlayer.position);

        if (distance <= followDistance)
        {
            Vector3.MoveTowards(transform.position, targetPlayer.position, followDistance);
        }
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

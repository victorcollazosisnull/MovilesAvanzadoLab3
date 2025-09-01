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
            FindPlayer();
            return;
        }

        float distance = Vector3.Distance(transform.position, targetPlayer.position);

        if (distance <= followDistance)
        {
            Vector3 direction = (targetPlayer.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    void FindPlayer()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObject = client.PlayerObject;
            if (playerObject != null)
            {
                targetPlayer = playerObject.transform;
                break; 
            }
        }
    }
}

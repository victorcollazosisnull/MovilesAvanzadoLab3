using System;
using Unity.Netcode;
using UnityEngine;

public class EnemyMovement : NetworkBehaviour
{
    public float followDistance = 10f;
    public float speed = 3f;
    private Transform targetPlayer;

    void Update()
    {
        if (!IsServer) return;

        if (targetPlayer == null)
        {
            FindClosestPlayer();
            return;
        }

        float distance = Vector3.Distance(transform.position, targetPlayer.position);
        if (distance <= followDistance)
        {
            Vector3 direction = (targetPlayer.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void FindClosestPlayer()
    {
        float minDistance = float.MaxValue;
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObj = client.PlayerObject;
            if (playerObj != null)
            {
                float dist = Vector3.Distance(transform.position, playerObj.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    targetPlayer = playerObj.transform;
                }
            }
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

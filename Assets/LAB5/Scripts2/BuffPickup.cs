using System;
using Unity.Netcode;
using UnityEngine;

public class BuffPickup : NetworkBehaviour
{
    private int buffAmount;

    private void Start()
    {
        buffAmount = UnityEngine.Random.Range(1, 4); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.AplyBuff(buffAmount);
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
using Unity.Netcode;
using UnityEngine;

public class RandomBuff : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AddBuffToPlayerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }
    [Rpc(SendTo.Server)]
    private void AddBuffToPlayerRpc(ulong playerID)
    {
        print("Aplicar buff a : " +  playerID);
        GetComponent<NetworkObject>().Despawn(true);
    }
}

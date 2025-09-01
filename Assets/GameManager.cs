using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public GameObject playerPrefab;
    public GameObject buffPrefab;

    public float currentBuffCount;
    public float buffSpawnCount;
    public override void OnNetworkSpawn()
    {
        SpawnPlayerRpc(NetworkManager.Singleton.LocalClientId);
    }
    [Rpc(SendTo.Server)]
    public void SpawnPlayerRpc(ulong id)
    {
        print("CurrentPlayer" + NetworkManager.Singleton.ConnectedClients.Count);
        GameObject player = Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(id, true);
    }
    private void Update()
    {
        if (IsServer /*&&  NetworkManager.Singleton.ConnectedClientsIds == 2*/)
        {
            currentBuffCount = Time.deltaTime;
            if (currentBuffCount > buffSpawnCount)
            {
                Vector3 randonPos = new Vector3(Random.Range(-8, 8), 0.5f, Random.Range(-8, 8));
                GameObject buff = Instantiate(buffPrefab, randonPos, Quaternion.identity);
                buff.GetComponent<NetworkObject>().Spawn(true);
                currentBuffCount = 0;
            }
        }
    }
}


using Unity.Netcode;
using UnityEngine;
public class GameManager : NetworkBehaviour
{
    [Header("Prefabs")]
    public GameObject playerPrefab;
    [Header("Buffers")]
    public GameObject buffPrefab;
    public float BuffSpawnCount = 4;
    public float currentBuffCount = 0;
    [Header("Enemies")]
    public GameObject enemyPrefab;
    public float enemySpawnInterval = 5f;
    private float enemySpawnTimer = 0f;

    public override void OnNetworkSpawn()
    {
        print("CurrentPlayer" + NetworkManager.Singleton.ConnectedClients.Count);
        SpawnPlayerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.Server)]
    public void SpawnPlayerRpc(ulong id)
    {
        GameObject player = Instantiate(playerPrefab);
        //player.GetComponent<NetworkObject>().Spawn(true);
        player.GetComponent<SimplePlayerController>().PlayerID.Value = id;

        player.GetComponent<NetworkObject>().SpawnWithOwnership(id, true);
    }
    void Update()
    {
        if (!IsServer) return;

        SpawnBuffer();

        SpawnEnemies();
    }
    private void SpawnEnemies()
    {
        enemySpawnTimer += Time.deltaTime;
        if (enemySpawnTimer > enemySpawnInterval)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-10, 10), 0.5f, Random.Range(-10, 10));
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            enemy.GetComponent<NetworkObject>().Spawn(true);
            enemySpawnTimer = 0;
        }
    }
    private void SpawnBuffer()
    {
        currentBuffCount += Time.deltaTime;
        if (currentBuffCount > BuffSpawnCount)
        {
            Vector3 randomPos = new Vector3(Random.Range(-8, 8), 0.5f, Random.Range(-8, 8));
            GameObject buff = Instantiate(buffPrefab, randomPos, Quaternion.identity);
            buff.GetComponent<NetworkObject>().Spawn(true);
            currentBuffCount = 0;
        }
    }
}




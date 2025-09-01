using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public GameObject playerPrefab;
    public GameObject buffPrefab;

    public float BuffSpawnCount = 4;
    public float currentBuffCount = 0;

    [Header("Enemies")]
    public GameObject enemyPrefab;
    public float enemySpawnInterval = 5f;
    private float enemySpawnTimer = 0f;

    void Start()
    {

    }

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


        currentBuffCount += Time.deltaTime;
        if (currentBuffCount > BuffSpawnCount)
        {
            Vector3 randomPos = new Vector3(Random.Range(-8, 8), 0.5f, Random.Range(-8, 8));
            GameObject buff = Instantiate(buffPrefab, randomPos, Quaternion.identity);
            buff.GetComponent<NetworkObject>().Spawn(true);
            currentBuffCount = 0;
        }


        enemySpawnTimer += Time.deltaTime;
        if (enemySpawnTimer > enemySpawnInterval)
        {
            Vector3 enemyPos = new Vector3(Random.Range(-10, 10), 0.5f, Random.Range(-10, 10));
            GameObject enemy = Instantiate(enemyPrefab, enemyPos, Quaternion.identity);
            enemy.GetComponent<NetworkObject>().Spawn(true);
            enemySpawnTimer = 0;
        }

    }
}



